import { Reminder } from '../models/reminder.model';
import { UpcomingTodo } from '../models/upcoming-todo.model';

export interface RemindersState {
  pending: Reminder[];
  upcoming: UpcomingTodo[];
  loading: boolean;
  connected: boolean;
  error?: string;
}