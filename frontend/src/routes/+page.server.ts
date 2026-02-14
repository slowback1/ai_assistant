// Redirect to /dashboard on server-side so the user never sees the page
import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async () => {
    // Use a 307 temporary redirect; adjust to 301 if you want permanent
    throw redirect(301, '/dashboard');
};

