import { User } from "./User";
import { MaintenanceTask } from "./MaintenanceTask";
export interface Manager extends User {
  tasksCreated: MaintenanceTask[];
}