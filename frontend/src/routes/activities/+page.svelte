<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import Table from '$lib/ui/table/table/Table.svelte';
	import type { TableColumnDefinition } from '$lib/ui/table/table/tableTypes';
	import HumanActivityApi from '$lib/api/humanActivity/HumanActivityApi';
	import type { HumanActivity } from '$lib/api/humanActivity/HumanActivityTypes';
	import ActivityActionsCell from '$lib/components/humanActivity/ActivityActionsCell.svelte';
	import FrequencyCell from '$lib/components/humanActivity/FrequencyCell.svelte';

	let activities: HumanActivity[] = [];
	let loading = true;
	const api = new HumanActivityApi();

	const columns: TableColumnDefinition<HumanActivity>[] = [
		{ key: 'name', title: 'Name', renderKey: 'name' },
		{ key: 'description', title: 'Description', renderKey: 'description' },
		{
			key: 'frequency',
			title: 'Frequency',
			renderTemplate: FrequencyCell
		},
		{
			key: 'actions',
			title: 'Actions',
			renderTemplate: ActivityActionsCell
		}
	];

	onMount(async () => {
		await loadActivities();
	});

	async function loadActivities() {
		loading = true;
		activities = await api.getAll();
		loading = false;
	}
</script>

<svelte:head>
	<title>Human Activities</title>
</svelte:head>

<div class="activities-page">
	<header class="page-header">
		<h1>Human Activities</h1>
		<Button href="/activities/create" testId="create-activity-button">Create Activity</Button>
	</header>

	{#if loading}
		<p>Loading...</p>
	{:else if activities.length === 0}
		<p data-testid="no-activities-message">No activities found. Create one to get started!</p>
	{:else}
		<Table {columns} rows={activities} />
	{/if}
</div>

<style>
	.activities-page {
		padding: 1rem;
	}

	.page-header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 2rem;
	}
</style>
