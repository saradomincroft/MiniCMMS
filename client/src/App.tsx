// src/App.tsx
import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import { useContext } from "react";
import { AuthContext } from "./context/AuthContext";

export default function App() {
  const auth = useContext(AuthContext);

  return (
    <Routes>
      {/* If user is logged in, go to Dashboard, otherwise show Login */}
      <Route
        path="/"
        element={auth?.user ? <Navigate to="/dashboard" /> : <Login />}
      />
      <Route
        path="/dashboard"
        element={auth?.user ? <Dashboard /> : <Navigate to="/" />}
      />
    </Routes>
  );
}
