import { ENV } from "@/lib/envSchema";
import { NextResponse } from "next/server";

export async function GET() {
	const returnTo = encodeURI(`${ENV.NEXTAUTH_URL}/login`);

	return NextResponse.redirect(
		`${ENV.AUTH_ISSUER}/v2/logout?federated&returnTo=${returnTo}&client_id=${ENV.AUTH_CLIENT_ID}`
	);
}
