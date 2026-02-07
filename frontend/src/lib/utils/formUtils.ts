export function cleanupOptionalFields<T extends Record<string, any>>(
	data: T,
	fields: (keyof T)[]
): void {
	for (const field of fields) {
		if (typeof data[field] === 'string' && !data[field]?.trim()) {
			data[field] = undefined as any;
		}
	}
}
