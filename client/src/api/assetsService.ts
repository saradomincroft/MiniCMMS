// api/authService.ts
import { User } from "../types/User";

const API_URL = "https://localhost:7119/api";

export const login = async (username: string, password: string) => {
  const res = await fetch(`${API_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  if (!res.ok) throw new Error("Login failed");
  return res.json() as Promise<{ token: string; user: User }>;
};

export const signup = async (userData: Partial<User>) => {
  const res = await fetch(`${API_URL}/auth/signup`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
  });
  if (!res.ok) throw new Error("Signup failed");
  return res.json() as Promise<User>;
};
