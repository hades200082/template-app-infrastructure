import { NextAuthOptions } from "next-auth";
import Auth0 from "next-auth/providers/auth0";

export const authOptions:NextAuthOptions = {
	session: {
		strategy: "jwt",
		maxAge: 24*60*60,
		updateAge: 23*60*60
	},
	jwt: {
		maxAge: 24*60*60
	},
	providers: [
		Auth0({
			clientId: process.env.AUTH_CLIENT_ID!,
			clientSecret: process.env.AUTH_CLIENT_SECRET!,
			issuer: process.env.AUTH_ISSUER!
		})
	],
	secret: process.env.NEXTAUTH_SECRET!,
	callbacks: {
		async session({ session, token }) {
			session.user.identityId = token.id as string;
			session.accessToken = token.accesstoken as string;
			
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
