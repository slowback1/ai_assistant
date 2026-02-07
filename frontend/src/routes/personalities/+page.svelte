<script lang="ts">
	import PersonalityApi, { type Personality } from '$lib/api/PersonalityApi';
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
		<div class="personality-list">
			{#if personalities.length === 0}
				<p>No personalities found. <a href="/personalities/create">Create one</a> to get started.</p>
			{:else}
				{#each personalities as personality}
					<div class="personality-card">
						<h3>{personality.name}</h3>
						<p>{personality.description}</p>
						<div class="card-actions">
							<a href="/personalities/{personality.id}" class="button">View</a>
							<a href="/personalities/{personality.id}/edit" class="button">Edit</a>
						</div>
					</div>
				{/each}
			{/if}
		</div>

		{#if totalPages > 1}
			<div class="pagination">
				<button on:click={prevPage} disabled={page === 1} class="button">Previous</button>
				<span>
					Page {page} of {totalPages} ({totalCount} total)
				</span>
				<button on:click={nextPage} disabled={page === totalPages} class="button">Next</button>
			</div>
		{/if}
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
		border: 1px solid #ccc;
		border-radius: 4px;
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

	.personality-list {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
		gap: 1rem;
		margin-bottom: 2rem;
	}

	.personality-card {
		border: 1px solid #ddd;
		border-radius: 8px;
		padding: 1.5rem;
		background-color: #f9f9f9;
	}

	.personality-card h3 {
		margin-top: 0;
		margin-bottom: 0.5rem;
	}

	.personality-card p {
		margin-bottom: 1rem;
		color: #666;
	}

	.card-actions {
		display: flex;
		gap: 0.5rem;
	}

	.pagination {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 1rem;
	}

	.error {
		color: red;
	}
</style>
