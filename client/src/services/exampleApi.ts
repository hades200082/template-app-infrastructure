import {API_BASE_URL, FindApi, PostApi} from "@/services/api-abstractions";
import {z, ZodSchema} from "zod";

export class ExampleApi	implements
	FindApi<ExampleObj>,
	PostApi<CreateExampleObj, ExampleObj>
{
	baseUrl: string;

	constructor() {
		this.baseUrl = `${API_BASE_URL}/v1/example`
	}

	async findAsync(id: string): Promise<ExampleObj> {
		const response = await fetch(`${this.baseUrl}/${id}`);
		if(response.status !== 200){ // Ok
			// handle error here
		}
		return await ExampleObjSchema.parseAsync(await response.json());
	}

	async postAsync(obj: CreateExampleObj): Promise<ExampleObj> {
		const model = await CreateExampleObjSchema.parseAsync(obj);

		const response = await fetch(this.baseUrl, {
			method: "POST",
			body: JSON.stringify(model)
		})

		if(response.status !== 201){ // created
			// handle error here
		}

		return await ExampleObjSchema.parseAsync(await response.json());
	}

}

// TODO define zod & type for ExampleEntity
const ExampleObjSchema = z.object({
	id: z.string(),
	name: z.string()
});
export type ExampleObj = z.infer<typeof ExampleObjSchema>;

const CreateExampleObjSchema = z.object({
	name: z.string()
});
export type CreateExampleObj = z.infer<typeof CreateExampleObjSchema>;
