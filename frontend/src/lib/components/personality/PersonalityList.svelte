<script lang="ts">
	import type { Personality } from '$lib/api/PersonalityApi';
	import PersonalityApi from '$lib/api/PersonalityApi';
	import StoryApi from '$lib/api/StoryApi';

	export let personalities: Personality[] = [];
	export let page: number = 1;
	export let pageSize: number = 10;
	export let totalPages: number = 1;
	export let totalCount: number = 0;
	export let onNextPage: () => void = () => {};
	export let onPrevPage: () => void = () => {};
	export let onSetActive: (personality: Personality) => void = () => {};

	const personalityApi = new PersonalityApi();
	const storyApi = new StoryApi();

	async function handleSetActive(personality: Personality) {
		try {
			await personalityApi.setActive(personality.id);
			await storyApi.generate();
			onSetActive(personality);
		} catch (error) {
			console.error('Failed to set active personality:', error);
		}
	}
</script>

<div class="personality-list" data-testid="personality-list">
	{#if personalities.length === 0}
		<p>No personalities found. <a href="/personalities/create">Create one</a> to get started.</p>
	{:else}
		{#each personalities as personality}
			<div class="personality-card" data-testid="personality-card" class:active={personality.isActive}>
				<h3>
					{personality.name}
					{#if personality.isActive}
						<span class="active-badge">Active</span>
					{/if}
				</h3>
				<p>{personality.description}</p>
				<div class="card-actions">
					<a href="/personalities/{personality.id}" class="button">View</a>
					<a href="/personalities/{personality.id}/edit" class="button">Edit</a>
					{#if personality.isActive}
						<button class="button active-btn" disabled>Active</button>
					{:else}
						<button class="button primary" on:click={() => handleSetActive(personality)}>Set Active</button>
					{/if}
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
		border: 1px solid var(--color-border);
		border-radius: 8px;
		padding: 1.5rem;
		background-color: var(--color-card);
	}

	.personality-card.active {
		border-color: var(--color-button-success);
		background-color: var(--color-card);
	}

	.personality-card h3 {
		margin-top: 0;
		margin-bottom: 0.5rem;
		display: flex;
		align-items: center;
		gap: 0.5rem;
	}

	.active-badge {
		font-size: 0.75rem;
		padding: 0.25rem 0.5rem;
		background-color: var(--color-button-success);
		color: var(--color-primary-font);
		border-radius: 4px;
	}

	.personality-card p {
		margin-bottom: 1rem;
		color: var(--color-muted);
	}

	.card-actions {
		display: flex;
		gap: 0.5rem;
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

	.button:disabled {
		background-color: var(--color-muted);
		cursor: not-allowed;
	}

	.button.primary {
		background-color: var(--color-button-success);
	}

	.button.primary:hover {
		background-color: var(--color-button-success-hover);
	}

	.button.active-btn {
		background-color: var(--color-muted);
		cursor: default;
	}

	.pagination {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 1rem;
	}
</style>
