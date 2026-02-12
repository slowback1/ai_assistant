<script lang="ts">
	import PersonalityApi, { type Personality } from '$lib/api/PersonalityApi';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';

	let personality: Personality | null = null;
	let isLoading = true;
	let error = '';

	const api = new PersonalityApi();

	async function loadPersonality() {
		const id = $page.params.id;
		isLoading = true;
		error = '';
		try {
			personality = await api.getById(id);
		} catch (e) {
			error = 'Failed to load personality';
			console.error(e);
		} finally {
			isLoading = false;
		}
	}

	onMount(() => {
		loadPersonality();
	});
</script>

<svelte:head>
	<title>{personality?.name || 'Personality'}</title>
</svelte:head>

<div class="container">
	{#if isLoading}
		<p>Loading...</p>
	{:else if error}
		<p class="error">{error}</p>
		<a href="/personalities" class="button">Back to List</a>
	{:else if personality}
		<div class="header">
			<h1>{personality.name}</h1>
			<div class="actions">
				<a href="/personalities" class="button">Back to List</a>
				<a href="/personalities/{personality.id}/edit" class="button primary">Edit</a>
			</div>
		</div>

		<div class="personality-details">
			<section>
				<h2>Description</h2>
				<p>{personality.description}</p>
			</section>

			{#if personality.age}
				<section>
					<h2>Age</h2>
					<p>{personality.age}</p>
				</section>
			{/if}

			{#if personality.occupation}
				<section>
					<h2>Occupation</h2>
					<p>{personality.occupation}</p>
				</section>
			{/if}

			{#if personality.physicalFeatures}
				<section>
					<h2>Physical Features</h2>
					<p>{personality.physicalFeatures}</p>
				</section>
			{/if}

			{#if personality.typicalClothing}
				<section>
					<h2>Typical Clothing</h2>
					<p>{personality.typicalClothing}</p>
				</section>
			{/if}

			{#if personality.background}
				<section>
					<h2>Background</h2>
					<p>{personality.background}</p>
				</section>
			{/if}

			{#if personality.motivations}
				<section>
					<h2>Motivations</h2>
					<p>{personality.motivations}</p>
				</section>
			{/if}

			{#if personality.dreamsForFuture}
				<section>
					<h2>Dreams for Future</h2>
					<p>{personality.dreamsForFuture}</p>
				</section>
			{/if}

			{#if personality.whatTheyWillTalkAbout}
				<section>
					<h2>What They Will Talk About</h2>
					<p>{personality.whatTheyWillTalkAbout}</p>
				</section>
			{/if}

			{#if personality.loves && personality.loves.length > 0}
				<section>
					<h2>Loves</h2>
					<ul>
						{#each personality.loves as item}
							<li>{item}</li>
						{/each}
					</ul>
				</section>
			{/if}

			{#if personality.likes && personality.likes.length > 0}
				<section>
					<h2>Likes</h2>
					<ul>
						{#each personality.likes as item}
							<li>{item}</li>
						{/each}
					</ul>
				</section>
			{/if}

			{#if personality.dislikes && personality.dislikes.length > 0}
				<section>
					<h2>Dislikes</h2>
					<ul>
						{#each personality.dislikes as item}
							<li>{item}</li>
						{/each}
					</ul>
				</section>
			{/if}

			{#if personality.hates && personality.hates.length > 0}
				<section>
					<h2>Hates</h2>
					<ul>
						{#each personality.hates as item}
							<li>{item}</li>
						{/each}
					</ul>
				</section>
			{/if}

			{#if personality.relationships && personality.relationships.length > 0}
				<section>
					<h2>Relationships</h2>
					<ul>
						{#each personality.relationships as item}
							<li>{item}</li>
						{/each}
					</ul>
				</section>
			{/if}
		</div>
	{/if}
</div>

<style>
	.container {
		max-width: 900px;
		margin: 0 auto;
		padding: 2rem;
	}

	.header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 2rem;
	}

	h1 {
		margin: 0;
	}

	.actions {
		display: flex;
		gap: 0.5rem;
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

	.button.primary {
		background-color: #28a745;
	}

	.button.primary:hover {
		background-color: #218838;
	}

	.personality-details section {
		margin-bottom: 1.5rem;
		padding: 1rem;
		background-color: #f9f9f9;
		border-radius: 8px;
	}

	.personality-details h2 {
		margin-top: 0;
		margin-bottom: 0.5rem;
		font-size: 1.2rem;
		color: #333;
	}

	.personality-details p {
		margin: 0;
		line-height: 1.6;
	}

	.personality-details ul {
		margin: 0;
		padding-left: 1.5rem;
	}

	.personality-details li {
		margin-bottom: 0.25rem;
	}

	.error {
		color: red;
	}
</style>
