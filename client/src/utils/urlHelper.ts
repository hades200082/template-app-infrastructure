
export function cleanUrl(u:string|URL):string{
	return new URL(u).toString();
}
export function removeStartingSlash(u:string|URL):string{
	const url= new URL(u);
	url.pathname.replace(/^\//, "");
	return url.toString();
}
