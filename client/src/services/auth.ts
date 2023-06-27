import { NextAuthOptions } from "next-auth";
import Auth0 from "next-auth/providers/auth0";

export const authOptions:NextAuthOptions = {
	session: {
		strategy: "jwt",
		maxAge: parseInt(process.env.AUTH_TOKEN_EXPIRATION_SECONDS!),
		updateAge: parseInt(process.env.AUTH_TOKEN_UPDATE_AT_SECONDS!)
	},
	jwt: {
		maxAge: parseInt(process.env.AUTH_TOKEN_EXPIRATION_SECONDS!)
	},
	providers: [
		Auth0({
			clientId: process.env.AUTH_CLIENT_ID!,
			clientSecret: process.env.AUTH_CLIENT_SECRET!,
			issuer: process.env.AUTH_ISSUER!
		})
	],
	secret: process.env.NEXTAUTH_SECRET!,
	pages: {
		signIn: "/login"
	},
	callbacks: {
		async session({ session, token }) {
			session.user.identityId = token.id as string;
			session.accessToken = token.accessToken as string;

			// TODO: fetch current user profile from api to populate session

			return session;
		},
		async jwt({ token, user, account }) {
			if(user) {
				token.id = user.id;
			}
			
			if(account) {
				token.accessToken = account.access_token;
			}

			return token;
		}
	}
}
