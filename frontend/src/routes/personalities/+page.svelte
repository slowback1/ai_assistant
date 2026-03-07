<script lang="ts">
	import PersonalityApi, { type Personality } from '$lib/api/PersonalityApi';
	import PersonalityList from '$lib/components/personality/PersonalityList.svelte';
	import { onMount } from 'svelte';

	let personalities: Personality[] = [];
	let page = 1;
	let pageSize = 10;
	let totalPages = 1;
	let totalCount = 0;
	let nameFilter = '';
	let isLoading = true;
	let error = '';

	const api = new PersonalityApi();

	async function loadPersonalities() {
		isLoading = true;
		error = '';
		try {
			const result = await api.paginate(page, pageSize, nameFilter || undefined);
			personalities = result.items;
			totalPages = result.totalPages;
			totalCount = result.totalCount;
		} catch (e) {
			error = 'Failed to load personalities';
			console.error(e);
		} finally {
			isLoading = false;
		}
	}

	function handleFilter() {
		page = 1; // Reset to first page when filtering
		loadPersonalities();
	}

	function nextPage() {
		if (page < totalPages) {
			page++;
			loadPersonalities();
		}
	}

	function prevPage() {
		if (page > 1) {
			page--;
			loadPersonalities();
		}
	}

	function handleSetActive() {
		loadPersonalities();
	}

	onMount(() => {
		loadPersonalities();
	});
</script>

<svelte:head>
	<title>Personalities</title>
</svelte:head>

<div class="container">
	<h1>Character Personalities</h1>

	<div class="actions">
		<a href="/personalities/create" class="button primary">Create New Personality</a>
	</div>

	<div class="filter-section">
		<input
			type="text"
			bind:value={nameFilter}
			placeholder="Filter by name..."
			on:keyup={(e) => e.key === 'Enter' && handleFilter()}
		/>
		<button on:click={handleFilter} class="button">Filter</button>
	</div>

	{#if isLoading}
		<p>Loading...</p>
	{:else if error}
		<p class="error">{error}</p>
	{:else}
		<PersonalityList
			{personalities}
			{page}
			{pageSize}
			{totalPages}
			{totalCount}
			onNextPage={nextPage}
			onPrevPage={prevPage}
			onSetActive={handleSetActive}
		/>
	{/if}
</div>

<style>
	.container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 2rem;
	}

	h1 {
		margin-bottom: 1rem;
	}

	.actions {
		margin-bottom: 1.5rem;
	}

	.filter-section {
		display: flex;
		gap: 0.5rem;
		margin-bottom: 1.5rem;
	}

	.filter-section input {
		flex: 1;
		padding: 0.5rem;
		border: 1px solid var(--color-border);
		border-radius: 4px;
		background-color: var(--color-card);
		color: var(--color-font);
	}

	.button {
		padding: 0.5rem 1rem;
		background-color: var(--color-button-primary);
		color: var(--color-primary-font);
		border: none;
		border-radius: 4px;
		cursor: pointer;
		text-decoration: none;
		display: inline-block;
	}

	.button:hover {
		background-color: var(--color-button-primary-hover);
	}

	.button.primary {
		background-color: var(--color-button-success);
	}

	.button.primary:hover {
		background-color: var(--color-button-success-hover);
	}

	.error {
		color: var(--color-error);
	}
</style>
