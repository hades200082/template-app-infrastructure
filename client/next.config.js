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


// Injected content via Sentry wizard below

// eslint-disable-next-line @typescript-eslint/no-var-requires
const { withSentryConfig } = require("@sentry/nextjs");

module.exports = withSentryConfig(
	module.exports,
	{
		// For all available options, see:
		// https://github.com/getsentry/sentry-webpack-plugin#options

		// Suppresses source map uploading logs during build
		silent: false,
		org: "distinction-ds",
		project: "",
	},
	{
		// For all available options, see:
		// https://docs.sentry.io/platforms/javascript/guides/nextjs/manual-setup/

		// Upload a larger set of source maps for prettier stack traces (increases build time)
		widenClientFileUpload: true,

		// Transpiles SDK to be compatible with IE11 (increases bundle size)
		transpileClientSDK: true,

		// Routes browser requests to Sentry through a Next.js rewrite to circumvent ad-blockers. (increases server load)
		// Note: Check that the configured route will not match with your Next.js middleware, otherwise reporting of client-
		// side errors will fail.
		tunnelRoute: "/monitoring",

		// Hides source maps from generated client bundles
		hideSourceMaps: true,

		// Automatically tree-shake Sentry logger statements to reduce bundle size
		disableLogger: true,

		// Enables automatic instrumentation of Vercel Cron Monitors.
		// See the following for more information:
		// https://docs.sentry.io/product/crons/
		// https://vercel.com/docs/cron-jobs
		automaticVercelMonitors: true,
	}
);
