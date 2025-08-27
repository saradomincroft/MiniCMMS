import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import ManagerDashboard from "./ManagerDashboard";
import TechnicianDashboard from "./TechnicianDashboard";

export default function Dashboard() {
  const auth = useContext(AuthContext);

  if (!auth?.user) return <p>Loading...</p>;

return auth.user.role === "Manager" 
  ? <ManagerDashboard /> 
  : <TechnicianDashboard />;}
