import BaseApi from "$lib/api/baseApi";

export default class StoryApi extends BaseApi {
    constructor() {
        super();
    }

    async GetLatestStory(): Promise<StoryEvent | null> {
        try {
            return await this.Get<StoryEvent>("Story/Latest");
        } catch (error) {
            return null;
        }
    }

    async GetAllStories(): Promise<StoryEvent[]> {
        try {
            return await this.Get<StoryEvent[]>("Story/All");
        } catch (error) {
            return [];
        }
    }
}

export interface StoryEvent {
    id: string;
    story: string;
    createdAt: string;
}
