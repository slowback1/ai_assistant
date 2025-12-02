<script lang="ts">
	import { goto } from '$app/navigation';
	import HumanActivityForm from '$lib/components/humanActivity/HumanActivityForm.svelte';
	import HumanActivityApi from '$lib/api/humanActivity/HumanActivityApi';
	import type { HumanActivityInput } from '$lib/api/humanActivity/HumanActivityTypes';
	import { ActivityFrequency } from '$lib/api/humanActivity/HumanActivityTypes';

	const api = new HumanActivityApi();
	let isSubmitting = false;
	let activity: HumanActivityInput = {
		name: '',
		description: '',
		frequency: ActivityFrequency.Daily
	};

	async function handleSubmit(data: HumanActivityInput) {
		isSubmitting = true;
		const result = await api.create(data);
		isSubmitting = false;

		if (result) {
			goto('/activities');
		}
	}

	function handleCancel() {
		goto('/activities');
	}
</script>

<svelte:head>
	<title>Create Activity</title>
</svelte:head>

<div class="create-activity-page">
	<h1>Create Activity</h1>

	<HumanActivityForm
		{activity}
		onSubmit={handleSubmit}
		onCancel={handleCancel}
		submitLabel="Create"
		{isSubmitting}
	/>
</div>

<style>
	.create-activity-page {
		padding: 1rem;
	}
</style>
