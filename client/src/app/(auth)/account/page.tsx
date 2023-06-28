import LogoutButton from "./LogoutButton";

export default async function Account() {
	return (
		<>
			<h1>Account</h1>
			<p>
				To view this page you need to be logged in.<br />
			</p>
			<LogoutButton />
		</>
	);
}
