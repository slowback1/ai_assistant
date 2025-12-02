import { describe, it, expect, vi, beforeEach } from 'vitest';
import HumanActivityApi from './HumanActivityApi';
import { ActivityFrequency, type HumanActivity, type HumanActivityInput } from './HumanActivityTypes';

// Mock the fetch function
const mockFetch = vi.fn();
global.fetch = mockFetch;

describe('HumanActivityApi', () => {
	let api: HumanActivityApi;

	beforeEach(() => {
		api = new HumanActivityApi();
		mockFetch.mockReset();
	});

	describe('getAll', () => {
		it('returns activities when fetch succeeds', async () => {
			const activities: HumanActivity[] = [
				{
					id: '1',
					name: 'Activity 1',
					description: 'Desc 1',
					frequency: ActivityFrequency.Daily,
					lastRequested: null
				}
			];
			mockFetch.mockResolvedValueOnce({
				ok: true,
				json: () => Promise.resolve(activities)
			});

			const result = await api.getAll();

			expect(result).toEqual(activities);
		});

		it('returns empty array when fetch fails', async () => {
			mockFetch.mockRejectedValueOnce(new Error('Network error'));

			const result = await api.getAll();

			expect(result).toEqual([]);
		});
	});

	describe('getById', () => {
		it('returns activity when fetch succeeds', async () => {
			const activity: HumanActivity = {
				id: '1',
				name: 'Activity 1',
				description: 'Desc 1',
				frequency: ActivityFrequency.Daily,
				lastRequested: null
			};
			mockFetch.mockResolvedValueOnce({
				ok: true,
				json: () => Promise.resolve(activity)
			});

			const result = await api.getById('1');

			expect(result).toEqual(activity);
		});

		it('returns null when fetch fails', async () => {
			mockFetch.mockRejectedValueOnce(new Error('Not found'));

			const result = await api.getById('1');

			expect(result).toBeNull();
		});
	});

	describe('create', () => {
		it('returns created activity when fetch succeeds', async () => {
			const newActivity: HumanActivityInput = {
				name: 'New Activity',
				description: 'New Desc',
				frequency: ActivityFrequency.Weekly
			};
			const createdActivity: HumanActivity = {
				id: '1',
				...newActivity,
				lastRequested: null
			};
			mockFetch.mockResolvedValueOnce({
				ok: true,
				json: () => Promise.resolve(createdActivity)
			});

			const result = await api.create(newActivity);

			expect(result).toEqual(createdActivity);
		});

		it('returns null when fetch fails', async () => {
			mockFetch.mockRejectedValueOnce(new Error('Error'));

			const result = await api.create({
				name: 'Test',
				description: '',
				frequency: ActivityFrequency.Daily
			});

			expect(result).toBeNull();
		});
	});

	describe('update', () => {
		it('returns updated activity when fetch succeeds', async () => {
			const updatedActivity: HumanActivity = {
				id: '1',
				name: 'Updated',
				description: 'Updated Desc',
				frequency: ActivityFrequency.Hourly,
				lastRequested: null
			};
			mockFetch.mockResolvedValueOnce({
				ok: true,
				json: () => Promise.resolve(updatedActivity)
			});

			const result = await api.update('1', {
				name: 'Updated',
				description: 'Updated Desc',
				frequency: ActivityFrequency.Hourly
			});

			expect(result).toEqual(updatedActivity);
		});
	});

	describe('delete', () => {
		it('returns true when delete succeeds', async () => {
			mockFetch.mockResolvedValueOnce({
				ok: true,
				json: () => Promise.resolve(true)
			});

			const result = await api.delete('1');

			expect(result).toBe(true);
		});

		it('returns false when delete fails', async () => {
			mockFetch.mockRejectedValueOnce(new Error('Error'));

			const result = await api.delete('1');

			expect(result).toBe(false);
		});
	});
});
