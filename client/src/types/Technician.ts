import { User } from "./User";
import { MaintenanceTask } from "./MaintenanceTask";
export interface Technician extends User {
  assignedTasks: MaintenanceTask[];
}