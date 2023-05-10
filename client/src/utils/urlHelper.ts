
export function removeTrailingSlash(u:string|URL):string{
	const url= new URL(u);
	url.pathname.replace(/\/$/, "");
	return url.toString();
}
export function removeStartingSlash(u:string|URL):string{
	const url= new URL(u);
	url.pathname.replace(/^\//, "");
	return url.toString();
}
