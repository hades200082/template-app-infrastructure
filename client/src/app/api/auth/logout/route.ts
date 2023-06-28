import { NextResponse } from "next/server";
import { ENV } from "@/lib/envSchema";

export async function GET(req: Request) {
	const returnTo = encodeURI(`${ENV.NEXTAUTH_URL}/login`);

	return NextResponse.redirect(
		`${ENV.AUTH_ISSUER}/v2/logout?federated&returnTo=${returnTo}&client_id=${ENV.AUTH_CLIENT_ID}`
	);
}
