"use client"

import { signOut } from "next-auth/react"

export default function LogoutButton() {
	return (
		<>
			<button 
				type="button"
				onClick={() => signOut({ callbackUrl: `${process.env.NEXTAUTH_URL}/api/auth/logout`})}
			>
				Log out
			</button>
		</>
	)
}
