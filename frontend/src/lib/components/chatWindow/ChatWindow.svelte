<script lang="ts">
import { afterUpdate } from 'svelte';
import AIApi from '$lib/api/AIApi';

const api = new AIApi();

// Optional initial session can be provided by parent
export let initialSession = null;

let session = initialSession ?? { sessionId: '', messages: [] };
let input = '';
let loading = false;
let error = '';
let messagesContainer: HTMLDivElement | null = null;

async function sendMessage() {
    error = '';
    const text = input.trim();
    if (!text) return;


    // Clear input and set loading
    input = '';
    loading = true;

    try {
        // If we have a sessionId use the whole session, otherwise send undefined
        let promise = api.ChatWithDarthVader(text, session.sessionId ? session : undefined as any);

        // Optimistically append the user's message
        session = {
            ...session,
            messages: [...session.messages, { role: 'User', message: text }]
        };

        const res = await promise;
        if (res && res.messages) {
            session = res;
        }
    } catch (err: any) {
        console.error('Chat error', err);
        error = err?.message ?? 'Failed to send message';
    } finally {
        loading = false;
    }
}

// auto-scroll to bottom whenever messages change
afterUpdate(() => {
    if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
});
</script>

<style>
.chat-window {
    display: flex;
    flex-direction: column;
    height: 100%;
    border: 1px solid var(--border-color, #e0e0e0);
    border-radius: 8px;
    background: var(--bg, #fff);
    overflow: hidden;
}
.header {
    padding: 12px 16px;
    font-weight: 600;
    border-bottom: 1px solid var(--border-color, #eaeaea);
    background: var(--header-bg, #fafafa);
}
.messages {
    flex: 1 1 auto;
    padding: 12px;
    overflow-y: auto;
    display: flex;
    flex-direction: column;
    gap: 8px;
}
.empty {
    color: #666;
    font-size: 0.95rem;
    text-align: center;
    margin-top: 12px;
}
.message {
    display: flex;
}
.message[data-role="User"] {
    justify-content: flex-end;
}
.message[data-role="Bot"] {
    justify-content: flex-start;
}
.bubble {
    max-width: 75%;
    padding: 10px 14px;
    border-radius: 15px;
    line-height: 1.3;
    white-space: pre-wrap;
    word-break: break-word;
}
.User .bubble {
    background: linear-gradient(180deg,#1f6feb,#0b4cd6);
    color: white;
    border-bottom-right-radius: 4px;
}
.Bot .bubble {
    background: #8e0627;
    color: #efefef;
    border-bottom-left-radius: 4px;
}
.composer {
    display: flex;
    gap: 8px;
    padding: 10px;
    border-top: 1px solid var(--border-color, #eaeaea);
    align-items: center;
}
.composer textarea {
    flex: 1 1 auto;
    resize: none;
    padding: 8px 10px;
    border-radius: 8px;
    border: 1px solid #ddd;
    font-size: 0.95rem;
    min-height: 38px;
    max-height: 120px;
}
.composer button {
    background: #0b63d6;
    color: white;
    border: none;
    padding: 8px 12px;
    border-radius: 8px;
    cursor: pointer;
}
.composer button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}
.error {
    color: #b00020;
    padding: 8px 12px;
    font-size: 0.9rem;
}
</style>

<div class="chat-window">
    <div class="header">Chat with Darth Vader</div>

    <div class="messages" bind:this={messagesContainer} aria-live="polite">
        {#if session.messages.length === 0}
            <div class="empty">No messages yet. Say hello to Darth Vader.</div>
        {/if}

        {#each session.messages as msg, idx}
            <div class="message {msg.role}" data-role={msg.role}>
                <div class="bubble">{msg.message}</div>
            </div>
        {/each}
    </div>

    <form class="composer" on:submit|preventDefault={sendMessage}>
        <textarea bind:value={input} placeholder="Type a message..." rows={1} disabled={loading}></textarea>
        <button type="submit" disabled={loading || input.trim().length === 0}>{loading ? '...' : 'Send'}</button>
    </form>

    {#if error}
        <div class="error">{error}</div>
    {/if}
</div>
