import { Employee } from "./employee.model";

export interface ApiResponse<T> {
  employes?: Employee[];
  success: boolean;
  errorCode: string | null;
  errorMessage: string | null;
}