import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../services/employee.service';
import { EmployeeCreate } from '../../models/employeeCreate.model';
import { EmployeeUpdate } from '../../models/employeeUpdate.model';
import { Employee } from '../../models/employee.model';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employee-form.component.html',
  styleUrls: ['./employee-form.component.scss']
})
export class EmployeeFormComponent implements OnInit {
  employeeForm!: FormGroup;
  isEditMode = false;
  employeeId: number | null = null;
  pageTitle = 'Add New Employee';
  isLoading = false;
  errorMessage: string | null = null;
  
  phoneTypes = [
    { value: 1, label: 'Mobile' },
    { value: 2, label: 'Landline' },
    { value: 3, label: 'Work' },
    { value: 4, label: 'Home' },
    { value: 5, label: 'Fax' },
    { value: 6, label: 'Emergency' },
    { value: 7, label: 'Other' },
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    public router: Router,
    private employeeService: EmployeeService,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.employeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      documentNumber: ['', Validators.required], 
      role: ['', Validators.required],
      birthDate: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      phones: this.fb.array([]) 
    });
    
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.employeeId = Number(idParam);
      this.isEditMode = true;
      this.pageTitle = 'Edit employee';
      
      this.employeeForm.get('password')?.clearValidators();
      this.employeeForm.get('password')?.updateValueAndValidity();
      
      this.loadEmployee(this.employeeId);
    }
    
    if (!this.isEditMode) {
      this.addPhone();
    }
  }

  private createPhoneGroup(type: string = '', number: string = ''): FormGroup {
    return this.fb.group({
      type: [type, Validators.required],
      number: [number, Validators.required]
    });
  }
  
  addPhone(): void {
    this.phones.push(this.createPhoneGroup());
  }
  
  removePhone(index: number): void {
    this.phones.removeAt(index);
  }
  
  loadEmployee(id: number): void {
    this.isLoading = true;
    this.employeeService.getById(id).subscribe({
      next: (employee: Employee) => {
        const formattedBirthDate = employee.birthDate ? 
            new Date(employee.birthDate).toISOString().substring(0, 10) : '';

        this.employeeForm.patchValue({
          firstName: employee.firstName || '',
          lastName: employee.lastName || '',
          email: employee.email || '',
          documentNumber: employee.documentNumber || '',
          role: employee.role || '',
          birthDate: formattedBirthDate
        });
        
        this.phones.clear();
        
        if (employee.phones && employee.phones.length > 0) {
          employee.phones.forEach(phone => {
            this.phones.push(this.createPhoneGroup(phone.type, phone.number));
          });
        } else {
          this.addPhone();
        }
        
        this.isLoading = false;
        this.cd.detectChanges(); 
      },
      error: (error) => {
        console.error('Error loading employee data:', error);
        this.errorMessage = 'Error loading employee data.';
        this.isLoading = false;
        this.cd.detectChanges(); 
      }
    });
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched(); 
      return;
    }

    this.isLoading = true;
    
    if (!this.isEditMode) {
      const createData: EmployeeCreate = {
        firstName: this.employeeForm.value.firstName,
        lastName: this.employeeForm.value.lastName,
        email: this.employeeForm.value.email,
        documentNumber: this.employeeForm.value.documentNumber,
        role: this.employeeForm.value.role,
        birthDate: this.employeeForm.value.birthDate,
        password: this.employeeForm.value.password,
        phones: this.employeeForm.value.phones
      } as EmployeeCreate;
      
      this.employeeService.create(createData).subscribe({
        next: () => {
          alert('Employee created successfully!');
          this.isLoading = false;
          this.router.navigate(['/employees']); 
        },
        error: (error) => {
          console.error('Save error:', error);
          this.errorMessage = 'Error creating employee.';
          this.isLoading = false;
          this.cd.detectChanges(); 
        }
      });
    } else if (this.employeeId) {
      const updateData: EmployeeUpdate = {
        employeeId: this.employeeId,
        firstName: this.employeeForm.value.firstName,
        lastName: this.employeeForm.value.lastName,
        email: this.employeeForm.value.email,
        documentNumber: this.employeeForm.value.documentNumber,
        role: this.employeeForm.value.role,
        birthDate: this.employeeForm.value.birthDate,
        phones: this.employeeForm.value.phones
      } as EmployeeUpdate; 
      
      this.employeeService.update(this.employeeId, updateData).subscribe({
        next: () => {
          alert('Employee updated successfully!');
          this.isLoading = false;
          this.router.navigate(['/employees']); 
        },
        error: (error) => {
          console.error('Error updating:', error);
          this.errorMessage = 'Error updating employee.';
          this.isLoading = false;
          this.cd.detectChanges(); 
        }
      });
    }
  }

  get f() {
    return this.employeeForm.controls;
  }

  get phones(): FormArray {
    return this.employeeForm.get('phones') as FormArray;
  }
}