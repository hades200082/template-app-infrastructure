// This file configures the initialization of Sentry on the server.
// The config you add here will be used whenever the server handles a request.
// https://docs.sentry.io/platforms/javascript/guides/nextjs/

import { ENV } from "@/lib/envSchema";
import * as Sentry from "@sentry/nextjs";

Sentry.init({
	dsn: ENV.NEXT_PUBLIC_SENTRY_DSN,
	environment: ENV.NEXT_PUBLIC_SENTRY_ENVIRONMENT,

	// Adjust this value in production, or use tracesSampler for greater control
	tracesSampleRate: ENV.NEXT_PUBLIC_SENTRY_TRACE_SAMPLE_RATE,

	// Setting this option to true will print useful information to the console while you're setting up Sentry.
	debug: false,

	// uncomment the line below to enable Spotlight (https://spotlightjs.com)
	// spotlight: process.env.NODE_ENV === 'development',
  
});
