<script lang="ts">
	import ActivityApi, { type Activity } from '$lib/api/ActivityApi';
	import { onMount } from 'svelte';

	let activities: Activity[] = [];
	let isLoading = true;
	let error = '';
	let newActivityName = '';
	let isSubmitting = false;
	let submitError = '';

	const api = new ActivityApi();

	async function loadActivities() {
		isLoading = true;
		error = '';
		try {
			activities = await api.getAll();
		} catch (e) {
			error = 'Failed to load activities.';
			console.error(e);
		} finally {
			isLoading = false;
		}
	}

	async function addActivity() {
		if (!newActivityName.trim()) return;
		isSubmitting = true;
		submitError = '';
		try {
			await api.create(newActivityName.trim());
			newActivityName = '';
			await loadActivities();
		} catch (e) {
			submitError = 'Failed to add activity.';
			console.error(e);
		} finally {
			isSubmitting = false;
		}
	}

	async function deleteActivity(id: string) {
		try {
			await api.delete(id);
			await loadActivities();
		} catch (e) {
			error = 'Failed to delete activity.';
			console.error(e);
		}
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Enter') addActivity();
	}

	onMount(() => {
		loadActivities();
	});
</script>

<svelte:head>
	<title>Activities</title>
</svelte:head>

<div class="container">
	<h1>Activities</h1>
	<p class="description">
		Define the list of activities for the hourly picker. Each activity will be picked once before
		any can repeat (7-bag rule).
	</p>

	<div class="add-section">
		<input
			type="text"
			bind:value={newActivityName}
			placeholder="Activity name..."
			on:keydown={handleKeydown}
			disabled={isSubmitting}
		/>
		<button on:click={addActivity} class="button primary" disabled={isSubmitting || !newActivityName.trim()}>
			{isSubmitting ? 'Adding...' : 'Add Activity'}
		</button>
	</div>

	{#if submitError}
		<p class="error">{submitError}</p>
	{/if}

	{#if isLoading}
		<p>Loading...</p>
	{:else if error}
		<p class="error">{error}</p>
	{:else if activities.length === 0}
		<div class="empty-state">
			<p>No activities defined yet.</p>
			<p class="hint">Add some activities above to get started.</p>
		</div>
	{:else}
		<ul class="activity-list">
			{#each activities as activity (activity.id)}
				<li class="activity-item">
					<span class="activity-name">{activity.name}</span>
					<button
						class="button danger"
						on:click={() => deleteActivity(activity.id)}
						aria-label="Delete {activity.name}"
					>
						Delete
					</button>
				</li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.container {
		max-width: 800px;
		margin: 0 auto;
		padding: 2rem;
	}

	h1 {
		margin-bottom: 0.5rem;
	}

	.description {
		color: color-mix(in lab, var(--color-font) 70%, transparent);
		margin-bottom: 1.5rem;
	}

	.add-section {
		display: flex;
		gap: 0.5rem;
		margin-bottom: 0.5rem;
	}

	.add-section input {
		flex: 1;
		padding: 0.5rem;
		border: 1px solid var(--color-border);
		border-radius: 4px;
		background-color: var(--color-card);
		color: var(--color-font);
		font-size: 1rem;
	}

	.activity-list {
		list-style: none;
		padding: 0;
		margin: 1.5rem 0 0;
		display: flex;
		flex-direction: column;
		gap: 0.5rem;
	}

	.activity-item {
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: 0.75rem 1rem;
		background: var(--color-card);
		border-radius: 6px;
		box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
	}

	.activity-name {
		font-size: 1rem;
		font-weight: 500;
	}

	.empty-state {
		text-align: center;
		padding: 3rem 2rem;
		color: var(--color-font);
	}

	.hint {
		font-size: 0.9rem;
		color: #999;
		margin-top: 0.5rem;
	}

	.button {
		padding: 0.5rem 1rem;
		border: none;
		border-radius: 4px;
		cursor: pointer;
		font-size: 0.9rem;
	}

	.button:disabled {
		opacity: 0.5;
		cursor: not-allowed;
	}

	.button.primary {
		background-color: var(--color-button-success);
		color: var(--color-primary-font);
	}

	.button.primary:hover:not(:disabled) {
		background-color: var(--color-button-success-hover);
	}

	.button.danger {
		background-color: var(--color-base-red);
		color: #fff;
	}

	.button.danger:hover {
		opacity: 0.85;
	}

	.error {
		color: var(--color-base-red);
	}
</style>
