import type { RenderResult } from '@testing-library/svelte';
import PersonalityList from '$lib/components/personality/PersonalityList.svelte';
import { render, fireEvent } from '@testing-library/svelte';
import type { Personality } from '$lib/api/PersonalityApi';

describe('PersonalityList', () => {
	let result: RenderResult<PersonalityList>;

	const mockPersonalities: Personality[] = [
		{
			id: '1',
			name: 'Test Character 1',
			description: 'Description 1'
		},
		{
			id: '2',
			name: 'Test Character 2',
			description: 'Description 2'
		}
	];

	function renderComponent(overrides: any = {}) {
		if (result) result.unmount();

		let props = {
			personalities: mockPersonalities,
			page: 1,
			pageSize: 10,
			totalPages: 1,
			totalCount: 2,
			onNextPage: vi.fn(),
			onPrevPage: vi.fn(),
			...overrides
		};

		result = render(PersonalityList, { props });
		return props;
	}

	it('renders the list of personalities', () => {
		renderComponent();

		const cards = result.getAllByTestId('personality-card');
		expect(cards).toHaveLength(2);
		expect(result.getByText('Test Character 1')).toBeInTheDocument();
		expect(result.getByText('Test Character 2')).toBeInTheDocument();
	});

	it('displays empty state when no personalities', () => {
		renderComponent({ personalities: [] });

		expect(result.getByText(/No personalities found/)).toBeInTheDocument();
		expect(result.getByText('Create one')).toBeInTheDocument();
	});

	it('shows pagination when totalPages > 1', () => {
		renderComponent({ totalPages: 3 });

		expect(result.getByTestId('pagination')).toBeInTheDocument();
		expect(result.getByText(/Page 1 of 3/)).toBeInTheDocument();
	});

	it('hides pagination when totalPages = 1', () => {
		renderComponent({ totalPages: 1 });

		expect(result.queryByTestId('pagination')).not.toBeInTheDocument();
	});

	it('calls onPrevPage when Previous button clicked', async () => {
		const props = renderComponent({ page: 2, totalPages: 3 });

		const prevButton = result.getByText('Previous');
		await fireEvent.click(prevButton);

		expect(props.onPrevPage).toHaveBeenCalled();
	});

	it('calls onNextPage when Next button clicked', async () => {
		const props = renderComponent({ page: 1, totalPages: 3 });

		const nextButton = result.getByText('Next');
		await fireEvent.click(nextButton);

		expect(props.onNextPage).toHaveBeenCalled();
	});

	it('disables Previous button on first page', () => {
		renderComponent({ page: 1, totalPages: 3 });

		const prevButton = result.getByText('Previous') as HTMLButtonElement;
		expect(prevButton.disabled).toBe(true);
	});

	it('disables Next button on last page', () => {
		renderComponent({ page: 3, totalPages: 3 });

		const nextButton = result.getByText('Next') as HTMLButtonElement;
		expect(nextButton.disabled).toBe(true);
	});

	it('displays view and edit links for each personality', () => {
		renderComponent();

		const viewLinks = result.getAllByText('View');
		const editLinks = result.getAllByText('Edit');

		expect(viewLinks).toHaveLength(2);
		expect(editLinks).toHaveLength(2);
		expect(viewLinks[0]).toHaveAttribute('href', '/personalities/1');
		expect(editLinks[0]).toHaveAttribute('href', '/personalities/1/edit');
	});
});
