"use client"

import { signOut } from "next-auth/react";
import { ENV } from "@/lib/envSchema";

export default function LogoutButton() {
	return (
		<>
			<button 
				type="button"
				onClick={() => signOut({ callbackUrl: `${ENV.NEXTAUTH_URL}/api/auth/logout`})}
			>
				Log out
			</button>
		</>
	)}
