/* eslint-disable no-process-env */
import { z } from "zod";

const envSchema = z.object({
	API_BASE_URL: z.string().trim().url(),
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
	SENTRY_DSN: z.string().optional(),
	SENTRY_PROJECT_NAME: z.string().optional(),
	SENTRY_REPLAY_SESSION_SAMPLE_RATE: z.number().min(0).max(1).optional(),
	SENTRY_SURPRESS_UPLOAD_SOURCEMAPS: z.boolean().optional(),
	SENTRY_TRACE_SAMPLE_RATE: z.number().min(0).max(1).optional(),
}).refine(
	schema => 
		schema.AUTH_TOKEN_UPDATE_AT_SECONDS <= schema.AUTH_TOKEN_EXPIRATION_SECONDS, 
	{
		message: "AUTH_TOKEN_EXPIRATION_SECONDS must be greater than AUTH_TOKEN_UPDATE_AT_SECONDS",
	}
);

export const ENV = envSchema.parse(process.env);

export const getEnvIssues = (): z.ZodIssue[] | void => {
	const result = envSchema.safeParse(process.env);
	if (!result.success) return result.error.issues;
};
