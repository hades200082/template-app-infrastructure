/** @type {import('next').NextConfig} */

const nextConfig = {
	async headers() {
		const ContentSecurityPolicy = `
		  default-src 'self';
		  script-src 'self';
		  style-src 'self';
		  font-src 'self';
		`;

		return [
			{
				source: "/:path*",
				headers: [
					{
						key: "Content-Security-Policy",
						value: ContentSecurityPolicy.replace(/\s{2,}/g, " ").trim()
					},
					{
						key: "Permissions-Policy",
						value: "camera=(), microphone=(), geolocation=(), browsing-topics=()"
					},
					{
						key: "X-XSS-Protection",
						value: "1; mode=block"
					},
					{
						key: "X-Frame-Options",
						value: "SAMEORIGIN"
					},
					{
						key: "X-Content-Type-Options",
						value: "nosniff"
					},
					{
						key: "Referrer-Policy",
						value: "origin-when-cross-origin"
					},
					{
						key: "X-DNS-Prefetch-Control",
						value: "on"
					}
				]
			}
		];
	}
};

module.exports = nextConfig;
