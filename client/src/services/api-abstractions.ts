import {removeTrailingSlash} from "@/utils/urlHelper";

export const API_BASE_URL = removeTrailingSlash(process.env.API_BASE_URL!);

interface ApiAbstractions {
	baseUrl:string;
}

export interface FindApi<TFindResult> extends ApiAbstractions {
	findAsync: (id:string) => Promise<TFindResult>;
}

export interface GetApi<TGetResult> extends ApiAbstractions {
	getAsync: (filter:string, orderBy:string, orderDirection:string) => Promise<TGetResult>;
}

export interface PostApi<TPostInput, TPostResult> extends ApiAbstractions {
	postAsync: (obj:TPostInput) => Promise<TPostResult>;
}

export interface PutApi<TPutInput, TPutResult> extends ApiAbstractions {
	putAsync: (obj:TPutInput) => Promise<TPutResult>;
}

export interface PatchApi<TPatchResult> extends ApiAbstractions {
	patchAsync: (patch:JsonPatch) => Promise<TPatchResult>;
}

export interface DeleteApi<TDeleteResult> extends ApiAbstractions {
	deleteAsync: (id:string) => Promise<TDeleteResult>;
}

// TODO define zod and types for ValidationProblem and Problem
