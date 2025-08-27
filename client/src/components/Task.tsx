// src/components/Task.tsx
import { useState } from "react";
import { MaintenanceTask } from "../types";
import "../styles/Task.css";

interface TaskProps {
  task: MaintenanceTask;
  authToken: string;
  onUpdate: (updatedTask: MaintenanceTask) => void;
  onDelete: (deletedTaskId: number) => void;
}

export default function Task({ task, authToken, onUpdate, onDelete }: TaskProps) {
  const [description, setDescription] = useState(task.description);
  const [priority, setPriority] = useState(task.priority);
  const [scheduledDate, setScheduledDate] = useState(task.scheduledDate?.slice(0, 10) ?? "");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleUpdate = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await fetch(`https://localhost:7119/api/MaintenanceTasks/${task.id}`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${authToken}`,
        },
        body: JSON.stringify({ description, priority, scheduledDate }),
      });

      if (!res.ok) throw new Error("Failed to update task");
      const updatedTask: MaintenanceTask = await res.json();
      onUpdate(updatedTask);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await fetch(`https://localhost:7119/api/MaintenanceTasks/${task.id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${authToken}` },
      });
      if (!res.ok) throw new Error("Failed to delete task");
      onDelete(task.id);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="task-detail-card">
      <h3>Task Detail</h3>
      {error && <p className="error">{error}</p>}
      <label>Description:</label>
      <input value={description} onChange={e => setDescription(e.target.value)} />
      
      <label>Priority:</label>
      <select value={priority} onChange={e => setPriority(e.target.value as "Low"|"Medium"|"High")}>
        <option>Low</option>
        <option>Medium</option>
        <option>High</option>
      </select>

      <label>Scheduled Date:</label>
      <input type="date" value={scheduledDate} onChange={e => setScheduledDate(e.target.value)} />

      <div className="task-buttons">
        <button onClick={handleUpdate} disabled={loading}>Update</button>
        <button onClick={handleDelete} disabled={loading}>Delete</button>
      </div>
    </div>
  );
}
