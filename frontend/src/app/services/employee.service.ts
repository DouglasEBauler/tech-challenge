import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Employee } from '../models/employee.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';
import { EmployeeCreate } from '../models/employeeCreate.model';
import { EmployeeUpdate } from '../models/employeeUpdate.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private apiUrl = `${environment.API_URL}/employee`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Employee[]> {
    return this.http.get<ApiResponse<Employee>>(this.apiUrl).pipe(
      map(response => {
        return response.employees || [];
      })
    );
  }

  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}/get`);
  }

  create(employee: EmployeeCreate): Observable<EmployeeCreate> {
    return this.http.post<EmployeeCreate>(`${this.apiUrl}/create`, employee);
  }

  update(id: number, employee: EmployeeUpdate): Observable<EmployeeUpdate> {
    return this.http.put<EmployeeUpdate>(`${this.apiUrl}/${id}/update`, employee);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/delete`);
  }
}