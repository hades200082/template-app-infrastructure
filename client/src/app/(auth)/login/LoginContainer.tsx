"use client";

import { signIn } from "next-auth/react";
import { useEffect } from "react";

export default function LoginContainer() {
	useEffect(() => {
		signIn("auth0")
	}, [])

	return (
		<></>
	)
}
