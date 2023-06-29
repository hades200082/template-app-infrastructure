import { usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import { z } from "zod";

declare global {
  interface Window {
    dataLayer: Record<string, any>[];
  }
}

const GtmDataLayerObjectSchema = z.object({
	event: z.string(),
	pagePath: z.string().optional()
}).catchall(z.string());

type GtmDataLayerObject = z.infer<typeof GtmDataLayerObjectSchema>;

export default function useGtm() {
	const pathname = usePathname();
	const [dataLayer, setDataLayer] = useState<GtmDataLayerObject|null>(null);

	useEffect(() => {
		if(!dataLayer || typeof window === "undefined") return;

		if(window.dataLayer && Array.isArray(window.dataLayer) && dataLayer) {
			window.dataLayer.push(dataLayer)
		}
	}, [dataLayer]);

	function push(dataLayerObject: GtmDataLayerObject) {
		const parsedObject = GtmDataLayerObjectSchema.parse(dataLayerObject);

		if(!parsedObject.pagePath && pathname)
			parsedObject.pagePath = pathname;

		setDataLayer(parsedObject);
	}
	
	return [useGtm];
}
