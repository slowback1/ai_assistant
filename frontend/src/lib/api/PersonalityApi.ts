import BaseApi from '$lib/api/baseApi';

export default class PersonalityApi extends BaseApi {
	constructor() {
		super();
	}

	async create(personality: PersonalityInput): Promise<Personality> {
		return await this.Post<Personality>('Personality', personality);
	}

	async getById(id: string): Promise<Personality> {
		return await this.Get<Personality>(`Personality/${id}`);
	}

	async update(id: string, personality: PersonalityInput): Promise<Personality> {
		return await this.Put<Personality>(`Personality/${id}`, personality);
	}

	async paginate(
		page: number = 1,
		pageSize: number = 10,
		nameFilter?: string,
		sortBy: string = 'name',
		sortOrder: string = 'asc'
	): Promise<PaginatedPersonalitiesResult> {
		const queryParams: Record<string, string> = {
			page: page.toString(),
			pageSize: pageSize.toString(),
			sortBy,
			sortOrder
		};

		if (nameFilter) {
			queryParams.nameFilter = nameFilter;
		}

		return await this.Get<PaginatedPersonalitiesResult>('Personality/paginate', queryParams);
	}
}

export interface Personality {
	id: string;
	name: string;
	description: string;
	age?: string;
	physicalFeatures?: string;
	loves?: string[];
	likes?: string[];
	dislikes?: string[];
	hates?: string[];
	relationships?: string[];
	typicalClothing?: string;
	dreamsForFuture?: string;
	whatTheyWillTalkAbout?: string;
	occupation?: string;
	background?: string;
	motivations?: string;
}

export interface PersonalityInput {
	name: string;
	description: string;
	age?: string;
	physicalFeatures?: string;
	loves?: string[];
	likes?: string[];
	dislikes?: string[];
	hates?: string[];
	relationships?: string[];
	typicalClothing?: string;
	dreamsForFuture?: string;
	whatTheyWillTalkAbout?: string;
	occupation?: string;
	background?: string;
	motivations?: string;
}

export interface PaginatedPersonalitiesResult {
	items: Personality[];
	page: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
}
