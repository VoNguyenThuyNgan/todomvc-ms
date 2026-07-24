export type ReminderState =
  | 'Pending'
  | 'Snoozed'
  | 'Dismissed';

export interface Reminder {
  id: string;
  todoId: string;
  dueAt: string;
  state: ReminderState;
  snoozeUntil: string | null;
  firedAt: string;
}

export interface SnoozeReminderRequest {
  minutes: number;
}

export interface UpcomingTodo {
  todoId: string;
  title: string;
  dueAt: string;
}