import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

export default function TechnicianDashboard() {
    const auth = useContext(AuthContext);
    const navigate = useNavigate();

    const handleLogout = () => {
        auth?.logout();
        navigate("/");
    };

    return (
        <div>
            <h1>Technician Dashboard</h1>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
}