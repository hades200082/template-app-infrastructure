import { cleanUrl } from "@/utils/urlHelper";
import { getServerSession } from "next-auth";
import { z, ZodSchema } from "zod"
import { authOptions } from "./auth";
import { getSession, signIn } from "next-auth/react";
import { ENV } from "@/lib/envSchema";

export const API_BASE_URL = cleanUrl(ENV.API_BASE_URL);

interface ApiAbstractions {
	/**
	 * Parses the given object using the given ZodSchema and returns the typed object or an ApiError
	 *
	 * @param obj - The object to be parsed
	 * @param schema - The ZodSchema to be used to validate the object
	 * @returns The typed object or an ApiError
	 */
	parseResult: <TResult>(obj:any, schema:ZodSchema) => Promise<TResult|ApiError>
}

export class CoreApi {
	constructor(){}

	async getToken() {
		if(typeof window === "undefined") {
			const session = await getServerSession(authOptions);

			if (session === null || session?.error === "RefreshAccessTokenError") {
				signIn("auth0");
			}

			return session!.accessToken
		}

		const session = await getSession();

		if(session === null || session?.error === "RefreshAccessTokenError") {
			signIn("auth0")
		}

		return session!.accessToken;
	}
}


/**
 * An interface for simple GET resource/{id} operations
 * @public
 */
export interface FindApi<TFindResult> extends ApiAbstractions {
	/**
	 * Retrieves the resource/record identified by the ID given.
	 *
	 * @param id - the ID of the item to retrieve
	 * @returns The item requested if it exists, null if it doesn't or an API Error if anything goes wrong.
	 */
	findAsync: (id:string) => Promise<TFindResult|ApiError|null>;
}

/**
 * An interface for simple GET resource/ operations
 *
 * @public
 */
export interface GetApi<TGetResult> extends ApiAbstractions {
	/**
	 * Retrieves a list of resources based on the filter and orderBy clauses.
	 *
	 * @param filter - A querystring to tells the API what to filter on. Check the API docs for details.
	 * @param orderBy - Tells the API which field to order by. Check the API docs for what is available.
	 * @param orderDirection - Tells the API which direction to order. Requires an orderBy be set.
	 */
	getAsync: (filter:string, orderBy:string, orderDirection:string) => Promise<TGetResult|ApiError>;
}

/**
 * An interface for POST resource/ create operations
 *
 * @public
 */
export interface PostApi<TPostInput, TPostResult> extends ApiAbstractions {
	/**
	 * Sends a POST request to the API with the provided model and validates the response.
	 *
	 * @param obj - The model for the thing we want to create. See the API docs for details.
	 * @returns The created object from the API
	 */
	postAsync: (obj:TPostInput) => Promise<TPostResult|ApiValidationError|ApiError>;
}

/**
 * An interface for PUT resource/{id} update/replace operations
 *
 * @public
 */
export interface PutApi<TPutInput, TPutResult> extends ApiAbstractions {
	/**
	 * Sends a PUT request to the API with the provided model and validates the response.
	 *
	 * A PUT replaces the data on the server with the data provided.
	 *
	 * @param obj - The model of the thing we want to update. See the API docs for details.
	 * @returns The updated object from the API
	 */
	putAsync: (obj:TPutInput) => Promise<TPutResult|ApiValidationError|ApiError>;
}

/**
 * An interface for PATCH resource/{id} update operations.
 *
 * A PATCH updates the fields on the server more surgically than a PUT using JSON PATCH
 *
 * @public
 */
export interface PatchApi<TPatchResult> extends ApiAbstractions {
	/**
	 * Sends a PATCH request to the API with the provided JsonPatch and validates the response..
	 *
	 * @param patch - The JsonPatch describing the changes to make to the target resource. See the API docs for details.
	 * @returns The updated object from the API
	 */
	patchAsync: (patch:JsonPatch) => Promise<TPatchResult|ApiValidationError|ApiError>;
}

/**
 * An interface for DELETE resource/{id} update operations.
 *
 * @public
 */
export interface DeleteApi<TDeleteResult> extends ApiAbstractions {
	/**
	 * Deletes the resource/record identified by the ID given.
	 *
	 * @param id - the ID of the item to delete
	 */
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
