import BaseApi from '$lib/api/baseApi';

export interface WeatherData {
	name: string;
	temp_f: number;
	wind_mph: number;
	precip_in: number;
	heatindex_f: number;
	condition: string;
	condition_icon: string;
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
