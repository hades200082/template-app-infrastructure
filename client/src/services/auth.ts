import { type NextAuthOptions, type TokenSet } from "next-auth";
import Auth0 from "next-auth/providers/auth0";
import { ENV } from "@/lib/envSchema";

export const authOptions:NextAuthOptions = {
	session: {
		strategy: "jwt",
		maxAge: ENV.AUTH_TOKEN_EXPIRATION_SECONDS,
		updateAge: ENV.AUTH_TOKEN_UPDATE_AT_SECONDS
	},
	jwt: {
		maxAge: ENV.AUTH_TOKEN_EXPIRATION_SECONDS
	},
	providers: [
		Auth0({
			clientId: ENV.AUTH_CLIENT_ID,
			clientSecret: ENV.AUTH_CLIENT_SECRET,
			issuer: ENV.AUTH_ISSUER,
			authorization: { params: { scope: "openid email profile", audience: ENV.AUTH_AUDIENCE } }
		})
	],
	secret: ENV.NEXTAUTH_SECRET,
	pages: {
		signIn: "/login"
	},
	callbacks: {
		async session({ session, token }) {
			session.user.identityId = token.id as string;
			session.accessToken = token.accessToken as string;
			session.error = token.error as "RefreshAccessTokenError";

			// TODO: fetch current user profile from api to populate session

			return session;
		},
		async jwt({ token, account }) {
			if(account) {
				return {
					accessToken: account.access_token,
					expires_at: Math.floor(Date.now() / 1000 + (account.expires_at as number)),
					refresh_token: account.refresh_token
				}
			}
			else if (Date.now() < ((token.exp as number) * 1000)) {
				return token;
			} 
			else {
				try {
					const response = await fetch(ENV.AUTH_TOKEN_REFRESH_URI!, {
						headers: { "Content-Type": "application/x-www-form-urlencoded" },
						body: new URLSearchParams({
							client_id: ENV.AUTH_CLIENT_ID!,
							client_secret: ENV.AUTH_CLIENT_SECRET!,
							grant_type: "refresh_token",
							refresh_token: token.accessToken as string
						})
					});
	
					const tokens: TokenSet = await response.json();
	
					if (!response.ok) throw tokens;
	
					return {
						...token, // Keep the previous token properties
						access_token: tokens.access_token,
						expires_at: Math.floor(Date.now() / 1000 + (tokens.expires_at as number)),
						// Fall back to old refresh token, but note that
						// many providers may only allow using a refresh token once.
						refresh_token: tokens.refresh_token ?? token.refresh_token,
					}
				}
				catch (error) {
          console.error("Error refreshing access token", error)
          // The error property will be used client-side to handle the refresh token error
          return { ...token, error: "RefreshAccessTokenError" as const }
        }				
			}
		}
	}
}
