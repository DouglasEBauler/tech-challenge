import { EmployeePhone } from "./employeePhone.model";

export interface Employee {
  employeeId: number;
  firstName: string;      
  lastName: string;
  email: string;
  birthDate: string;    
  phones: EmployeePhone[];
  managerName: string;
  role: string;
  documentNumber: string;
}