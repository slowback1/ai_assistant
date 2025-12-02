<script lang="ts">
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import {
		type HumanActivityInput,
		ActivityFrequency,
		ActivityFrequencyLabels
	} from '$lib/api/humanActivity/HumanActivityTypes';
	import type { SelectOption } from '$lib/ui/inputs/Select/SelectTypes';

	export let activity: HumanActivityInput = {
		name: '',
		description: '',
		frequency: ActivityFrequency.Daily
	};
	export let onSubmit: (activity: HumanActivityInput) => void = () => {};
	export let onCancel: () => void = () => {};
	export let submitLabel = 'Save';
	export let isSubmitting = false;

	const frequencyOptions: SelectOption[] = Object.entries(ActivityFrequencyLabels).map(
		([value, label]) => ({
			value: parseInt(value),
			label
		})
	);

	function handleSubmit(event: Event) {
		event.preventDefault();
		onSubmit(activity);
	}

	function handleFrequencyChange(value: string) {
		const parsedValue = parseInt(value);
		if (parsedValue in ActivityFrequency) {
			activity.frequency = parsedValue as ActivityFrequency;
		}
	}
</script>

<form on:submit={handleSubmit} class="activity-form" data-testid="activity-form">
	<div class="form-group">
		<TextBox
			id="activity-name"
			label="Name"
			bind:value={activity.name}
			required
		/>
	</div>

	<div class="form-group">
		<TextBox
			id="activity-description"
			label="Description"
			bind:value={activity.description}
		/>
	</div>

	<div class="form-group">
		<Select
			id="activity-frequency"
			label="Frequency"
			options={frequencyOptions}
			value={activity.frequency}
			onChange={handleFrequencyChange}
			data-testid="activity-frequency"
		/>
	</div>

	<div class="form-actions">
		<Button testId="cancel-button" variant="secondary" onClick={onCancel} disabled={isSubmitting}>
			Cancel
		</Button>
		<Button testId="submit-button" variant="primary" disabled={isSubmitting}>
			{isSubmitting ? 'Saving...' : submitLabel}
		</Button>
	</div>
</form>

<style>
	.activity-form {
		display: flex;
		flex-direction: column;
		gap: 1rem;
		max-width: 500px;
	}

	.form-group {
		display: flex;
		flex-direction: column;
		gap: 0.25rem;
	}

	.form-actions {
		display: flex;
		gap: 1rem;
		margin-top: 1rem;
	}
</style>
