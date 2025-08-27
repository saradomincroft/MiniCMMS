import { useState } from "react";
import { Asset, Technician, MaintenanceTask } from "../types";
import { AuthContext } from "../context/AuthContext";
import "../styles/TaskForm.css";

interface TaskFormProps {
  assets: Asset[];
  technicians: Technician[];
  setTasks: React.Dispatch<React.SetStateAction<MaintenanceTask[]>>;
  auth: React.ContextType<typeof AuthContext> | null;
}

export default function TaskForm({ assets, technicians, setTasks, auth }: TaskFormProps) {
  const [description, setDescription] = useState("");
  const [scheduledDate, setScheduledDate] = useState("");
  const [priority, setPriority] = useState<"Low" | "Medium" | "High">("Low");
  const [assetId, setAssetId] = useState<number | null>(null);
  const [assignedTechIds, setAssignedTechIds] = useState<number[]>([]);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!auth?.token || assetId === null) return;

    setError("");

const dto = {
  description,
  scheduledDate: scheduledDate || undefined,
  priority,
  assetId: assetId!,
  technicianIds: assignedTechIds,
};


    try {
      const res = await fetch("https://localhost:7119/api/MaintenanceTasks", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify(dto),
      });

      if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        throw new Error(data?.message || "Failed to create task");
      }

      const newTask: MaintenanceTask = await res.json();
      setTasks(prev => [...prev, newTask]);

      // Reset form
      setDescription("");
      setScheduledDate("");
      setPriority("Low");
      setAssetId(null);
      setAssignedTechIds([]);
    } catch (err: any) {
      setError(err.message);
      console.error(err);
    }
  };

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <input
        className="task-input"
        value={description}
        onChange={e => setDescription(e.target.value)}
        placeholder="Task description"
        required
      />

      <input
        className="task-input"
        type="date"
        value={scheduledDate}
        onChange={e => setScheduledDate(e.target.value)}
        required
      />

      <select
        className="task-select"
        value={priority}
        onChange={e => setPriority(e.target.value as "Low" | "Medium" | "High")}
      >
        <option>Low</option>
        <option>Medium</option>
        <option>High</option>
      </select>

      <select
        className="task-select"
        value={assetId ?? ""}
        onChange={e => setAssetId(Number(e.target.value))}
        required
      >
        <option value="">Select Asset</option>
        {assets.map(a => (
          <option key={a.id} value={a.id}>
            {a.name} ({a.mainLocation}/{a.subLocation})
          </option>
        ))}
      </select>

      <select
        className="task-select"
        multiple
        value={assignedTechIds.map(String)}
        onChange={e => {
          const selected = Array.from(e.target.selectedOptions, o => Number(o.value));
          setAssignedTechIds(selected);
        }}
      >
        {technicians.map(t => (
          <option key={t.id} value={t.id}>
            {t.firstName} {t.lastName}
          </option>
        ))}
      </select>

      {error && <p className="task-error">{error}</p>}

      <button className="task-button" type="submit" disabled={!description || !assetId}>
        Add Task
      </button>
    </form>
  );
}
