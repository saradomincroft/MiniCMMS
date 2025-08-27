// src/components/Task.tsx
import { useState } from "react";
import { MaintenanceTask } from "../types";
import "../styles/Task.css";

interface TaskProps {
  task: MaintenanceTask;
  authToken: string;
  onDelete: (id: number) => void;
  onUpdate: (updatedTask: MaintenanceTask) => void;
  onBack: () => void;
}

export default function Task({ task, authToken, onDelete, onUpdate, onBack }: TaskProps) {
  const [priority, setPriority] = useState(task.priority);
  const [scheduledDate, setScheduledDate] = useState(task.scheduledDate || "");
  const [error, setError] = useState("");

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete this task?")) return;

    const res = await fetch(`https://localhost:7119/api/MaintenanceTasks/${task.id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${authToken}` },
    });

    if (res.ok) onDelete(task.id);
  };

  const handleSave = async () => {
    setError("");
    try {
      const dto = { 
        priority, 
        scheduledDate: scheduledDate || undefined 
      };

      const res = await fetch(`https://localhost:7119/api/MaintenanceTasks/${task.id}`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${authToken}`,
        },
        body: JSON.stringify(dto),
      });

      if (!res.ok) throw new Error("Failed to update task");

      const updatedTask = await res.json();
      onUpdate(updatedTask);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="task-container">
      <button className="back-btn" onClick={onBack}>← Back to Tasks</button>

      <h2>Edit Task: {task.description}</h2>

      <div className="task-edit-field">
        <label>Priority:</label>
        <div className="priority-options">
          {["Low", "Medium", "High"].map(level => (
            <label key={level}>
              <input
                type="radio"
                name="priority"
                value={level}
                checked={priority === level}
                onChange={() => setPriority(level as "Low" | "Medium" | "High")}
              />
              {level}
            </label>
          ))}
        </div>
      </div>

      <div className="task-edit-field">
        <label>Scheduled Date:</label>
        <input
          type="date"
          value={scheduledDate}
          onChange={e => setScheduledDate(e.target.value)}
        />
      </div>

      <div className="task-details">
        <p><strong>Asset:</strong> {task.assetName} ({task.assetMainLocation}/{task.assetSubLocation})</p>
        <p><strong>Assigned:</strong> {task.technicians?.map(t => `${t.firstName} ${t.lastName}`).join(", ") || "Unassigned"}</p>
      </div>

      {error && <p className="task-error">{error}</p>}

      <div className="task-actions">
        <button className="save-btn" onClick={handleSave}>Save Changes</button>
        <button className="delete-btn" onClick={handleDelete}>Delete Task</button>
      </div>
    </div>
  );
}
