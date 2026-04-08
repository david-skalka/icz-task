import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FormsModule, NgForm } from '@angular/forms';
import { Task, TaskApiService } from '../../../api-client';
import { throwError } from 'rxjs';

@Component({
  selector: 'app-user-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCheckboxModule,
    FormsModule
  ],
  template: `
<h2 mat-dialog-title>{{ data.user.id ? 'Edit Task' : 'New Task' }}</h2>

<form #form="ngForm" (ngSubmit)="save(form)">
<mat-dialog-content>
  <div>
    <mat-form-field appearance="fill" style="width: 100%;">
      <mat-label>Name</mat-label>
      <input matInput [(ngModel)]="data.user.name" name="name">
    </mat-form-field>
  </div>

  <div>
    <mat-form-field appearance="fill" style="width: 100%;">
      <mat-label>Description</mat-label>
      <input matInput [(ngModel)]="data.user.description" name="description">
    </mat-form-field>
  </div>

  <div>
    <mat-checkbox [(ngModel)]="data.user.finished" name="finished">
      Finished
    </mat-checkbox>
  </div>

</mat-dialog-content>
<mat-dialog-actions align="end">
    <button mat-button type="button" (click)="cancel()">Cancel</button>
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
  errors: string[] = [];

  private api = inject(TaskApiService);
  private dialogRef = inject(MatDialogRef);
  data = inject<{ user: Task }>(MAT_DIALOG_DATA);

  save(form: NgForm) {
    if (form.invalid) return;
    this.errors = [];
    const user = this.data.user;
    const payload: Task = {
      name: user.name,
      description: user.description,
      finished: user.finished ?? false
    };
    if (user.id != null) {
      payload.id = user.id;
    }
    const request =
      user.id != null
        ? this.api.apiTasksPut(payload)
        : this.api.apiTasksPost(payload);
    request.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        if (err.status === 400) {
          const errorDetails = err.error?.errors;
          this.errors = Object.values(errorDetails).flat() as string[];
          
        } else {
          throw err;
        }
      }
    });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
