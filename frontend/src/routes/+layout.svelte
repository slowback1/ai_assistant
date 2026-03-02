<script lang="ts">
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import Header from '$lib/components/navigation/Header.svelte';
	import UrlPathProvider, { RealUrlProvider } from '$lib/providers/urlPathProvider';
	import { ColorTheme } from '$lib/services/Theme/ThemeService';
	import { Messages } from '$lib/bus/Messages';
	import ConfigService from '$lib/services/Config/ConfigService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import ConfigFeatureFlagProvider from '$lib/services/FeatureFlag/ConfigFeatureFlagProvider';
	import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';

	let currentTheme: ColorTheme = ColorTheme.Light;
    let isLoaded = false;

	onMount(() => {
		MessageBus.initialize(new LocalStorageProvider());
		UrlPathProvider.initialize(new RealUrlProvider());
		ConfigService.initialize();
		FeatureFlagService.initialize(new ConfigFeatureFlagProvider());

        //todo - this is a band-aid for an issue where the app loads before config is fully loaded, replace with a more robust solution
        setTimeout(() => {
            isLoaded = true;
        }, 250);

		MessageBus.subscribe<ColorTheme>(
			Messages.CurrentTheme,
			(value) => (currentTheme = value ?? ColorTheme.Light)
		);
	});
</script>

<svelte:head>
	<meta name="description" content="The Svelte Starter Kit!!!" />
</svelte:head>

{#if isLoaded}
    <div
            class="dark-theme"
    >
        <ToastWrapper />
        <main id="content" class="main-content">
            <slot />
        </main>
    </div>
{/if}


<style global>
	@import '../style/reset.css';
	@import '../style/globals.css';

	.main-content {
		min-height: calc(100vh - var(--gutters-y) * 2 - var(--header-height));
		padding: var(--gutters-y) var(--gutters-x);
		scroll-behavior: auto;
		display: flex;
		flex-direction: column;
	}
</style>
