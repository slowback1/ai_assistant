<script lang="ts">
	import PersonalityApi, { type Personality, type PersonalityInput } from '$lib/api/PersonalityApi';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { cleanupOptionalFields } from '$lib/utils/formUtils';

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

	let lovesText = '';
	let likesText = '';
	let dislikesText = '';
	let hatesText = '';
	let relationshipsText = '';

	let isLoading = true;
	let isSubmitting = false;
	let error = '';

	const api = new PersonalityApi();

	function parseListField(text: string): string[] | undefined {
		if (!text.trim()) return undefined;
		return text
			.split('\n')
			.map((line) => line.trim())
			.filter((line) => line.length > 0);
	}

	function arrayToText(arr: string[] | undefined): string {
		return arr ? arr.join('\n') : '';
	}

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

			lovesText = arrayToText(personality.loves);
			likesText = arrayToText(personality.likes);
			dislikesText = arrayToText(personality.dislikes);
			hatesText = arrayToText(personality.hates);
			relationshipsText = arrayToText(personality.relationships);
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
			formData.loves = parseListField(lovesText);
			formData.likes = parseListField(likesText);
			formData.dislikes = parseListField(dislikesText);
			formData.hates = parseListField(hatesText);
			formData.relationships = parseListField(relationshipsText);

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

		<form on:submit|preventDefault={handleSubmit}>
			<div class="form-group">
				<label for="name">Name *</label>
				<input type="text" id="name" bind:value={formData.name} required />
			</div>

			<div class="form-group">
				<label for="description">Description *</label>
				<textarea id="description" bind:value={formData.description} required rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="age">Age</label>
				<input type="text" id="age" bind:value={formData.age} />
			</div>

			<div class="form-group">
				<label for="occupation">Occupation</label>
				<input type="text" id="occupation" bind:value={formData.occupation} />
			</div>

			<div class="form-group">
				<label for="physicalFeatures">Physical Features</label>
				<textarea id="physicalFeatures" bind:value={formData.physicalFeatures} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="typicalClothing">Typical Clothing</label>
				<textarea id="typicalClothing" bind:value={formData.typicalClothing} rows="2"></textarea>
			</div>

			<div class="form-group">
				<label for="background">Background</label>
				<textarea id="background" bind:value={formData.background} rows="4"></textarea>
			</div>

			<div class="form-group">
				<label for="motivations">Motivations</label>
				<textarea id="motivations" bind:value={formData.motivations} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="dreamsForFuture">Dreams for Future</label>
				<textarea id="dreamsForFuture" bind:value={formData.dreamsForFuture} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="whatTheyWillTalkAbout">What They Will Talk About</label>
				<textarea id="whatTheyWillTalkAbout" bind:value={formData.whatTheyWillTalkAbout} rows="3"
				></textarea>
			</div>

			<div class="form-group">
				<label for="loves">Loves (one per line)</label>
				<textarea id="loves" bind:value={lovesText} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="likes">Likes (one per line)</label>
				<textarea id="likes" bind:value={likesText} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="dislikes">Dislikes (one per line)</label>
				<textarea id="dislikes" bind:value={dislikesText} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="hates">Hates (one per line)</label>
				<textarea id="hates" bind:value={hatesText} rows="3"></textarea>
			</div>

			<div class="form-group">
				<label for="relationships">Relationships (one per line)</label>
				<textarea id="relationships" bind:value={relationshipsText} rows="3"></textarea>
			</div>

			{#if error}
				<p class="error">{error}</p>
			{/if}

			<div class="form-actions">
				<button type="submit" class="button primary" disabled={isSubmitting}>
					{isSubmitting ? 'Saving...' : 'Save Changes'}
				</button>
				<a href="/personalities/{$page.params.id}" class="button">Cancel</a>
			</div>
		</form>
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

	.form-group {
		margin-bottom: 1.5rem;
	}

	label {
		display: block;
		margin-bottom: 0.5rem;
		font-weight: bold;
	}

	input[type='text'],
	textarea {
		width: 100%;
		padding: 0.5rem;
		border: 1px solid #ccc;
		border-radius: 4px;
		font-family: inherit;
	}

	textarea {
		resize: vertical;
	}

	.form-actions {
		display: flex;
		gap: 0.5rem;
		margin-top: 2rem;
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

	.button:disabled {
		background-color: #ccc;
		cursor: not-allowed;
	}

	.button.primary {
		background-color: #28a745;
	}

	.button.primary:hover {
		background-color: #218838;
	}

	.error {
		color: red;
		margin-bottom: 1rem;
	}
</style>
