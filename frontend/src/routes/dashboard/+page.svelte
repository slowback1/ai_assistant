<script lang="ts">
	import StoryApi, { type StoryEvent } from '$lib/api/StoryApi';
	import { onMount, onDestroy } from 'svelte';

	let latestStory: StoryEvent | null = null;
	let loading = true;
	let error = '';
	let pollInterval: number;

	const api = new StoryApi();

	async function fetchLatestStory() {
		try {
			loading = true;
			error = '';
			const story = await api.GetLatestStory();
			latestStory = story;
		} catch (err) {
			error = 'Failed to fetch the latest story. Please try again later.';
			console.error('Error fetching story:', err);
		} finally {
			loading = false;
		}
	}

	onMount(() => {
		fetchLatestStory();
		// Poll for new stories every 30 seconds
		pollInterval = setInterval(fetchLatestStory, 30000);
	});

	onDestroy(() => {
		if (pollInterval) {
			clearInterval(pollInterval);
		}
	});

	function formatDate(dateString: string): string {
		const date = new Date(dateString);
		return date.toLocaleString();
	}
</script>

<svelte:head>
	<title>Dashboard - Character Story</title>
</svelte:head>

<div class="dashboard-container">
	<div class="dashboard-header">
		<h1>Darth Vader's Adventure Dashboard</h1>
		<p class="subtitle">Following the dark lord's latest exploits across the galaxy</p>
	</div>

	<div class="dashboard-content">
		<div class="story-section">
			<h2>Latest Story Event</h2>
			
			{#if loading && !latestStory}
				<div class="loading">Loading the latest adventure...</div>
			{:else if error}
				<div class="error">{error}</div>
			{:else if latestStory}
				<div class="story-card">
					<div class="story-content">
						<p class="story-text">{latestStory.story}</p>
					</div>
					<div class="story-footer">
						<span class="story-timestamp">
							{formatDate(latestStory.createdAt)}
						</span>
					</div>
				</div>
			{:else}
				<div class="no-story">
					<p>No stories have been generated yet.</p>
					<p class="hint">The first story will appear once the hourly job runs.</p>
				</div>
			{/if}
		</div>

		<!-- Placeholder for future dashboard sections -->
		<div class="placeholder-section">
			<h2>More Features Coming Soon</h2>
			<p>This space is reserved for future dashboard enhancements.</p>
		</div>
	</div>
</div>

<style>
	.dashboard-container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 2rem;
	}

	.dashboard-header {
		text-align: center;
		margin-bottom: 3rem;
	}

	.dashboard-header h1 {
		font-size: 2.5rem;
		color: #e63946;
		margin-bottom: 0.5rem;
	}

	.subtitle {
		font-size: 1.1rem;
		color: #666;
	}

	.dashboard-content {
		display: grid;
		gap: 2rem;
	}

	.story-section {
		background: white;
		border-radius: 8px;
		padding: 2rem;
		box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
	}

	.story-section h2 {
		margin-top: 0;
		color: #333;
		border-bottom: 2px solid #e63946;
		padding-bottom: 0.5rem;
	}

	.story-card {
		background: #f8f9fa;
		border-left: 4px solid #e63946;
		padding: 1.5rem;
		border-radius: 4px;
		margin-top: 1rem;
	}

	.story-content {
		margin-bottom: 1rem;
	}

	.story-text {
		font-size: 1.1rem;
		line-height: 1.6;
		color: #333;
		margin: 0;
	}

	.story-footer {
		display: flex;
		justify-content: flex-end;
		padding-top: 0.5rem;
		border-top: 1px solid #ddd;
	}

	.story-timestamp {
		font-size: 0.9rem;
		color: #666;
		font-style: italic;
	}

	.loading,
	.error,
	.no-story {
		text-align: center;
		padding: 2rem;
		font-size: 1.1rem;
	}

	.loading {
		color: #666;
	}

	.error {
		color: #e63946;
	}

	.no-story {
		color: #666;
	}

	.hint {
		font-size: 0.9rem;
		color: #999;
		margin-top: 0.5rem;
	}

	.placeholder-section {
		background: white;
		border-radius: 8px;
		padding: 2rem;
		box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
		text-align: center;
		color: #999;
		border: 2px dashed #ddd;
		min-height: 200px;
		display: flex;
		flex-direction: column;
		justify-content: center;
	}

	.placeholder-section h2 {
		color: #999;
		margin-bottom: 1rem;
	}
</style>
