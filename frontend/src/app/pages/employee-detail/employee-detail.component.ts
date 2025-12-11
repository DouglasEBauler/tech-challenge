import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './employee-detail.component.html',
  styleUrls: ['./employee-detail.component.scss']
})
export class EmployeeDetailComponent implements OnInit {
  employee: Employee | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const employeeId = this.route.snapshot.paramMap.get('id');

    if (employeeId) {
      this.loadEmployeeDetails(Number(employeeId));
    } else {
      this.errorMessage = 'Employee id is required.';
      this.isLoading = false;
    }
  }

  loadEmployeeDetails(id: number): void {
    this.isLoading = true;
    this.employeeService.getById(id).subscribe({
      next: (data) => {
        this.employee = data;
        this.isLoading = false;
        this.cd.detectChanges(); 
      },
      error: (error) => {
        console.error('Failed to load detail employee:', error);
        this.errorMessage = 'Do not possible loading detail employee.';
        this.isLoading = false;
        this.cd.detectChanges(); 
      }
    });
  }
}