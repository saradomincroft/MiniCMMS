// src/components/TaskList.tsx
import { useEffect, useState } from "react";
import { MaintenanceTask } from "../types";
import Task from "./Task";
import "../styles/TaskList.css";

interface TaskListProps {
  authToken: string;
}

export default function TaskList({ authToken }: TaskListProps) {
  const [tasks, setTasks] = useState<MaintenanceTask[]>([]);
    const [selectedTask, setSelectedTask] = useState<MaintenanceTask | null>(null);
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

  if (selectedTask) {
    return (
      <Task
        task={selectedTask}
        authToken={authToken}
        onUpdate={updated => {
          setTasks(tasks.map(t => t.id === updated.id ? updated : t));
          setSelectedTask(null);
        }}
        onDelete={deletedId => {
          setTasks(tasks.filter(t => t.id !== deletedId));
          setSelectedTask(null);
        }}
      />
    );
  }

  return (
    <section className="task-list-section">
      <h2>Tasks</h2>
      <div className="cards-container">
        {tasks.map(task => (
          <div key={task.id} className="card task-card" onClick={() => setSelectedTask(task)}>
            <h3>{task.description}</h3>
            <p><strong>Priority:</strong> {task.priority}</p>
            <p><strong>Scheduled:</strong> {task.scheduledDate ? new Date(task.scheduledDate).toLocaleDateString() : "N/A"}</p>
          </div>
        ))}
      </div>
    </section>
  );
}