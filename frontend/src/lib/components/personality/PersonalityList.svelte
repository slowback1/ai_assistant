<script lang="ts">
	import type { Personality } from '$lib/api/PersonalityApi';

	export let personalities: Personality[] = [];
	export let page: number = 1;
	export let pageSize: number = 10;
	export let totalPages: number = 1;
	export let totalCount: number = 0;
	export let onNextPage: () => void = () => {};
	export let onPrevPage: () => void = () => {};
</script>

<div class="personality-list" data-testid="personality-list">
	{#if personalities.length === 0}
		<p>No personalities found. <a href="/personalities/create">Create one</a> to get started.</p>
	{:else}
		{#each personalities as personality}
			<div class="personality-card" data-testid="personality-card">
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
	<div class="pagination" data-testid="pagination">
		<button on:click={onPrevPage} disabled={page === 1} class="button">Previous</button>
		<span>
			Page {page} of {totalPages} ({totalCount} total)
		</span>
		<button on:click={onNextPage} disabled={page === totalPages} class="button">Next</button>
	</div>
{/if}

<style>
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

	.pagination {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 1rem;
	}
</style>
