import BaseApi from '$lib/api/baseApi';

export interface WeatherData {
	name: string;
	tempF: number;
	windMph: number;
	precipIn: number;
	heatindexF: number;
	condition: string;
	conditionIcon: string;
}

export default class WeatherApi extends BaseApi {
	constructor() {
		super();
	}

	async GetCurrentWeather(): Promise<WeatherData | null> {
		try {
			return await this.Get<WeatherData>('Weather/Current');
		} catch (error) {
			console.error('Failed to fetch weather data:', error);
			return null;
		}
	}
}
