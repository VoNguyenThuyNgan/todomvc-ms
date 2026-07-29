import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RemindersStore } from '../../store/reminders.store';

@Component({
  selector: 'app-reminders-panel-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reminders-panel-component.html',
  styleUrl: './reminders-panel-component.scss',
})
export class RemindersPanelComponent {
private readonly store = inject(RemindersStore);

  readonly pending$ = this.store.pending$;

  constructor() {
    this.store.loadReminders();
  }

  snooze10m(id: string) {
    this.store.snoozeReminder({
      id,
      request: {
        minutes: 10,
      },
    });
  }

  snooze1h(id: string) {
    this.store.snoozeReminder({
      id,
      request: {
        minutes: 60,
      },
    });
  }

  dismiss(id: string) {
    this.store.dismissReminder(id);
  }
}
