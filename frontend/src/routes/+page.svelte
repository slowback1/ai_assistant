<script lang="ts">
    import StoryApi, { type StoryEvent } from '$lib/api/StoryApi';
    import DonetickTaskList from '$lib/components/DonetickTaskList.svelte';
    import WeatherWidget from '$lib/components/WeatherWidget.svelte';
    import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
    import { onMount, onDestroy } from 'svelte';
    import {FeatureFlags} from "$lib/services/FeatureFlag/FeatureFlags";

    let latestStory: StoryEvent | null = null;
    let loading = true;
    let error = '';
    let pollInterval: number;
    let donetickEnabled = false;
    let featureFlagUnsubscribe: (() => void) | null = null;

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

        // Subscribe to Donetick feature flag
        featureFlagUnsubscribe = FeatureFlagService.subscribeToFeature(FeatureFlags.DONETICK_ENABLED, (isEnabled) => {
            donetickEnabled = isEnabled;
        });
    });

    onDestroy(() => {
        if (pollInterval) {
            clearInterval(pollInterval);
        }
        if (featureFlagUnsubscribe) {
            featureFlagUnsubscribe();
        }
    });

    function formatDate(dateString: string): string {
        const date = new Date(dateString);
        return date.toLocaleString();
    }
</script>

<svelte:head>
    <title>Dashboard</title>
</svelte:head>

<div class="dashboard-container">
    <div class="dashboard-content">
        {#if donetickEnabled}
            <div class="donetick-section">
                <DonetickTaskList />
            </div>
        {/if}

        <div class="weather-widget-section">
            <WeatherWidget />
        </div>

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
    </div>
</div>

<style>
    .dashboard-header h1 {
        font-size: 2.5rem;
        color: #e63946;
        margin-bottom: 0.5rem;
    }

    .dashboard-content {
        display: grid;
        gap: 2rem;
        grid-template-columns: 1fr 1fr;
    }

    .story-section {
        background: white;
        border-radius: 8px;
        padding: 2rem;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        height: fit-content;
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
</style>
