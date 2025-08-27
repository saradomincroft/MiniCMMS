// src/components/TaskList.tsx
import { useEffect, useState } from "react";
import { MaintenanceTask } from "../types";
import "../styles/TaskList.css";

interface TaskListProps {
  authToken: string;
}

export default function TaskList({ authToken }: TaskListProps) {
  const [tasks, setTasks] = useState<MaintenanceTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        const res = await fetch("https://localhost:7119/api/MaintenanceTasks/all", {
          headers: { Authorization: `Bearer ${authToken}` },
        });

        if (!res.ok) throw new Error(`Failed to fetch tasks: ${res.status}`);
        setTasks(await res.json());
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTasks();
  }, [authToken]);

  if (loading) return <p>Loading tasks...</p>;
  if (error) return <p>Error: {error}</p>;
  if (tasks.length === 0) return <p>No tasks found.</p>;

  return (
    <section className="task-list-section">
      <h2>Tasks</h2>
      <div className="cards-container">
        {tasks.map(task => (
          <div key={task.id} className="card task-card">
            <h3>{task.description}</h3>
            <p><strong>Priority:</strong> {task.priority}</p>
            <p><strong>Scheduled:</strong> {task.scheduledDate ? new Date(task.scheduledDate).toLocaleDateString() : "N/A"}</p>
            <p><strong>Asset:</strong> {task.asset?.name || "N/A"}</p>
            <p><strong>Assigned:</strong> {task.technicians?.length ? task.technicians.map(t => `${t.firstName} ${t.lastName}`).join(", ") : "Unassigned"}</p>
          </div>
        ))}
      </div>
    </section>
  );
}
