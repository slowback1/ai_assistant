import BaseApi from '$lib/api/baseApi';

export interface Chore {
	id: number;
	name: string;
	nextDueDate: string | null;
}

export interface CompleteChoreResponse {
	success: boolean;
	message: string;
}

export default class DonetickApi extends BaseApi {
	constructor() {
		super();
	}

	async GetOverdueChores(): Promise<Chore[]> {
		try {
			return await this.Get<Chore[]>('Donetick/OverdueChores');
		} catch (error) {
			console.error('Failed to fetch overdue chores:', error);
			return [];
		}
	}

	async CompleteChore(choreId: number): Promise<CompleteChoreResponse> {
		try {
			return await this.Post<CompleteChoreResponse>(`Donetick/CompleteChore/${choreId}`, {});
		} catch (error) {
			console.error('Failed to complete chore:', error);
			return { success: false, message: 'Failed to complete chore' };
		}
	}
}
