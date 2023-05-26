import {removeStartingSlash, removeTrailingSlash} from "@/utils/urlHelper";
import {ZodSchema} from "zod";

export const API_BASE_URL = removeTrailingSlash(process.env.API_BASE_URL!);

// TODO: Add API services

interface Api {
	baseUrl:string;
}

export interface FindApi<TFindResult> extends Api {
	findResultValidator: ZodSchema;
	findAsync: (id:string) => Promise<TFindResult>;
}

export interface GetApi<TGetResult> extends Api {
	getResultValidator: ZodSchema;
	getAsync: (filter:string, orderBy:string, orderDirection:string) => Promise<TGetResult>;
}

export interface PostApi<TPostInput, TPostResult> extends Api {
	postInputValidator: ZodSchema;
	postResultValidator: ZodSchema;
	postAsync: (obj:TPostInput) => Promise<TPostResult>;
}

export interface PutApi<TPutInput, TPutResult> extends Api {
	putInputValidator: ZodSchema;
	putResultValidator: ZodSchema;
	putAsync: (obj:TPutInput) => Promise<TPutResult>;
}

export interface PatchApi<TPatchResult> extends Api {
	patchResultValidator: ZodSchema;
	patchAsync: (patch:JsonPatch) => Promise<TPatchResult>;
}

export interface DeleteApi<TDeleteResult> extends Api {
	deleteResultValidator: ZodSchema;
	deleteAsync: (id:string) => Promise<TDeleteResult>;
}
