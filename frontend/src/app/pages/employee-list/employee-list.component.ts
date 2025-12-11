import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})

export class EmployeeListComponent implements OnInit {
  employees: Employee[] = [];
  isLoading = true;
  errorMessage: string | null = null;

  constructor(
        private employeeService: EmployeeService,
        private cd: ChangeDetectorRef
    ) { }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.employeeService.getAll().subscribe({
      next: (data) => {
        this.employees = data;
        this.isLoading = false;
        
        this.cd.detectChanges(); 
    },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load employees.';
        
        this.cd.detectChanges();
      }
    });
  }
  
onDelete(id: number, name: string): void {
    if (confirm(`Are you sure in delete ${name} (ID: ${id})?`)) {
      this.employeeService.delete(id).subscribe({
        next: () => {
          this.loadEmployees(); 
        },
        error: (error) => {
          this.errorMessage = `Deleted error ${name}.`;
          this.cd.detectChanges();
        }
      });
    }
}
}