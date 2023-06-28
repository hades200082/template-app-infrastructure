import LogoutButton from "./LogoutButton";
import { ExampleApi } from "@/services/exampleApi";

export default async function Account() {
	const api = new ExampleApi();
	const response = await api.findAsync("test");

	return (
		<>
			<h1>Account</h1>
			<p>
				To view this page you need to be logged in.<br />
			</p>
			<LogoutButton />
		</>
	)
}
