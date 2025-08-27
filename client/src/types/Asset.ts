import { MaintenanceTask } from "./MaintenanceTask";
export interface Asset {
  id: number;
  name: string;
  mainLocation: string;
  subLocation: string;
  category: string;
  lastMaintained: string;
  maintenanceTasks: MaintenanceTask[];
}