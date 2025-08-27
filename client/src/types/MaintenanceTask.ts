import { Asset } from "./Asset";
import { Manager } from "./Manager";
import { Technician } from "./Technician";
export interface MaintenanceTask {
  id: number;
  description: string;
  createdAt: string;
  scheduledDate?: string;
  priority: "Low" | "Medium" | "High";
  isCompleted: boolean;
  completedDate?: string;
  assetId: number;
  asset?: Asset;
  createdById: number;
  createdBy: Manager;
  technicians: { firstName: string; lastName: string }[];
}