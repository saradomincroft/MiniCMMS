import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import { login as loginApi } from "../api/authService";
import { User } from "../types/User";

export default function Login() {
  const [identifier, setIdentifier] = useState("");
  const [password, setPassword] = useState("");
  const auth = useContext(AuthContext);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const { token, user } = await loginApi(identifier, password);

      const roleUser: User = {
        ...user,
        role: user.role === "Manager" ? "Manager" : "Technician",
      };

      auth?.login(roleUser, token);

      if (roleUser.role === "Manager") {
        navigate("/manager-dashboard");
      } else {
        navigate("/technician-dashboard");
      }
    } catch (err) {
      alert("Login failed");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        value={identifier}
        onChange={(e) => setIdentifier(e.target.value)}
        placeholder="Username"
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
      />
      <button type="submit">Login</button>
    </form>
  );
}
