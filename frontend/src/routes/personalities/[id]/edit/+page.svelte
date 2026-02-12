<script lang="ts">
	import PersonalityApi, { type Personality, type PersonalityInput } from '$lib/api/PersonalityApi';
	import PersonalityForm from '$lib/components/personality/PersonalityForm.svelte';
	import { cleanupOptionalFields } from '$lib/utils/formUtils';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';

	let personality: Personality | null = null;
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

	let isLoading = true;
	let isSubmitting = false;
	let error = '';

	const api = new PersonalityApi();

	async function loadPersonality() {
		const id = $page.params.id;
		isLoading = true;
		error = '';
		try {
			personality = await api.getById(id);
			formData = {
				name: personality.name,
				description: personality.description,
				age: personality.age,
				physicalFeatures: personality.physicalFeatures,
				loves: personality.loves,
				likes: personality.likes,
				dislikes: personality.dislikes,
				hates: personality.hates,
				relationships: personality.relationships,
				typicalClothing: personality.typicalClothing,
				dreamsForFuture: personality.dreamsForFuture,
				whatTheyWillTalkAbout: personality.whatTheyWillTalkAbout,
				occupation: personality.occupation,
				background: personality.background,
				motivations: personality.motivations
			};
		} catch (e) {
			error = 'Failed to load personality';
			console.error(e);
		} finally {
			isLoading = false;
		}
	}

	async function handleSubmit() {
		error = '';
		if (!formData.name || !formData.description) {
			error = 'Name and description are required';
			return;
		}

		const id = $page.params.id;
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

			await api.update(id, formData);
			goto(`/personalities/${id}`);
		} catch (e) {
			error = 'Failed to update personality';
			console.error(e);
		} finally {
			isSubmitting = false;
		}
	}

	function handleCancel() {
		goto(`/personalities/${$page.params.id}`);
	}

	onMount(() => {
		loadPersonality();
	});
</script>

<svelte:head>
	<title>Edit {personality?.name || 'Personality'}</title>
</svelte:head>

<div class="container">
	{#if isLoading}
		<p>Loading...</p>
	{:else if error && !personality}
		<p class="error">{error}</p>
		<a href="/personalities" class="button">Back to List</a>
	{:else}
		<h1>Edit Personality</h1>

		<PersonalityForm
			bind:formData
			onSubmit={handleSubmit}
			onCancel={handleCancel}
			{isSubmitting}
			{error}
			submitLabel="Save Changes"
		/>
	{/if}
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

	.button {
		padding: 0.5rem 1rem;
		background-color: #007bff;
		color: white;
		border: none;
		border-radius: 4px;
		cursor: pointer;
		text-decoration: none;
		display: inline-block;
	}

	.button:hover {
		background-color: #0056b3;
	}

	.error {
		color: red;
		margin-bottom: 1rem;
	}
</style>
