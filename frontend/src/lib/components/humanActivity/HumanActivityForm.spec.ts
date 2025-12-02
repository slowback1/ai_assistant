import { render, fireEvent, waitFor } from '@testing-library/svelte';
import { describe, it, expect, vi } from 'vitest';
import HumanActivityForm from './HumanActivityForm.svelte';
import { ActivityFrequency } from '$lib/api/humanActivity/HumanActivityTypes';
import type { HumanActivityInput } from '$lib/api/humanActivity/HumanActivityTypes';

describe('HumanActivityForm', () => {
	it('renders the form with default values', () => {
		const { getByTestId } = render(HumanActivityForm);

		expect(getByTestId('activity-form')).toBeInTheDocument();
		expect(getByTestId('activity-name')).toBeInTheDocument();
		expect(getByTestId('activity-description')).toBeInTheDocument();
		expect(getByTestId('activity-frequency')).toBeInTheDocument();
	});

	it('renders with provided activity values', () => {
		const activity: HumanActivityInput = {
			name: 'Test Activity',
			description: 'Test Description',
			frequency: ActivityFrequency.Weekly
		};

		const { getByTestId } = render(HumanActivityForm, { props: { activity } });

		const nameInput = getByTestId('activity-name') as HTMLInputElement;
		const descriptionInput = getByTestId('activity-description') as HTMLInputElement;

		expect(nameInput.value).toBe('Test Activity');
		expect(descriptionInput.value).toBe('Test Description');
	});

	it('calls onSubmit with form data when submitted', async () => {
		const onSubmit = vi.fn();
		const activity: HumanActivityInput = {
			name: 'Test',
			description: 'Desc',
			frequency: ActivityFrequency.Daily
		};

		const { getByTestId } = render(HumanActivityForm, {
			props: { activity, onSubmit }
		});

		const form = getByTestId('activity-form');
		await fireEvent.submit(form);

		expect(onSubmit).toHaveBeenCalledWith(activity);
	});

	it('calls onCancel when cancel button is clicked', async () => {
		const onCancel = vi.fn();

		const { getByTestId } = render(HumanActivityForm, {
			props: { onCancel }
		});

		const cancelButton = getByTestId('cancel-button');
		await fireEvent.click(cancelButton);

		expect(onCancel).toHaveBeenCalled();
	});

	it('displays custom submit label', () => {
		const { getByTestId } = render(HumanActivityForm, {
			props: { submitLabel: 'Create' }
		});

		expect(getByTestId('submit-button').textContent).toContain('Create');
	});

	it('shows saving text when isSubmitting is true', () => {
		const { getByTestId } = render(HumanActivityForm, {
			props: { isSubmitting: true }
		});

		expect(getByTestId('submit-button').textContent).toContain('Saving...');
	});
});
