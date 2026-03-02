<script lang="ts">
    import StoryApi, { type StoryEvent } from '$lib/api/StoryApi';
    import DonetickTaskList from '$lib/components/DonetickTaskList.svelte';
    import WeatherWidget from '$lib/components/WeatherWidget.svelte';
    import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
    import { onMount, onDestroy } from 'svelte';
    import {FeatureFlags} from "$lib/services/FeatureFlag/FeatureFlags";
    import LatestStory from "$lib/components/LatestStory.svelte";

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
            <div class="donetick-section card">
                <DonetickTaskList />
            </div>
        {/if}

        <div class="weather-widget-section card">
            <WeatherWidget />
        </div>

        <div class="story-section card">
            <LatestStory />
        </div>
    </div>
</div>

<style>
    .dashboard-content {
        display: grid;
        gap: 2rem;
        grid-template-columns: 1fr 1fr;
    }

    .card {
        border-radius: 8px;
        padding: 2rem;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        height: fit-content;
        background: var(--color-card);
    }
</style>
