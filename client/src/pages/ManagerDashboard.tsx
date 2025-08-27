import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import TaskForm from "../components/TaskForm";
import Task from "../components/Task";
import { Asset, Technician, MaintenanceTask } from "../types";
import "../styles/ManagerDashboard.css";

export default function ManagerDashboard() {
  const auth = useContext(AuthContext);
  const navigate = useNavigate();
  const [assets, setAssets] = useState<Asset[]>([]);
  const [technicians, setTechnicians] = useState<Technician[]>([]);
  const [tasks, setTasks] = useState<MaintenanceTask[]>([]);
  const [selectedTask, setSelectedTask] = useState<MaintenanceTask | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!auth?.token) {
      navigate("/");
      return;
    }

    try {
      const [assetsRes, techRes, tasksRes] = await Promise.all([
        fetch("https://localhost:7119/api/Assets", { headers: { Authorization: `Bearer ${auth.token}` } }),
        fetch("https://localhost:7119/api/User/technicians", { headers: { Authorization: `Bearer ${auth.token}` } }),
        fetch("https://localhost:7119/api/MaintenanceTasks/all", { headers: { Authorization: `Bearer ${auth.token}` } }),
      ]);

      if (!assetsRes.ok) throw new Error(`Failed to fetch assets: ${assetsRes.status}`);
      if (!techRes.ok) throw new Error(`Failed to fetch technicians: ${techRes.status}`);
      if (!tasksRes.ok) throw new Error(`Failed to fetch tasks: ${tasksRes.status}`);

      setAssets(await assetsRes.json());
      setTechnicians(await techRes.json());
      setTasks(await tasksRes.json());
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

  // If a task is selected, show Task edit component
  if (selectedTask) {
    return (
      <Task
        task={selectedTask}
        authToken={auth?.token ?? ""}
        onDelete={(id) => {
          setTasks(prev => prev.filter(t => t.id !== id));
          setSelectedTask(null);
        }}
        onUpdate={(updatedTask) => {
          setTasks(prev => prev.map(t => t.id === updatedTask.id ? updatedTask : t));
          setSelectedTask(null);
        }}
        onBack={() => setSelectedTask(null)}
      />
    );
  }

  return (
    <div className="manager-dashboard">
      <header className="dashboard-header">
        <h1>Manager Dashboard</h1>
        <button className="logout-button" onClick={auth?.logout}>Logout</button>
      </header>

      <section className="dashboard-section">
        <h2>Create New Task</h2>
        <TaskForm
          assets={assets}
          technicians={technicians}
          auth={auth}
          setTasks={(newTasks) => setTasks(newTasks)} // update task list automatically
        />
      </section>

      <section className="dashboard-section">
        <h2>Tasks</h2>
        <div className="cards-container">
          {tasks.length === 0 ? (
            <p>No tasks found.</p>
          ) : (
            tasks.map(task => (
              <div
                key={task.id}
                className="card task-card"
                onClick={() => setSelectedTask(task)}
              >
                <h3>{task.description}</h3>
                <p><strong>Priority:</strong> {task.priority}</p>
                <p><strong>Scheduled:</strong> {task.scheduledDate ? new Date(task.scheduledDate).toLocaleDateString() : "N/A"}</p>
                <p>
                  <strong>Asset:</strong> {task.assetName} ({task.assetMainLocation}/{task.assetSubLocation})
                </p>
                <p>
                  <strong>Assigned:</strong> {task.technicians?.map(t => `${t.firstName} ${t.lastName}`).join(", ") || "Unassigned"}
                </p>
              </div>
            ))
          )}
        </div>
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
