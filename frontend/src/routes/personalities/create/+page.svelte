<script lang="ts">
	import PersonalityApi, { type PersonalityInput } from '$lib/api/PersonalityApi';
	import PersonalityForm from '$lib/components/personality/PersonalityForm.svelte';
	import { cleanupOptionalFields } from '$lib/utils/formUtils';
	import { goto } from '$app/navigation';

	let formData: PersonalityInput = {
		name: '',
		description: '',
		age: '',
		physicalFeatures: '',
		loves: [],
		likes: [],
		dislikes: [],
		hates: [],
		relationships: [],
		typicalClothing: '',
		dreamsForFuture: '',
		whatTheyWillTalkAbout: '',
		occupation: '',
		background: '',
		motivations: ''
	};

	let isSubmitting = false;
	let error = '';

	const api = new PersonalityApi();

	async function handleSubmit() {
		error = '';
		if (!formData.name || !formData.description) {
			error = 'Name and description are required';
			return;
		}

		isSubmitting = true;
		try {
			// Clean up empty strings
			cleanupOptionalFields(formData, [
				'age',
				'physicalFeatures',
				'typicalClothing',
				'dreamsForFuture',
				'whatTheyWillTalkAbout',
				'occupation',
				'background',
				'motivations'
			]);

			const created = await api.create(formData);
			goto(`/personalities/${created.id}`);
		} catch (e) {
			error = 'Failed to create personality';
			console.error(e);
		} finally {
			isSubmitting = false;
		}
	}

	function handleCancel() {
		goto('/personalities');
	}
</script>

<svelte:head>
	<title>Create Personality</title>
</svelte:head>

<div class="container">
	<h1>Create New Personality</h1>

	<PersonalityForm
		bind:formData
		onSubmit={handleSubmit}
		onCancel={handleCancel}
		{isSubmitting}
		{error}
		submitLabel="Create Personality"
	/>
</div>

<style>
	.container {
		max-width: 800px;
		margin: 0 auto;
		padding: 2rem;
	}

	h1 {
		margin-bottom: 2rem;
	}
</style>
