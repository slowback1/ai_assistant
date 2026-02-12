<script lang="ts">
	import type { PersonalityInput } from '$lib/api/PersonalityApi';

	export let formData: PersonalityInput;
	export let onSubmit: () => void = () => {};
	export let onCancel: () => void = () => {};
	export let isSubmitting: boolean = false;
	export let error: string = '';
	export let submitLabel: string = 'Submit';

	let lovesText = '';
	let likesText = '';
	let dislikesText = '';
	let hatesText = '';
	let relationshipsText = '';

	// Initialize text fields from formData if they exist
	$: if (formData.loves) lovesText = formData.loves.join('\n');
	$: if (formData.likes) likesText = formData.likes.join('\n');
	$: if (formData.dislikes) dislikesText = formData.dislikes.join('\n');
	$: if (formData.hates) hatesText = formData.hates.join('\n');
	$: if (formData.relationships) relationshipsText = formData.relationships.join('\n');

	function handleSubmit() {
		// Parse list fields
		formData.loves = parseListField(lovesText);
		formData.likes = parseListField(likesText);
		formData.dislikes = parseListField(dislikesText);
		formData.hates = parseListField(hatesText);
		formData.relationships = parseListField(relationshipsText);

		onSubmit();
	}

	function parseListField(text: string): string[] | undefined {
		if (!text.trim()) return undefined;
		return text
			.split('\n')
			.map((line) => line.trim())
			.filter((line) => line.length > 0);
	}
</script>

<form on:submit|preventDefault={handleSubmit} data-testid="personality-form">
	<div class="form-group">
		<label for="name">Name *</label>
		<input type="text" id="name" bind:value={formData.name} required data-testid="name-input" />
	</div>

	<div class="form-group">
		<label for="description">Description *</label>
		<textarea
			id="description"
			bind:value={formData.description}
			required
			rows="3"
			data-testid="description-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="age">Age</label>
		<input type="text" id="age" bind:value={formData.age} data-testid="age-input" />
	</div>

	<div class="form-group">
		<label for="occupation">Occupation</label>
		<input
			type="text"
			id="occupation"
			bind:value={formData.occupation}
			data-testid="occupation-input"
		/>
	</div>

	<div class="form-group">
		<label for="physicalFeatures">Physical Features</label>
		<textarea
			id="physicalFeatures"
			bind:value={formData.physicalFeatures}
			rows="3"
			data-testid="physicalFeatures-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="typicalClothing">Typical Clothing</label>
		<textarea
			id="typicalClothing"
			bind:value={formData.typicalClothing}
			rows="2"
			data-testid="typicalClothing-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="background">Background</label>
		<textarea
			id="background"
			bind:value={formData.background}
			rows="4"
			data-testid="background-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="motivations">Motivations</label>
		<textarea
			id="motivations"
			bind:value={formData.motivations}
			rows="3"
			data-testid="motivations-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="dreamsForFuture">Dreams for Future</label>
		<textarea
			id="dreamsForFuture"
			bind:value={formData.dreamsForFuture}
			rows="3"
			data-testid="dreamsForFuture-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="whatTheyWillTalkAbout">What They Will Talk About</label>
		<textarea
			id="whatTheyWillTalkAbout"
			bind:value={formData.whatTheyWillTalkAbout}
			rows="3"
			data-testid="whatTheyWillTalkAbout-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="loves">Loves (one per line)</label>
		<textarea id="loves" bind:value={lovesText} rows="3" data-testid="loves-input"></textarea>
	</div>

	<div class="form-group">
		<label for="likes">Likes (one per line)</label>
		<textarea id="likes" bind:value={likesText} rows="3" data-testid="likes-input"></textarea>
	</div>

	<div class="form-group">
		<label for="dislikes">Dislikes (one per line)</label>
		<textarea id="dislikes" bind:value={dislikesText} rows="3" data-testid="dislikes-input"
		></textarea>
	</div>

	<div class="form-group">
		<label for="hates">Hates (one per line)</label>
		<textarea id="hates" bind:value={hatesText} rows="3" data-testid="hates-input"></textarea>
	</div>

	<div class="form-group">
		<label for="relationships">Relationships (one per line)</label>
		<textarea
			id="relationships"
			bind:value={relationshipsText}
			rows="3"
			data-testid="relationships-input"
		></textarea>
	</div>

	{#if error}
		<p class="error" data-testid="form-error">{error}</p>
	{/if}

	<div class="form-actions">
		<button type="submit" class="button primary" disabled={isSubmitting} data-testid="submit-button">
			{isSubmitting ? 'Submitting...' : submitLabel}
		</button>
		<button type="button" class="button" on:click={onCancel} data-testid="cancel-button">
			Cancel
		</button>
	</div>
</form>

<style>
	.form-group {
		margin-bottom: 1.5rem;
	}

	label {
		display: block;
		margin-bottom: 0.5rem;
		font-weight: bold;
	}

	input[type='text'],
	textarea {
		width: 100%;
		padding: 0.5rem;
		border: 1px solid #ccc;
		border-radius: 4px;
		font-family: inherit;
	}

	textarea {
		resize: vertical;
	}

	.form-actions {
		display: flex;
		gap: 0.5rem;
		margin-top: 2rem;
	}

	.button {
		padding: 0.5rem 1rem;
		background-color: #007bff;
		color: white;
		border: none;
		border-radius: 4px;
		cursor: pointer;
		text-decoration: none;
		display: inline-block;
	}

	.button:hover {
		background-color: #0056b3;
	}

	.button:disabled {
		background-color: #ccc;
		cursor: not-allowed;
	}

	.button.primary {
		background-color: #28a745;
	}

	.button.primary:hover {
		background-color: #218838;
	}

	.error {
		color: red;
		margin-bottom: 1rem;
	}
</style>
