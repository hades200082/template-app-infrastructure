"use client";

import { SessionProvider } from "next-auth/react";
import { ThemeProvider, useTheme } from "next-themes";

type Props = {
	children?: React.ReactNode;
};

export const Providers = ({ children }: Props) => {
	return (
		<ThemeProvider attribute="class">
			<SessionProvider>
				{children}
			</SessionProvider>
		</ThemeProvider>
	);
};
