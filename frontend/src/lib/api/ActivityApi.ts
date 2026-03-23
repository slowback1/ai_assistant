import BaseApi from '$lib/api/baseApi';

export interface Activity {
	id: string;
	name: string;
}

export default class ActivityApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<Activity[]> {
		try {
			return await this.Get<Activity[]>('Activity');
		} catch {
			return [];
		}
	}

	async create(name: string): Promise<Activity> {
		return await this.Post<Activity>('Activity', { name });
	}

	async delete(id: string): Promise<void> {
		await this.Delete<void>(`Activity/${id}`);
	}

	async getCurrent(): Promise<Activity | null> {
		try {
			return await this.Get<Activity | null>('Activity/Current');
		} catch {
			return null;
		}
	}

	async pickNow(): Promise<Activity | null> {
		try {
			return await this.Post<Activity | null>('Activity/PickNow', {});
		} catch {
			return null;
		}
	}
}
