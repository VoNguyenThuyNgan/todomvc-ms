import { Reminder } from './models/reminder.model';

export interface RemindersState {
  pending: Reminder[];
  connected: boolean;
  loading: boolean;
  error?: string;
}