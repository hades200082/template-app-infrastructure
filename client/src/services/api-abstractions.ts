import { removeTrailingSlash } from "@/utils/urlHelper";
import { z, ZodSchema } from "zod"

export const API_BASE_URL = removeTrailingSlash(process.env.API_BASE_URL!);

interface ApiAbstractions {
	baseUrl:string;
	parseResult: <TResult>(obj:any, schema:ZodSchema) => Promise<TResult|ApiError>
}

export interface FindApi<TFindResult> extends ApiAbstractions {
	findAsync: (id:string) => Promise<TFindResult|ApiError|null>;
}

export interface GetApi<TGetResult> extends ApiAbstractions {
	getAsync: (filter:string, orderBy:string, orderDirection:string) => Promise<TGetResult|ApiError>;
}

export interface PostApi<TPostInput, TPostResult> extends ApiAbstractions {
	postAsync: (obj:TPostInput) => Promise<TPostResult|ApiValidationError|ApiError>;
}

export interface PutApi<TPutInput, TPutResult> extends ApiAbstractions {
	putAsync: (obj:TPutInput) => Promise<TPutResult|ApiValidationError|ApiError>;
}

export interface PatchApi<TPatchResult> extends ApiAbstractions {
	patchAsync: (patch:JsonPatch) => Promise<TPatchResult|ApiValidationError|ApiError>;
}

export interface DeleteApi<TDeleteResult> extends ApiAbstractions {
	deleteAsync: (id:string) => Promise<TDeleteResult|ApiValidationError|ApiError>;
}

export const ApiErrorSchema = z.object({
	type: z.string().optional(),
	title: z.string().optional(),
	status: z.number().int().optional(),
	detail: z.string().optional(),
	instance: z.string().optional(),
});
export type ApiError = z.infer<typeof ApiErrorSchema>;

export const ApiValidationErrorSchema = z.object({
	errors: z.string().array().optional()
}).extend(ApiErrorSchema.shape);
export type ApiValidationError = z.infer<typeof ApiValidationErrorSchema>;

export const JsonPatchOperationSchema = z.object({
	op: z.enum(["add", "remove", "replace", "copy", "move"]),
	path: z.string().startsWith("/"),
	value: z.any()
});
export type JsonPatchOperation = z.infer<typeof JsonPatchOperationSchema>;

const JsonPatchSchema = JsonPatchOperationSchema.array()
	.max(10); // JsonPatch in Cosmos only allows 10 patch operations. We may remove this and implement patch splitting in the API layer yet - not sure.
export type JsonPatch = z.infer<typeof JsonPatchSchema>;
