import { authOptions } from "@/services/auth"
import { getServerSession } from "next-auth"
import { signOut } from "next-auth/react";
import LogoutButton from "./LogoutButton";

export default async function Account() {
	const session = await getServerSession(authOptions);

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
