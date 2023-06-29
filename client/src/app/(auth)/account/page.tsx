import LogoutButton from "./LogoutButton";
import { findAsync } from "@/services/exampleApi"

export default async function Account() {
	const response = await findAsync("test");

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
