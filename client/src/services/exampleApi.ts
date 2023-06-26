import {
	API_BASE_URL,
	ApiErrorSchema,
	FindApi,
	PostApi,
	ApiError,
	ApiValidationError,
	ApiValidationErrorSchema
} from "@/services/api-abstractions"
import { z, ZodSchema } from "zod"

export class ExampleApi	implements
	FindApi<ExampleObj>,
	PostApi<CreateExampleObj, ExampleObj>
{
	baseUrl: string;

	constructor() {
		this.baseUrl = `${API_BASE_URL}/v1/example`
	}

	async findAsync(id: string, accessToken: string): Promise<ExampleObj|ApiError|null> {
		const response = await fetch(
			`${this.baseUrl}/${id}`, {
				headers: {
					Authorization: `Bearer ${accessToken}`
				}
			}
		);

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

	async postAsync(obj: CreateExampleObj, accessToken: string): Promise<ExampleObj|ApiValidationError|ApiError> {
		console.group("postAsync");
		const model = await CreateExampleObjSchema.parseAsync(obj);

		const response = await fetch(this.baseUrl, {
			headers: {
				Authorization: `Bearer ${accessToken}`
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

		console.info(`POST request successfully sent to ${this.baseUrl}`);

		// All good, let"s parse the response
		const finalResult = await this.parseResult<ExampleObj>(json, ExampleObjSchema);

		console.groupEnd()

		return finalResult;
	}

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

const ExampleObjSchema = z.object({
	id: z.string(),
	name: z.string()
});
export type ExampleObj = z.infer<typeof ExampleObjSchema>;

const CreateExampleObjSchema = z.object({
	name: z.string()
});
export type CreateExampleObj = z.infer<typeof CreateExampleObjSchema>;
