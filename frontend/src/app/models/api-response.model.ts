import { Employee } from "./employee.model";

export interface ApiResponse<T> {
  employees?: Employee[];
  success: boolean;
  errorCode: string | null;
  errorMessage: string | null;
}