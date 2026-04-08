import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Task, TaskApiService } from '../../../api-client';

@Component({
  selector: 'app-user-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data?.id ? 'Edit Task' : 'New Task' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()">
      <mat-dialog-content>
        <div>
          <mat-form-field appearance="fill" style="width: 100%;">
            <mat-label>Name</mat-label>
            <input matInput formControlName="name" placeholder="Name">
          </mat-form-field>
        </div>
        <div>
          <mat-form-field appearance="fill" style="width: 100%;">
            <mat-label>Description</mat-label>
            <input matInput formControlName="description" placeholder="Description">
          </mat-form-field>
        </div>
        <div>
          <mat-checkbox formControlName="finished">Finished</mat-checkbox>
        </div>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="dialogRef.close()">Cancel</button>
        <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid">Save</button>
      </mat-dialog-actions>
    </form>
    <div *ngIf="errors.length" style="color: #f44336; font-size: 12px; padding: 8px 24px 16px;">
      <ul style="margin: 0; padding-left: 1.25em;">
        <li *ngFor="let err of errors">{{ err }}</li>
      </ul>
    </div>
  `
})
export class UserEditDialog {
  
  private fb = inject(FormBuilder);
  private api = inject(TaskApiService);
  public dialogRef = inject(MatDialogRef<UserEditDialog>);
  public data = inject<Task | undefined>(MAT_DIALOG_DATA);
  
  errors: string[] = [];

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      id: [null as number | null],
      name: ['', [Validators.required]],
      description: ['', []],
      finished: [false]
    });
    this.form.patchValue(this.data ?? {});
  }

  submit(): void {
    this.errors = [];
    const payload = this.form.getRawValue() as Task;

    const request$ =
      payload.id != null ? this.api.apiTasksPut(payload) : this.api.apiTasksPost({name: payload.name, description: payload.description, finished: payload.finished});

    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        if (err.status !== 400) throw err;
        this.errors = Object.values(err.error?.errors ?? {}).flat() as string[];
      }
    });
  }
}
