"use client";

import { ENV } from "@/lib/envSchema";
import { signOut } from "next-auth/react";

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
	);}
