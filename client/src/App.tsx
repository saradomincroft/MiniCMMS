// src/App.tsx
import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import ManagerDashboard from "./pages/ManagerDashboard";
import TechnicianDashboard from "./pages/TechnicianDashboard";
import { useContext } from "react";
import { AuthContext } from "./context/AuthContext";

export default function App() {
  const auth = useContext(AuthContext);

  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/manager-dashboard" element={<ManagerDashboard /> } />
      <Route path="/technician-dashboard" element={<TechnicianDashboard />} />
    </Routes>
  );
}
