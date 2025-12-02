<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import HumanActivityForm from '$lib/components/humanActivity/HumanActivityForm.svelte';
	import HumanActivityApi from '$lib/api/humanActivity/HumanActivityApi';
	import type { HumanActivityInput } from '$lib/api/humanActivity/HumanActivityTypes';
	import { ActivityFrequency } from '$lib/api/humanActivity/HumanActivityTypes';
	import Button from '$lib/ui/buttons/Button/Button.svelte';

	const api = new HumanActivityApi();
	let isSubmitting = false;
	let isDeleting = false;
	let loading = true;
	let activityId = '';
	let activity: HumanActivityInput = {
		name: '',
		description: '',
		frequency: ActivityFrequency.Daily
	};
	let notFound = false;

	onMount(async () => {
		activityId = $page.params.id;
		const result = await api.getById(activityId);
		loading = false;

		if (result) {
			activity = {
				name: result.name,
				description: result.description,
				frequency: result.frequency
			};
		} else {
			notFound = true;
		}
	});

	async function handleSubmit(data: HumanActivityInput) {
		isSubmitting = true;
		const result = await api.update(activityId, data);
		isSubmitting = false;

		if (result) {
			goto('/activities');
		}
	}

	async function handleDelete() {
		if (confirm('Are you sure you want to delete this activity?')) {
			isDeleting = true;
			const success = await api.delete(activityId);
			isDeleting = false;

			if (success) {
				goto('/activities');
			}
		}
	}

	function handleCancel() {
		goto('/activities');
	}
</script>

<svelte:head>
	<title>Edit Activity</title>
</svelte:head>

<div class="edit-activity-page">
	<h1>Edit Activity</h1>

	{#if loading}
		<p>Loading...</p>
	{:else if notFound}
		<p data-testid="not-found-message">Activity not found.</p>
		<Button href="/activities" testId="back-button">Back to Activities</Button>
	{:else}
		<HumanActivityForm
			{activity}
			onSubmit={handleSubmit}
			onCancel={handleCancel}
			submitLabel="Update"
			{isSubmitting}
		/>

		<div class="danger-zone">
			<h2>Danger Zone</h2>
			<Button
				testId="delete-button"
				variant="secondary"
				onClick={handleDelete}
				disabled={isDeleting}
			>
				{isDeleting ? 'Deleting...' : 'Delete Activity'}
			</Button>
		</div>
	{/if}
</div>

<style>
	.edit-activity-page {
		padding: 1rem;
	}

	.danger-zone {
		margin-top: 2rem;
		padding: 1rem;
		border: 1px solid var(--color-font);
		border-radius: 4px;
	}

	.danger-zone h2 {
		margin-bottom: 1rem;
		color: inherit;
	}
</style>
