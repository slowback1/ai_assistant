<script lang="ts">
	import WeatherApi, { type WeatherData } from '$lib/api/WeatherApi';
	import { onMount, onDestroy } from 'svelte';

	let weather: WeatherData | null = null;
	let loading = true;
	let error = '';
	let pollInterval: number;

	const api = new WeatherApi();
	const WEATHER_REFRESH_INTERVAL_MS = 3_600_000; // 1 hour

	async function fetchWeather() {
		try {
			loading = true;
			error = '';
			weather = await api.GetCurrentWeather();
		} catch (err) {
			error = 'Failed to fetch weather data. Please try again later.';
			console.error('Error fetching weather:', err);
		} finally {
			loading = false;
		}
	}

	onMount(() => {
		fetchWeather();
		// Poll for updated weather once an hour
		pollInterval = setInterval(fetchWeather, WEATHER_REFRESH_INTERVAL_MS);
	});

	onDestroy(() => {
		if (pollInterval) {
			clearInterval(pollInterval);
		}
	});
</script>

<div class="weather-section">
	<h2>Current Weather</h2>

	{#if loading && !weather}
		<div class="loading">Loading weather...</div>
	{:else if error}
		<div class="error">{error}</div>
	{:else if weather}
		<div class="weather-card">
			<div class="weather-location">{weather.name}</div>
			<div class="weather-main">
				{#if weather.conditionIcon}
					<img
						src="https://{weather.conditionIcon}"
						alt={weather.condition}
						class="weather-icon"
					/>
				{/if}
				<div class="weather-temp">{weather.tempF}°F</div>
			</div>
			<div class="weather-condition">{weather.condition}</div>
			<div class="weather-details">
				<div class="weather-detail">
					<span class="detail-label">Wind</span>
					<span class="detail-value">{weather.windMph} mph</span>
				</div>
				<div class="weather-detail">
					<span class="detail-label">Precip</span>
					<span class="detail-value">{weather.precipIn} in</span>
				</div>
				<div class="weather-detail">
					<span class="detail-label">Heat Index</span>
					<span class="detail-value">{weather.heatindexF}°F</span>
				</div>
			</div>
		</div>
	{:else}
		<div class="no-weather">
			<p>Weather data is unavailable.</p>
		</div>
	{/if}
</div>

<style>
	.weather-section {
		border-radius: 8px;
		padding: 2rem;
		height: fit-content;
	}

	.weather-section h2 {
		margin-top: 0;
		border-bottom: 2px solid var(--color-base-blue);
		padding-bottom: 0.5rem;
	}

	.weather-card {
		margin-top: 1rem;
	}

	.weather-location {
		font-size: 1.2rem;
		font-weight: 600;
		margin-bottom: 0.5rem;
	}

	.weather-main {
		display: flex;
		align-items: center;
		gap: 1rem;
		margin-bottom: 0.5rem;
	}

	.weather-icon {
		width: 64px;
		height: 64px;
	}

	.weather-temp {
		font-size: 2.5rem;
		font-weight: 700;
        color: var(--color-base-blue);
	}

	.weather-condition {
		font-size: 1.1rem;
		margin-bottom: 1rem;
	}

	.weather-details {
		display: flex;
		gap: 1.5rem;
		flex-wrap: wrap;
		padding-top: 0.75rem;
		border-top: 1px solid var(--color-base-blue);
	}

	.weather-detail {
		display: flex;
		flex-direction: column;
	}

	.detail-label {
		font-size: 0.8rem;
		text-transform: uppercase;
        color: color-mix(in lab, var(--color-base-blue) 30%, var(--color-font));
	}

	.detail-value {
		font-size: 1rem;
		font-weight: 500;
	}

	.loading,
	.error,
	.no-weather {
		text-align: center;
		padding: 2rem;
		font-size: 1.1rem;
	}

	.error {
		color: var(--color-base-red);
	}
</style>
