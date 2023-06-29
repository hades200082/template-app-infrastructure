import Script from "next/script";
import { z } from "zod";

const PropsSchema = z.object({
	Id: z.string()
})

type Props = z.infer<typeof PropsSchema>;

export default function GoogleTagManager({ Id }: Props) {
	return (
		<>
			<Script 
				id="GTM"
				strategy="beforeInteractive"
				dangerouslySetInnerHTML={{
					__html: `
						window.dataLayer = window.dataLayer || [];
						(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
						new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
						j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
						'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
						})(window,document,'script','dataLayer','${Id}');
					`
				}}
			></Script>
			<noscript>
				<iframe src={`https://www.googletagmanager.com/ns.html?id=${Id}`} height="0" width="0" style={{display:"none",visibility:"hidden"}}></iframe>
			</noscript>
		</>
	)
}
