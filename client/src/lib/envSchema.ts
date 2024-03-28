/* eslint-disable no-process-env */
import { z } from "zod";

const serverEnvSchema = z.object({
	AUTH_CLIENT_ID: z.string(),
	AUTH_CLIENT_SECRET: z.string(),
	AUTH_ISSUER: z.string().trim().url(),
	AUTH_AUDIENCE: z.string().trim().url(),
	AUTH_TOKEN_REFRESH_URI: z.string().trim().url(),
	AUTH_TOKEN_EXPIRATION_SECONDS: z.coerce.number(),
	AUTH_TOKEN_UPDATE_AT_SECONDS: z.coerce.number(),
	NEXTAUTH_URL: z.string().trim().url().optional(),
	NEXTAUTH_SECRET: z.string(),
	NODE_TLS_REJECT_UNAUTHORIZED: z.coerce.number().min(0).max(1).optional(),
	SENTRY_AUTH_TOKEN: z.string().optional(),
	SENTRY_SURPRESS_UPLOAD_SOURCEMAPS: z.boolean().optional(),
	// PUBLIC ENVs
	NEXT_PUBLIC_API_BASE_URL: z.string().trim().url(),
	NEXT_PUBLIC_SENTRY_DSN: z.string().optional(),
	NEXT_PUBLIC_SENTRY_ENVIRONMENT: z.string().optional(),
	NEXT_PUBLIC_SENTRY_REPLAY_SESSION_SAMPLE_RATE: z.number().min(0).max(1).optional(),
	NEXT_PUBLIC_SENTRY_TRACE_SAMPLE_RATE: z.number().min(0).max(1).optional(),
}).refine(
	schema => 
		schema.AUTH_TOKEN_UPDATE_AT_SECONDS <= schema.AUTH_TOKEN_EXPIRATION_SECONDS, 
	{
		message: "AUTH_TOKEN_EXPIRATION_SECONDS must be greater than AUTH_TOKEN_UPDATE_AT_SECONDS",
	}
);

const clientEnvSchema = z.object({
	AUTH_CLIENT_ID: z.string(),
	AUTH_CLIENT_SECRET: z.string(),
	AUTH_ISSUER: z.string().trim().url(),
	AUTH_AUDIENCE: z.string().trim().url(),
	AUTH_TOKEN_REFRESH_URI: z.string().trim().url(),
	AUTH_TOKEN_EXPIRATION_SECONDS: z.coerce.number(),
	AUTH_TOKEN_UPDATE_AT_SECONDS: z.coerce.number(),
	NEXTAUTH_URL: z.string().trim().url().optional(),
	NEXTAUTH_SECRET: z.string(),
	NODE_TLS_REJECT_UNAUTHORIZED: z.coerce.number().min(0).max(1).optional(),
	SENTRY_AUTH_TOKEN: z.string().optional(),
	SENTRY_SURPRESS_UPLOAD_SOURCEMAPS: z.boolean().optional(),
	// PUBLIC ENVs
	NEXT_PUBLIC_API_BASE_URL: z.string().trim().url(),
	NEXT_PUBLIC_SENTRY_DSN: z.string().optional(),
	NEXT_PUBLIC_SENTRY_ENVIRONMENT: z.string().optional(),
	NEXT_PUBLIC_SENTRY_REPLAY_SESSION_SAMPLE_RATE: z.number().min(0).max(1).optional(),
	NEXT_PUBLIC_SENTRY_TRACE_SAMPLE_RATE: z.number().min(0).max(1).optional(),
});

export const ENV = typeof window === "undefined" ? serverEnvSchema.parse(process.env) :
	/*
	* The client schema must be manually specified, referencing each process.env variable individually.
	* This is because the NextJS WebPack build process _replaces_ all instances of `process.env.*` with the
	* literal value of that environment variable at build time, but only for "NEXT_PUBLIC_" prefixed
	* variables. `process.env` itself is not available on the client side.
	**/
	clientEnvSchema.parse({
		NEXT_PUBLIC_API_BASE_URL: process.env.NEXT_PUBLIC_API_BASE_URL,
		NEXT_PUBLIC_SENTRY_DSN: process.env.NEXT_PUBLIC_SENTRY_DSN,
		NEXT_PUBLIC_SENTRY_ENVIRONMENT: process.env.NEXT_PUBLIC_SENTRY_ENVIRONMENT,
		NEXT_PUBLIC_SENTRY_REPLAY_SESSION_SAMPLE_RATE: process.env.NEXT_PUBLIC_SENTRY_REPLAY_SESSION_SAMPLE_RATE,
		NEXT_PUBLIC_SENTRY_TRACE_SAMPLE_RATE: process.env.NEXT_PUBLIC_SENTRY_TRACE_SAMPLE_RATE
	});

export const getEnvIssues = (): z.ZodIssue[] | void => {
	const result = serverEnvSchema.safeParse(process.env);
	if (!result.success) return result.error.issues;
};
