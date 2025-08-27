// pages/Login.tsx
import { useState, useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { login as loginApi } from "../api/authService";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const auth = useContext(AuthContext);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const { token, user } = await loginApi(username, password);
      auth?.login(user, token);
      alert("Logged in!");
    } catch (err) {
      alert("Login failed");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" />
      <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
      <button type="submit">Login</button>
    </form>
  );
}
