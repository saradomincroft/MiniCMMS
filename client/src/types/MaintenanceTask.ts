import { Asset } from "./Asset";
import { Manager } from "./Manager";
import { Technician } from "./Technician";

export interface TechnicianInTask {
  id: number;
  firstName: string;
  lastName: string;
}

export interface MaintenanceTask {
  id: number;
  description: string;
  createdAt: string;
  scheduledDate?: string;
  priority: "Low" | "Medium" | "High";
  isCompleted: boolean;
  completedDate?: string;
  assetId: number;
  assetName: string;
  assetMainLocation: string;
  assetSubLocation: string;

  createdById: number;
  createdByFirstName: string;
  createdByLastName: string;
  technicians: TechnicianInTask[];
}
