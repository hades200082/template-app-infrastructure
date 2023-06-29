import {
	API_BASE_URL,
	ApiErrorSchema,
	FindApi,
	PostApi,
	ApiError,
	ApiValidationError,
	ApiValidationErrorSchema,
	CoreApi
} from "@/services/api-abstractions"
import { z, ZodSchema } from "zod"

//#region Define Zod schemas and TypeScript types for this service
const ExampleObjSchema = z.object({
	id: z.string(),
	name: z.string()
});
export type ExampleObj = z.infer<typeof ExampleObjSchema>;

const CreateExampleObjSchema = z.object({
	name: z.string()
});
export type CreateExampleObj = z.infer<typeof CreateExampleObjSchema>;
//#endregion

/**
 * This is an example class linked to the ExampleEntity resource on the API in this template.
 *
 * @remarks
 * While we could just export our ExampleApi class and let it be "newed up" where ever it is needed
 * this would mean that we may have multiple instances of the class lying around in memory throughout
 * the application.
 *
 * A more memory efficient way is to use a singleton.
 *
 * Technically we could `export default new ExampleApi()` and this would produce a singleton. However,
 * this would still leave the constructor as public, meaning that it could still be newed up separately.
 *
 * To enforce the singleton approach we must explicitly declare it as a singleton and make the constructor
 * private.
 *
 * @public
 */
class ExampleApi
extends CoreApi
implements
	FindApi<ExampleObj>,
	PostApi<CreateExampleObj, ExampleObj>
{
	private readonly _baseUrl: string = `${API_BASE_URL}v1/example`;

	// Explicitly define a singleton instance
	private static _instance = new ExampleApi();

	// Make our constructor private so nothing else can new it up
	// enforcing our singleton state below
	private constructor() {
		super();
	}

	// give the instance a public accessor
	static get instance() {
		return this._instance;
	}

	/** {@inheritDoc FindApi.findAsync} */
	async findAsync(id: string): Promise<ExampleObj|ApiError|null> {
		const response = await fetch(
			`${this._baseUrl}/${id}`, {
				headers: {
					Authorization: `Bearer ${await this.getToken()}`
				}
			}
		);

		console.log(response);

		const json = await response.json();

		if(response.status !== 200){ // not an "Ok" response
			if(response.status === 404) {
				console.info(`${id} not found`);
				return null // null when not found
			}

			// Otherwise return the ApiError - the consumer must deal with this.
			else {
				const error = await ApiErrorSchema.parseAsync(json);
				console.error(error);
				return error;
			}
		}

		// All good, let"s parse the response
		return await this.parseResult<ExampleObj>(json, ExampleObjSchema);
	}

	/** {@inheritDoc FindApi.postAsync} */
	async postAsync(obj: CreateExampleObj): Promise<ExampleObj|ApiValidationError|ApiError> {
		console.group("postAsync");
		const model = await CreateExampleObjSchema.parseAsync(obj);

		const response = await fetch(this._baseUrl, {
			headers: {
				Authorization: `Bearer ${await this.getToken()}`
			},
			method: "POST",
			body: JSON.stringify(model)
		})

		const json = await response.json();

		if(response.status !== 201){ // created
			if(response.status === 400) {
				// Bad Request is typically used for validation errors
				const validationError = await ApiValidationErrorSchema.parseAsync(json)
				console.warn(validationError)
				return validationError
			}
			else {
				const error = await ApiErrorSchema.parseAsync(json);
				console.error(error);
				return error;
			}
		}

		console.info(`POST request successfully sent to ${this._baseUrl}`);

		// All good, let"s parse the response
		const finalResult = await this.parseResult<ExampleObj>(json, ExampleObjSchema);

		console.groupEnd()

		return finalResult;
	}


	/** {@inheritDoc ApiAbstractions.parseResult} */
	async parseResult<TResult>(obj: any, schema: ZodSchema): Promise<ApiError | TResult> {
		const parseResult = await schema.safeParseAsync(obj);

		if(!parseResult.success) {
			console.error("The value returned by the server does not match the ExampleObjSchema", parseResult, obj);
			return await ApiErrorSchema.parseAsync({
				type: "ResponseValidationError",
				title: "The server returned an unexpected schema but the item may have been created.",
				detail: parseResult.error,
				instance: JSON.stringify(obj)
			})
		}

		return parseResult.data;
	}

}
export default ExampleApi.instance;
