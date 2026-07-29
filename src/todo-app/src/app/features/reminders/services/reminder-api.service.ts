import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/environment';
import { Reminder } from '../models/reminder.model';
import { ReminderState } from '../models/reminder-state.enum';
import { SnoozeReminderRequest } from '../dtos/snooze-reminder.request';
import { UpcomingTodo } from '../models/upcoming-todo.model';

@Injectable({
  providedIn: 'root',
})
export class ReminderApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiBaseUrl}/reminders`;

  getReminders(state?: ReminderState): Observable<Reminder[]> {
    if (state === undefined) {
      return this.http.get<Reminder[]>(this.baseUrl);
    }

    return this.http.get<Reminder[]>(`${this.baseUrl}?state=${state}`);
  }

  getUpcomingReminders(within: string = '24h'): Observable<UpcomingTodo[]> {
    return this.http.get<UpcomingTodo[]>(`${this.baseUrl}/upcoming?within=${within}`);
  }

  snoozeReminder(id: string, request: SnoozeReminderRequest): Observable<Reminder> {
    return this.http.patch<Reminder>(`${this.baseUrl}/${id}/snooze`, request);
  }

  dismissReminder(id: string): Observable<Reminder> {
    return this.http.patch<Reminder>(`${this.baseUrl}/${id}/dismiss`, null);
  }
}
