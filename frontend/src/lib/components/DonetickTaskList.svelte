<script lang="ts">
	import DonetickApi, { type Chore } from '$lib/api/DonetickApi';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import { onMount, onDestroy } from 'svelte';

	let chores: Chore[] = [];
	let loading = true;
	let error = '';
	let pollInterval: number;
	let completingChoreIds: Set<number> = new Set();

	const api = new DonetickApi();
	const toastService = new ToastService();

	async function fetchOverdueChores() {
		try {
			loading = true;
			error = '';
			const fetchedChores = await api.GetOverdueChores();
			chores = fetchedChores;
		} catch (err) {
			error = 'Failed to fetch overdue chores. Please try again later.';
			console.error('Error fetching chores:', err);
		} finally {
			loading = false;
		}
	}

	async function completeChore(choreId: number) {
		// Prevent multiple clicks
		if (completingChoreIds.has(choreId)) return;

		completingChoreIds.add(choreId);
		completingChoreIds = completingChoreIds; // Trigger reactivity

		try {
			const result = await api.CompleteChore(choreId);

			if (result.success) {
				toastService.AddToast({
					message: 'Chore completed successfully!',
					variant: ToastVariant.success
				});
				// Remove the completed chore from the list
				chores = chores.filter((c) => c.id !== choreId);
			} else {
				toastService.AddToast({
					message: result.message || 'Failed to complete chore',
					variant: ToastVariant.error
				});
			}
		} catch (err) {
			toastService.AddToast({
				message: 'Failed to complete chore',
				variant: ToastVariant.error
			});
			console.error('Error completing chore:', err);
		} finally {
			completingChoreIds.delete(choreId);
			completingChoreIds = completingChoreIds; // Trigger reactivity
		}
	}

	onMount(() => {
		fetchOverdueChores();
		// Poll for new chores every 5 minutes
		pollInterval = setInterval(fetchOverdueChores, 300000);
	});

	onDestroy(() => {
		if (pollInterval) {
			clearInterval(pollInterval);
		}
	});
</script>

<div class="donetick-section">
	<h2>To-Do Tasks</h2>

	{#if loading && chores.length === 0}
		<div class="loading">Loading tasks...</div>
	{:else if error}
		<div class="error">{error}</div>
	{:else if chores.length === 0}
		<div class="no-chores">
			<p>No overdue tasks! 🎉</p>
		</div>
	{:else}
		<div class="chores-list">
			{#each chores as chore (chore.id)}
				<div class="chore-item">
					<div class="chore-info">
						<span class="chore-name">{chore.name}</span>
					</div>
					<button
						class="complete-btn"
						on:click={() => completeChore(chore.id)}
						disabled={completingChoreIds.has(chore.id)}
					>
						{completingChoreIds.has(chore.id) ? 'Completing...' : 'Complete'}
					</button>
				</div>
			{/each}
		</div>
	{/if}
</div>

<style>
	.donetick-section {
		background: white;
		border-radius: 8px;
		padding: 2rem;
		box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
	}

	.donetick-section h2 {
		margin-top: 0;
		color: #333;
		border-bottom: 2px solid #4caf50;
		padding-bottom: 0.5rem;
	}

	.loading,
	.error,
	.no-chores {
		text-align: center;
		padding: 2rem;
		font-size: 1.1rem;
	}

	.loading {
		color: #666;
	}

	.error {
		color: #e63946;
	}

	.no-chores {
		color: #666;
	}

	.chores-list {
		display: flex;
		flex-direction: column;
		gap: 1rem;
		margin-top: 1rem;
	}

	.chore-item {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 1rem;
		background: #f8f9fa;
		border-left: 4px solid #4caf50;
		border-radius: 4px;
		transition: transform 0.2s;
	}

	.chore-item:hover {
		transform: translateX(4px);
	}

	.chore-info {
		flex: 1;
	}

	.chore-name {
		font-size: 1.1rem;
		color: #333;
		font-weight: 500;
	}

	.complete-btn {
		background: #4caf50;
		color: white;
		border: none;
		padding: 0.5rem 1.5rem;
		border-radius: 4px;
		cursor: pointer;
		font-size: 1rem;
		font-weight: 500;
		transition:
			background 0.2s,
			transform 0.1s;
	}

	.complete-btn:hover:not(:disabled) {
		background: #45a049;
		transform: scale(1.05);
	}

	.complete-btn:active:not(:disabled) {
		transform: scale(0.98);
	}

	.complete-btn:disabled {
		background: #cccccc;
		cursor: not-allowed;
	}
</style>
