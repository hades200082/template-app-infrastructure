import { NextResponse } from "next/server"

export async function GET(req: Request) {
	const returnTo = encodeURI(`${process.env.NEXTAUTH_URL}/login`);

	return NextResponse.redirect(
		`${process.env.AUTH_ISSUER}/v2/logout?federated&returnTo=${returnTo}&client_id=${process.env.AUTH_CLIENT_ID}`
	);
}
