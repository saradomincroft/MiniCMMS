import { User } from "../types/User";

export async function login(identifier: string, password: string): Promise<{ token: string; user: User }> {
  const res = await fetch("https://localhost:7119/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ identifier, password }),
  });

  if (!res.ok) {
    const text = await res.text();
    console.error("Login failed:", text);
    throw new Error("Login failed");
  }

  const data = await res.json();

  const user: User = {
    id: Number(data.id ?? 0),
    firstName: data.firstName ?? "",
    lastName: data.lastName ?? "",
    username: data.username ?? "",
    email: data.email ?? "",
    role: data.role === "Manager" ? "Manager" : "Technician",
  };

  return { token: data.token, user };
}
