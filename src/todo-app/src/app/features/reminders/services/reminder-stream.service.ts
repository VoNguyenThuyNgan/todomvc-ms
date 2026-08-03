import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/environment';
import { Reminder } from '../models/reminder.model';

@Injectable({
  providedIn: 'root',
})
export class ReminderStreamService {

  private readonly url =
    `${environment.apiBaseUrl}/reminders/stream`;

  connect(): Observable<Reminder> {

    return new Observable<Reminder>(observer => {

      console.log('[SSE] Connecting...');

      const es = new EventSource(this.url);

      es.onopen = () => {
        console.log('[SSE] Connected');
      };

      es.addEventListener(
        'reminder-fired',
        (event: MessageEvent) => {

          console.log('[SSE] reminder-fired');
          console.log(event.data);

          const reminder =
            JSON.parse(event.data) as Reminder;

          observer.next(reminder);
        });

      es.onerror = err => {

        console.error('[SSE] Error');

        console.error(err);

        observer.error(err);

        es.close();
      };

      return () => {

        console.log('[SSE] Closed');

        es.close();

      };

    });

  }

}