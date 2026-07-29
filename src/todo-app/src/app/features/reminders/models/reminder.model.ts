import { ReminderState } from './reminder-state.enum';

export interface Reminder {
  id: string;
  todoId: string;
  todoTitle: string;
  dueAt: string;
  state: ReminderState;
  snoozeUntil: string | null;
  firedAt: string;
}