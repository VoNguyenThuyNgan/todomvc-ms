import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RemindersStore } from '../../store/reminders.store';

@Component({
  selector: 'app-notification-bell-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell-component.html',
  styleUrl: './notification-bell-component.scss',
})
export class NotificationBellComponent {
  private readonly store = inject(RemindersStore);
  readonly pendingCount$ = this.store.pendingCount$;
}
