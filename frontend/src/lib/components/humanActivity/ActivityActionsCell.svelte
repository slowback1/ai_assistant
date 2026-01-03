<script lang="ts">
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import type { HumanActivity } from '$lib/api/humanActivity/HumanActivityTypes';

	export let row: HumanActivity;
</script>

<div class="actions">
	<Button href={`/activities/edit/${row.id}`} testId="edit-button" size="small">Edit</Button>
</div>

<style>
	.actions {
		display: flex;
		gap: 0.5rem;
	}
</style>
