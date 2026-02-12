import type { RenderResult } from '@testing-library/svelte';
import PersonalityForm from '$lib/components/personality/PersonalityForm.svelte';
import { render, fireEvent } from '@testing-library/svelte';
import type { PersonalityInput } from '$lib/api/PersonalityApi';

describe('PersonalityForm', () => {
	let result: RenderResult<PersonalityForm>;

	const mockFormData: PersonalityInput = {
		name: '',
		description: ''
	};

	function renderComponent(overrides: any = {}) {
		if (result) result.unmount();

		let props = {
			formData: { ...mockFormData },
			onSubmit: vi.fn(),
			onCancel: vi.fn(),
			isSubmitting: false,
			error: '',
			submitLabel: 'Submit',
			...overrides
		};

		result = render(PersonalityForm, { props });
		return props;
	}

	it('renders form with required fields', () => {
		renderComponent();

		expect(result.getByTestId('name-input')).toBeInTheDocument();
		expect(result.getByTestId('description-input')).toBeInTheDocument();
		expect(result.getByTestId('submit-button')).toBeInTheDocument();
		expect(result.getByTestId('cancel-button')).toBeInTheDocument();
	});

	it('renders all optional fields', () => {
		renderComponent();

		expect(result.getByTestId('age-input')).toBeInTheDocument();
		expect(result.getByTestId('occupation-input')).toBeInTheDocument();
		expect(result.getByTestId('physicalFeatures-input')).toBeInTheDocument();
		expect(result.getByTestId('loves-input')).toBeInTheDocument();
		expect(result.getByTestId('likes-input')).toBeInTheDocument();
	});

	it('calls onSubmit when form is submitted', async () => {
		const props = renderComponent();

		const form = result.getByTestId('personality-form');
		await fireEvent.submit(form);

		expect(props.onSubmit).toHaveBeenCalled();
	});

	it('calls onCancel when cancel button is clicked', async () => {
		const props = renderComponent();

		const cancelButton = result.getByTestId('cancel-button');
		await fireEvent.click(cancelButton);

		expect(props.onCancel).toHaveBeenCalled();
	});

	it('displays error message when error prop is set', () => {
		renderComponent({ error: 'Test error message' });

		expect(result.getByTestId('form-error')).toHaveTextContent('Test error message');
	});

	it('disables submit button when isSubmitting is true', () => {
		renderComponent({ isSubmitting: true });

		const submitButton = result.getByTestId('submit-button') as HTMLButtonElement;
		expect(submitButton.disabled).toBe(true);
	});

	it('shows submitting text when isSubmitting is true', () => {
		renderComponent({ isSubmitting: true });

		expect(result.getByText('Submitting...')).toBeInTheDocument();
	});

	it('shows custom submit label', () => {
		renderComponent({ submitLabel: 'Create Personality' });

		expect(result.getByText('Create Personality')).toBeInTheDocument();
	});

	it('displays pre-filled values', () => {
		const formData: PersonalityInput = {
			name: 'Test Name',
			description: 'Test Description',
			age: '25'
		};
		renderComponent({ formData });

		const nameInput = result.getByTestId('name-input') as HTMLInputElement;
		const descInput = result.getByTestId('description-input') as HTMLTextAreaElement;
		const ageInput = result.getByTestId('age-input') as HTMLInputElement;

		expect(nameInput.value).toBe('Test Name');
		expect(descInput.value).toBe('Test Description');
		expect(ageInput.value).toBe('25');
	});
});
