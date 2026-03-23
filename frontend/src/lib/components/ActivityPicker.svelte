<script lang="ts">
	import ActivityApi, { type Activity } from '$lib/api/ActivityApi';
	import { onMount, onDestroy } from 'svelte';

	let currentActivity: Activity | null = null;
	let loading = true;
	let error = '';
	let picking = false;
	let pollInterval: ReturnType<typeof setInterval>;

	const api = new ActivityApi();
	const POLL_INTERVAL_MS = 5 * 60 * 1000; // 5 minutes

	async function fetchCurrentActivity() {
		try {
			error = '';
			currentActivity = await api.getCurrent();
		} catch (err) {
			error = 'Failed to fetch activity.';
			console.error('Error fetching current activity:', err);
		} finally {
			loading = false;
		}
	}

	onMount(() => {
		fetchCurrentActivity();
		pollInterval = setInterval(fetchCurrentActivity, POLL_INTERVAL_MS);
	});

	onDestroy(() => {
		clearInterval(pollInterval);
	});

	async function pickNow() {
		picking = true;
		error = '';
		try {
			const picked = await api.pickNow();
			currentActivity = picked;
		} catch (err) {
			error = 'Failed to pick an activity.';
			console.error('Error picking activity:', err);
		} finally {
			picking = false;
		}
	}
</script>

<div class="activity-section">
	<h2>Activity Picker</h2>

	{#if loading}
		<div class="loading">Loading...</div>
	{:else if error}
		<div class="error">{error}</div>
	{:else if currentActivity}
		<div class="activity-card">
			<div class="activity-name">{currentActivity.name}</div>
		</div>
	{:else}
		<div class="no-activity">
			<p>No activity has been picked yet.</p>
			<p class="hint">The first pick will happen when the hourly job runs.</p>
			<button class="pick-now-button" on:click={pickNow} disabled={picking}>
				{picking ? 'Picking...' : 'Pick Now'}
			</button>
		</div>
	{/if}

	<a class="manage-link" href="/activities">Manage</a>
</div>

<style>
	.activity-section h2 {
		margin-top: 0;
		border-bottom: 2px solid var(--color-base-green);
		padding-bottom: 0.5rem;
	}

	.activity-card {
		margin-top: 1rem;
		border-left: 4px solid var(--color-base-green);
		padding: 1rem 1.5rem;
		border-radius: 4px;
	}

	.activity-name {
		font-size: 1.5rem;
		font-weight: 700;
		color: var(--color-font);
	}

	.loading,
	.error,
	.no-activity {
		text-align: center;
		padding: 2rem;
		font-size: 1.1rem;
	}

	.error {
		color: var(--color-base-red);
	}

	.no-activity {
		color: var(--color-font);
	}

	.pick-now-button {
		margin-top: 1rem;
		padding: 0.5rem 1.25rem;
		background-color: var(--color-button-success);
		color: var(--color-primary-font);
		border: none;
		border-radius: 4px;
		cursor: pointer;
		font-size: 0.95rem;
	}

	.pick-now-button:hover:not(:disabled) {
		background-color: var(--color-button-success-hover);
	}

	.pick-now-button:disabled {
		opacity: 0.5;
		cursor: not-allowed;
	}

	.hint {
		font-size: 0.9rem;
		color: #999;
		margin-top: 0.5rem;
	}

	.manage-link {
		display: inline-block;
		margin-top: 1rem;
		padding: 0.5rem 1rem;
		opacity: 0.7;
	}
</style>
