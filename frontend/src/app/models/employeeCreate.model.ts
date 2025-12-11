import { EmployeePhone } from './employeePhone.model';

export interface EmployeeCreate {
  firstName: string;      
  lastName: string;
  email: string;
  documentNumber: string;
  birthDate: string;    
  phones: EmployeePhone[];
  managerName: string;
  role: string;
  password: string;
}