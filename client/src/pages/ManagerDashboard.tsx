import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import TaskForm from "../components/TaskForm";
import { Asset, Technician, MaintenanceTask } from "../types";

export default function ManagerDashboard() {
  const auth = useContext(AuthContext);
  const [assets, setAssets] = useState<Asset[]>([]);
  const [technicians, setTechnicians] = useState<Technician[]>([]);
  const [tasks, setTasks] = useState<MaintenanceTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!auth?.token) {
      setError("No auth token, please log in");
      setLoading(false);
      return;
    }

    try {
      // Fetch assets, technicians, and tasks concurrently
      const [assetsRes, techRes, tasksRes] = await Promise.all([
        fetch("https://localhost:7119/api/Assets", {
          headers: { Authorization: `Bearer ${auth.token}` },
        }),
        fetch("https://localhost:7119/api/User/technicians", {
          headers: { Authorization: `Bearer ${auth.token}` },
        }),
        fetch("https://localhost:7119/api/MaintenanceTasks/all", {
          headers: { Authorization: `Bearer ${auth.token}` },
        }),
      ]);

      if (!assetsRes.ok) throw new Error(`Failed to fetch assets: ${assetsRes.status}`);
      if (!techRes.ok) throw new Error(`Failed to fetch technicians: ${techRes.status}`);
      if (!tasksRes.ok) throw new Error(`Failed to fetch tasks: ${tasksRes.status}`);

      const assetsData: Asset[] = await assetsRes.json();
      const techData: Technician[] = await techRes.json();
      const tasksData: MaintenanceTask[] = await tasksRes.json();

      setAssets(assetsData);
      setTechnicians(techData);
      setTasks(tasksData);
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

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div>
      <h1>Manager Dashboard</h1>
      <button onClick={auth?.logout}>Logout</button>

      <h2>Create New Task</h2>
      <TaskForm assets={assets} technicians={technicians} setTasks={setTasks} auth={auth} />

      <h2>Assets</h2>
      {assets.length === 0 ? (
        <p>No assets found.</p>
      ) : (
        <ul>
          {assets.map(a => (
            <li key={a.id}>
              {a.name} - {a.mainLocation} / {a.subLocation}
            </li>
          ))}
        </ul>
      )}

      <h2>Technicians</h2>
      {technicians.length === 0 ? (
        <p>No technicians found.</p>
      ) : (
        <ul>
          {technicians.map(t => (
            <li key={t.id}>
              {t.firstName} {t.lastName} ({t.username})
            </li>
          ))}
        </ul>
      )}

      <h2>Tasks</h2>
      {tasks.length === 0 ? (
        <p>No tasks found.</p>
      ) : (
        <ul>
          {tasks.map(task => {
            const assetName = task.asset?.name || "N/A";
            const assignedTechs =
              task.technicians?.length > 0
                ? task.technicians.map(t => `${t.firstName} ${t.lastName}`).join(", ")
                : "Unassigned";

            return (
              <li key={task.id}>
                <strong>{task.description}</strong> | Priority: {task.priority} | Scheduled:{" "}
                {task.scheduledDate ? new Date(task.scheduledDate).toLocaleDateString() : "N/A"} | Asset: {assetName} | Assigned: {assignedTechs}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}
