export interface HumanActivity {
	id: string;
	name: string;
	description: string;
	frequency: ActivityFrequency;
	lastRequested: string | null;
}

export type HumanActivityInput = Omit<HumanActivity, 'id' | 'lastRequested'>;

export enum ActivityFrequency {
	Hourly = 0,
	Daily = 1,
	EveryFewDays = 2,
	Weekly = 3
}

export const ActivityFrequencyLabels: Record<ActivityFrequency, string> = {
	[ActivityFrequency.Hourly]: 'Hourly',
	[ActivityFrequency.Daily]: 'Daily',
	[ActivityFrequency.EveryFewDays]: 'Every Few Days',
	[ActivityFrequency.Weekly]: 'Weekly'
};
