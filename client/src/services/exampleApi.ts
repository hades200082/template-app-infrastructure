import {
	API_BASE_URL,
	ApiErrorSchema,
	FindApi,
	PostApi,
	ApiError,
	ApiValidationError,
	ApiValidationErrorSchema,
	getToken, parseResult
} from "@/services/api-abstractions"
import { z } from "zod"

// Define the base URL for this resource
const baseUrl: string = `${API_BASE_URL}v1/example`;

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

export const findAsync : FindApi =
	async function <ExampleObj>(id: string):Promise<ExampleObj|ApiError|null> {
		const response = await fetch(
			`${baseUrl}/${id}`, {
				headers: {
					Authorization: `Bearer ${await getToken()}`
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
		return await parseResult<ExampleObj>(json, ExampleObjSchema);
	}

export const postAsync  : PostApi =
	async function <CreateExampleObj, ExampleObj>(obj: CreateExampleObj): Promise<ExampleObj|ApiValidationError|ApiError> {
		console.group("postAsync");
		const model = await CreateExampleObjSchema.parseAsync(obj);

		const response = await fetch(baseUrl, {
			headers: {
				Authorization: `Bearer ${await getToken()}`
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

		console.info(`POST request successfully sent to ${baseUrl}`);

		// All good, let"s parse the response
		const finalResult = await parseResult<ExampleObj>(json, ExampleObjSchema);

		console.groupEnd()

		return finalResult;
	}
