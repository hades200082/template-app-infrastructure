import { DefaultSession } from "next-auth";

declare module "next-auth" {
	  /**
   * Returned by `useSession`, `getSession` and received as a prop on the `SessionProvider` React Context
   */
		interface Session {
			accessToken: string,
			error?: "RefreshAccessTokenError",
			user: {
				identityId: string,
				// TODO: add anymore user specific data to session, e.g. email, first/last name etc
			} & DefaultSession["user"]
		}

		interface JWT {
			access_token: string,
			expires_at: number,
			refresh_token: string,
			error?: "RefreshAccessTokenError"
		}
}
