import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/environment';

import {
  Reminder,
  ReminderState,
  SnoozeReminderRequest,
  UpcomingTodo,
} from '../models/reminder.model';

@Injectable({
  providedIn: 'root',
})
export class ReminderApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiBaseUrl}/bff/todos`;

  getReminders(
    state: ReminderState = 'Pending',
  ): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(
      `${this.baseUrl}?state=${state}`,
    );
  }

  getUpcomingReminders(
    within: string = '24h',
  ): Observable<UpcomingTodo[]> {
    return this.http.get<UpcomingTodo[]>(
      `${this.baseUrl}/upcoming?within=${within}`,
    );
  }

  snoozeReminder(
    id: string,
    request: SnoozeReminderRequest,
  ): Observable<Reminder> {
    return this.http.patch<Reminder>(
      `${this.baseUrl}/${id}/snooze`,
      request,
    );
  }

  dismissReminder(id: string): Observable<Reminder> {
    return this.http.patch<Reminder>(
      `${this.baseUrl}/${id}/dismiss`,
      {},
    );
  }
}