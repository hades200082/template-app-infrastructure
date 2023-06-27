import { authOptions } from "@/services/auth"
import { getServerSession } from "next-auth"
import LogoutButton from "./LogoutButton";
import { ExampleApi } from "@/services/exampleApi";

export default async function Account() {
	const session = await getServerSession(authOptions);
	const api = new ExampleApi(session!.accessToken);
	const response = await api.findAsync("test");

	return (
		<>
			<h1>Account</h1>
			<p>
				To view this page you need to be logged in.<br />
				This is your identityId: <strong>{session?.user.identityId}</strong>
			</p>
			<LogoutButton />
		</>
	)
}
