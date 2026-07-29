import { inject, Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { EMPTY, switchMap, tap } from 'rxjs';
import { handleEffect } from '../../../core/utils/effect.helper';
import { ReminderApiService } from '../services/reminder-api.service';
import { RemindersState } from '../state/reminders.state';
import { Reminder } from '../models/reminder.model';
import { UpcomingTodo } from '../models/upcoming-todo.model';
import { ReminderState } from '../models/reminder-state.enum';
import { SnoozeReminderRequest } from '../dtos/snooze-reminder.request';

const initialState: RemindersState = {
  pending: [],
  upcoming: [],
  loading: false,
  connected: false,
};

@Injectable()
export class RemindersStore extends ComponentStore<RemindersState> {
  private readonly reminderApi = inject(ReminderApiService);

  constructor() {
    super(initialState);
  }

  readonly pending$ = this.select((state) => state.pending);
  readonly upcoming$ = this.select((state) => state.upcoming);
  readonly loading$ = this.select((state) => state.loading);
  readonly connected$ = this.select((state) => state.connected);
  readonly pendingCount$ = this.select(this.pending$, (pending) => pending.length);

  readonly setPending = this.updater((state, pending: Reminder[]) => ({
    ...state,
    pending,
  }));

  readonly setUpcoming = this.updater((state, upcoming: UpcomingTodo[]) => ({
    ...state,
    upcoming,
  }));

  readonly setLoading = this.updater((state, loading: boolean) => ({
    ...state,
    loading,
  }));

  readonly setConnected = this.updater((state, connected: boolean) => ({
    ...state,
    connected,
  }));

  readonly setError = this.updater((state, error: string | undefined) => ({
    ...state,
    error,
  }));

  readonly dismissReminderInState = this.updater((state, id: string) => ({
    ...state,
    pending: state.pending.filter((reminder) => reminder.id !== id),
  }));

  readonly snoozeReminderInState = this.updater((state, id: string) => ({
    ...state,
    pending: state.pending.filter((reminder) => reminder.id !== id),
  }));

  readonly loadReminders = this.effect<void>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),
      switchMap(() =>
        handleEffect(
          this.reminderApi.getReminders(ReminderState.Pending),
          (reminders) => this.setPending(reminders),
          (err) => this.setError(err.message ?? 'Load reminders failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly loadUpcoming = this.effect<void>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),
      switchMap(() =>
        handleEffect(
          this.reminderApi.getUpcomingReminders(),
          (todos) => this.setUpcoming(todos),
          (err) => this.setError(err.message ?? 'Load upcoming reminders failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly dismissReminder = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),
      switchMap((id) =>
        handleEffect(
          this.reminderApi.dismissReminder(id),
          () => this.dismissReminderInState(id),
          (err) => this.setError(err.message ?? 'Dismiss reminder failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly snoozeReminder = this.effect<{
    id: string;
    request: SnoozeReminderRequest;
  }>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),
      switchMap(({ id, request }) =>
        handleEffect(
          this.reminderApi.snoozeReminder(id, request),
          () => this.snoozeReminderInState(id),
          (err) => this.setError(err.message ?? 'Snooze reminder failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );
}
