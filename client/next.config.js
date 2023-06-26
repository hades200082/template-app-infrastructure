/** @type {import('next').NextConfig} */

if((process.env.NODE_ENV || "development") === "development")
	process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
	
const nextConfig = {
	env: {
		NEXTAUTH_URL: process.env.NEXTAUTH_URL
	}
}

module.exports = nextConfig
