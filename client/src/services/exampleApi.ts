import {
	API_BASE_URL,
	ApiError,
	ApiErrorSchema,
	ApiValidationError,
	ApiValidationErrorSchema,
	getToken, parseResult
} from "@/services/api-abstractions";
import { z } from "zod";

// Define the base URL for the resource this file deals with
const baseUrl = `${API_BASE_URL}v1/example`;

//#region ZodSchema and type definitions
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
 * Finds a single item from the API by it's ID
 *
 * @param id - The ID of the item to be retrieved
 * @returns
 * A promise that will resolve to the item if it exists, null if the ID is not found
 * or an {@link ApiError} if an error was thrown/returned by the API
 */
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
				return null; // null when not found
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
	};

/**
 * Creates a new item via the API
 *
 * @param obj - The model that defines the creation of a new item
 * @returns
 * A promise that will resolve to the item if it was created, a {@link ApiValidationError} if
 * the given `obj` argument or the returned data is invalid or an {@link ApiError} if an error
 * was thrown/returned by the API
 */
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
		});

		const json = await response.json();

		if(response.status !== 201){ // created
			if(response.status === 400) {
				// Bad Request is typically used for validation errors
				const validationError = await ApiValidationErrorSchema.parseAsync(json);
				console.warn(validationError);
				return validationError;
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

		console.groupEnd();

		return finalResult;
	};
