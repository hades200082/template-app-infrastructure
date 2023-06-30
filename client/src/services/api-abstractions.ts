import {cleanUrl} from '@/utils/urlHelper'
import {getServerSession} from 'next-auth'
import {z, ZodSchema} from 'zod'
import {authOptions} from './auth'
import {getSession, signIn} from 'next-auth/react'
import {ENV} from '@/lib/envSchema'

export const API_BASE_URL = cleanUrl(ENV.API_BASE_URL)

//#region utilities
/**
 * Retrieves the current user's JWT from their logged-in next-auth session
 *
 * @returns A JWT for the current user
 */
export async function getToken() {
    if (typeof window === 'undefined') {
        const session = await getServerSession(authOptions)

        if (session === null || session?.error === 'RefreshAccessTokenError') {
            signIn('auth0')
        }

        return session!.accessToken
    }

    const session = await getSession()

    if (session === null || session?.error === 'RefreshAccessTokenError') {
        signIn('auth0')
    }

    return session!.accessToken
}

/**
 * Parses & validates the given `obj` using the given `schema`
 *
 * @param obj - The object to be parsed/validated
 * @param schema - The {@link ZodSchema} used to parse/validate the `obj`
 * @returns
 * The parsed object is valid, otherwise an {@link ApiError}
 */
export async function parseResult<TResult>(obj: any, schema: ZodSchema): Promise<ApiError | TResult> {
    const parseResult = await schema.safeParseAsync(obj)

    if (!parseResult.success) {
        console.error('The value returned by the server does not match the given type schema.', parseResult, obj)
        return await ApiErrorSchema.parseAsync({
            type: 'ResponseValidationError',
            title: 'The server returned an unexpected schema but the item may have been created.',
            detail: parseResult.error,
            instance: JSON.stringify(obj)
        })
    }

    return parseResult.data
}
//#endregion

//#region endpoint method signature definitions
export type FindApi = <TFindResult>(id: string) => Promise<TFindResult | ApiError | null>;
export type GetApi = <TGetResult>(filter: string, orderBy: string, orderDirection: string) => Promise<TGetResult | ApiError>;
export type PostApi = <TPostInput, TPostResult>(obj: TPostInput) => Promise<TPostResult | ApiValidationError | ApiError>;
export type PutApi = <TPutInput, TPutResult>(obj: TPutInput) => Promise<TPutResult | ApiValidationError | ApiError>;
export type PatchApi = <TPatchResult>(patch: JsonPatch) => Promise<TPatchResult | ApiValidationError | ApiError>;
export type DeleteApi = <TDeleteResult>(id: string) => Promise<TDeleteResult | ApiValidationError | ApiError>;
//#endregion

//#region General ZodSchema & type definitions
export const ApiErrorSchema = z.object({
    type: z.string().optional(),
    title: z.string().optional(),
    status: z.number().int().optional(),
    detail: z.string().optional(),
    instance: z.string().optional(),
})

/**
 * Defines the structure of a `ProblemDetails` object that may be returned by the API
 */
export type ApiError = z.infer<typeof ApiErrorSchema>;

export const ApiValidationErrorSchema = z.object({
    errors: z.string().array().optional()
}).extend(ApiErrorSchema.shape)

/**
 * Defines the structure of a `ValidationProblemDetails` object that may be returned by the API
 */
export type ApiValidationError = z.infer<typeof ApiValidationErrorSchema>;

export const JsonPatchOperationSchema = z.object({
    op: z.enum(['add', 'remove', 'replace', 'copy', 'move']),
    path: z.string().startsWith('/'),
    value: z.any()
})
export type JsonPatchOperation = z.infer<typeof JsonPatchOperationSchema>;

const JsonPatchSchema = JsonPatchOperationSchema.array()
    .max(10) // JsonPatch in Cosmos only allows 10 patch operations. We may remove this and implement patch splitting in the API layer yet - not sure.
export type JsonPatch = z.infer<typeof JsonPatchSchema>;
//#endregion
