import { Asset } from "./Asset";
import { Manager } from "./Manager";
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
}