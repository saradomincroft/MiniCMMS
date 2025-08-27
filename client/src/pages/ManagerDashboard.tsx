import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

export default function ManagerDashboard() {
    const auth = useContext(AuthContext);
    const navigate = useNavigate();

    const handleLogout = () => {
        auth?.logout();
        navigate("/");
    };

    return (
        <div>
            <h1>Manager Dashboard</h1>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
}