<script lang="ts">
    import StoryApi, { type StoryEvent } from '$lib/api/StoryApi';
    import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
    import { onMount, onDestroy } from 'svelte';
    import {FeatureFlags} from "$lib/services/FeatureFlag/FeatureFlags";

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

<a class="config-link" href="/personalities">Config</a>

<style>
    .story-section h2 {
        margin-top: 0;
        color: var(--color-font);
        border-bottom: 2px solid var(--color-base-red);
        padding-bottom: 0.5rem;
    }

    .story-card {
        border-left: 4px solid var(--color-base-red);
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
        color: var(--color-font);
        margin: 0;
    }

    .story-footer {
        display: flex;
        justify-content: flex-end;
        padding-top: 0.5rem;
        border-top: 1px solid var(--color-font);
    }

    .story-timestamp {
        font-size: 0.9rem;
        color: var(--color-font);
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
        color: var(--color-font);
    }

    .error {
        color: var(--color-base-red);
    }

    .no-story {
        color: var(--color-font);
    }

    .hint {
        font-size: 0.9rem;
        color: #999;
        margin-top: 0.5rem;
    }

    .config-link {
        display: inline-block;
        margin-top: 1rem;
        padding: 0.5rem 1rem;
        opacity: 0.7;
    }
</style>
