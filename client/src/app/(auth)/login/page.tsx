import { authOptions } from "@/services/auth";
import { getServerSession } from "next-auth";
import LoginContainer from "./LoginContainer";
import { redirect } from "next/navigation";

export default async function Login() {
	const session = await getServerSession(authOptions);

	if(session) redirect("/");
	
	return (
		<>
			<LoginContainer />
		</>
	);
}
