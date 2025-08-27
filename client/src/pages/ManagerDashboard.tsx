import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import TaskForm from "../components/TaskForm";
import TaskList from "../components/TaskList";
import { Asset, Technician, MaintenanceTask } from "../types";
import "../styles/ManagerDashboard.css";

export default function ManagerDashboard() {
  const auth = useContext(AuthContext);
  const navigate = useNavigate();
  const [assets, setAssets] = useState<Asset[]>([]);
  const [technicians, setTechnicians] = useState<Technician[]>([]);
  const [tasks, setTasks] = useState<MaintenanceTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!auth?.token) {
        navigate("/");
        return;
    }

    
    try {
      const [assetsRes, techRes] = await Promise.all([
        fetch("https://localhost:7119/api/Assets", { headers: { Authorization: `Bearer ${auth.token}` } }),
        fetch("https://localhost:7119/api/User/technicians", { headers: { Authorization: `Bearer ${auth.token}` } }),
      ]);

      if (!assetsRes.ok) throw new Error(`Failed to fetch assets: ${assetsRes.status}`);
      if (!techRes.ok) throw new Error(`Failed to fetch technicians: ${techRes.status}`);

      setAssets(await assetsRes.json());
      setTechnicians(await techRes.json());
    } catch (err: any) {
      console.error(err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [auth]);

  if (loading) return <p className="loading">Loading...</p>;
  if (error) return <p className="error">Error: {error}</p>;

  return (
    <div className="manager-dashboard">
      <header className="dashboard-header">
        <h1>Manager Dashboard</h1>
        <button className="logout-button" onClick={auth?.logout}>Logout</button>
      </header>

      <section className="dashboard-section">
        <h2>Create New Task</h2>
        <TaskForm assets={assets} technicians={technicians} setTasks={setTasks} auth={auth} />
      </section>


      <section className="dashboard-section">
            <TaskList authToken={auth?.token ?? ""} />
      </section>

      <section className="dashboard-section">
        <h2>Assets</h2>
        <div className="cards-container">
          {assets.length === 0 ? <p>No assets found.</p> : assets.map(a => (
            <div key={a.id} className="card">
              <h3>{a.name}</h3>
              <p>{a.mainLocation} / {a.subLocation}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="dashboard-section">
        <h2>Technicians</h2>
        <div className="cards-container">
          {technicians.length === 0 ? <p>No technicians found.</p> : technicians.map(t => (
            <div key={t.id} className="card">
              <h3>{t.firstName} {t.lastName}</h3>
              <p>Username: {t.username}</p>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}
