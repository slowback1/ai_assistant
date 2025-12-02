<script lang="ts">
	import type { HumanActivity } from '$lib/api/humanActivity/HumanActivityTypes';
	import { ActivityFrequencyLabels, ActivityFrequency } from '$lib/api/humanActivity/HumanActivityTypes';

	export let row: HumanActivity;
</script>

<span data-testid="frequency-cell">{ActivityFrequencyLabels[row.frequency as ActivityFrequency] || 'Unknown'}</span>
