import { authOptions } from "@/services/auth"
import { getServerSession } from "next-auth"
import { redirect } from "next/navigation";
import LoginContainer from "./LoginContainer";

export default async function Login() {
	const session = await getServerSession(authOptions);

	if(session) redirect("/");
	
	return (
		<>
			<LoginContainer />
		</>
	)
}
