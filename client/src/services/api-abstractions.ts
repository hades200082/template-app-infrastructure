import {cleanUrl} from '@/utils/urlHelper'
import {getServerSession} from 'next-auth'
import {z, ZodSchema} from 'zod'
import {authOptions} from './auth'
import {getSession, signIn} from 'next-auth/react'
import {ENV} from '@/lib/envSchema'

export const API_BASE_URL = cleanUrl(ENV.API_BASE_URL)

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

interface ApiAbstractions {
    /**
     * Parses the given object using the given ZodSchema and returns the typed object or an ApiError
     *
     * @param obj - The object to be parsed
     * @param schema - The ZodSchema to be used to validate the object
     * @returns The typed object or an ApiError
     */
    parseResult: <TResult>(obj: any, schema: ZodSchema) => Promise<TResult | ApiError>
}

export type FindApi = <TFindResult>(id: string) => Promise<TFindResult | ApiError | null>;
export type GetApi = <TGetResult>(filter: string, orderBy: string, orderDirection: string) => Promise<TGetResult | ApiError>;
export type PostApi = <TPostInput, TPostResult>(obj: TPostInput) => Promise<TPostResult | ApiValidationError | ApiError>;
export type PutApi = <TPutInput, TPutResult>(obj: TPutInput) => Promise<TPutResult | ApiValidationError | ApiError>;
export type PatchApi = <TPatchResult>(patch: JsonPatch) => Promise<TPatchResult | ApiValidationError | ApiError>;
export type DeleteApi = <TDeleteResult>(id: string) => Promise<TDeleteResult | ApiValidationError | ApiError>;

export const ApiErrorSchema = z.object({
    type: z.string().optional(),
    title: z.string().optional(),
    status: z.number().int().optional(),
    detail: z.string().optional(),
    instance: z.string().optional(),
})
export type ApiError = z.infer<typeof ApiErrorSchema>;

export const ApiValidationErrorSchema = z.object({
    errors: z.string().array().optional()
}).extend(ApiErrorSchema.shape)
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
