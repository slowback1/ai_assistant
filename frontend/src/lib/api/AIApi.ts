import BaseApi from "$lib/api/baseApi";

export default class AIApi extends BaseApi {
    constructor() {
        super();
    }

    async ChatWithDarthVader(message: string, session? : AISession) : Promise<AISession> {
        let request = {
            message: message,
            session: session
        }

        return await this.Post("ChatWithDarthVader", request);
    }
}

export interface AISession {
    sessionId: string;
    messages: AIMessage[];
}

export interface AIMessage {
    role: "user" | "bot";
    message: string;
}