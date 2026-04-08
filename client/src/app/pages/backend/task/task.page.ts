import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { provideNativeDateAdapter } from '@angular/material/core';
import { Task, TaskApiService } from '../../../api-client';
import { ConfirmDialog } from '../confirm-dialog';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserEditDialog } from './task-edit-dialog';
import { MatMenuModule } from '@angular/material/menu';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-user-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDialogModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './task.page.html',
  styleUrl: './task.page.scss'
})
export class TaskPage implements OnInit {
  private api = inject(TaskApiService);
  private dialog = inject(MatDialog);
private router = inject(Router);
 private snackBar = inject(MatSnackBar);

  displayedColumns: string[] = ['id', 'name', 'description', 'finished', 'actions'];
  items: Task[] = [];
  nameFilter = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.api.apiTasksGet(this.nameFilter).subscribe(data => (this.items = data));
  }

  add() {
    const dialogRef = this.dialog.open(UserEditDialog, {
      data: { user: {} as Task }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadData();
    });
  }

  edit(task: Task) {
    const dialogRef = this.dialog.open(UserEditDialog, {
      data: { user: { ...task } }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadData();
    });
  }



 delete(user: Task) {
  const dialogRef = this.dialog.open(ConfirmDialog, {
    data: {
      title: 'Confirm Deletion',
      message: `Are you sure you want to delete task "${user.name}"?`
    }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.api.apiTasksIdDelete(user.id!).subscribe(() => this.loadData());
    }
  });

}





setting(userId: number) {
  this.router.navigate(['/backend/setting', userId]);
 }


}
