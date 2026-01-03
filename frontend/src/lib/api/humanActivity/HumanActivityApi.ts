import BaseApi from '$lib/api/baseApi';
import type { HumanActivity, HumanActivityInput } from './HumanActivityTypes';

export default class HumanActivityApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<HumanActivity[]> {
		try {
			return await this.Get<HumanActivity[]>('HumanActivity');
		} catch (error) {
			return [];
		}
	}

	async getById(id: string): Promise<HumanActivity | null> {
		try {
			return await this.Get<HumanActivity>(`HumanActivity/${id}`);
		} catch (error) {
			return null;
		}
	}

	async create(activity: HumanActivityInput): Promise<HumanActivity | null> {
		try {
			return await this.Post<HumanActivity>('HumanActivity', activity);
		} catch (error) {
			return null;
		}
	}

	async update(id: string, activity: HumanActivityInput): Promise<HumanActivity | null> {
		try {
			return await this.Put<HumanActivity>(`HumanActivity/${id}`, activity);
		} catch (error) {
			return null;
		}
	}

	async delete(id: string): Promise<boolean> {
		try {
			await this.Delete(`HumanActivity/${id}`);
			return true;
		} catch (error) {
			return false;
		}
	}
}
