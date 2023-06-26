import { NextAuthOptions } from "next-auth";
import Auth0 from "next-auth/providers/auth0";

export const authOptions:NextAuthOptions = {
	session: {
		strategy: "jwt"
	},
	providers: [
		Auth0({
			clientId: process.env.AUTH_CLIENT_ID!,
			clientSecret: process.env.AUTH_CLIENT_SECRET!,
			issuer: process.env.AUTH_ISSUER!
		})
	]
}
