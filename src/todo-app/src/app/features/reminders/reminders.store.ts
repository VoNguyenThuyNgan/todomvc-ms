import { inject, Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { EMPTY, switchMap, tap, timer } from 'rxjs';

import { RemindersState } from './reminders.state';
import { ReminderApiService } from './services/reminder-api.service';
import { Reminder } from './models/reminder.model';
import { handleEffect } from '../../core/utils/effect.helper';

const initialState: RemindersState = {
  pending: [],
  connected: false,
  loading: false,
};

@Injectable()
export class RemindersStore
  extends ComponentStore<RemindersState> {

  private readonly reminderApi =
    inject(ReminderApiService);

  constructor() {
    super(initialState);
  }

  // =========================
  // Selectors
  // =========================

  readonly pending$ = this.select(
    (state) => state.pending,
  );

  readonly connected$ = this.select(
    (state) => state.connected,
  );

  readonly loading$ = this.select(
    (state) => state.loading,
  );

  readonly error$ = this.select(
    (state) => state.error,
  );

  readonly pendingCount$ = this.select(
    this.pending$,
    (pending) => pending.length,
  );

  // =========================
  // Updaters
  // =========================

  readonly setPending = this.updater(
    (state, pending: Reminder[]) => ({
      ...state,
      pending,
    }),
  );

  readonly setLoading = this.updater(
    (state, loading: boolean) => ({
      ...state,
      loading,
    }),
  );

  readonly setConnected = this.updater(
    (state, connected: boolean) => ({
      ...state,
      connected,
    }),
  );

  readonly setError = this.updater(
    (state, error: string | undefined) => ({
      ...state,
      error,
    }),
  );

  readonly removePending = this.updater(
    (state, id: string) => ({
      ...state,
      pending: state.pending.filter(
        (reminder) => reminder.id !== id,
      ),
    }),
  );

  // =========================
  // Effects
  // =========================

  readonly loadPending = this.effect<void>(
    (trigger$) =>
      trigger$.pipe(
        tap(() => {
          this.setLoading(true);
          this.setError(undefined);
        }),

        switchMap(() =>
          handleEffect(
            this.reminderApi.getReminders('Pending'),

            (reminders) => {
              this.setPending(reminders);
              this.setConnected(true);
            },

            (err) => {
              this.setError(
                err.message ??
                'Load reminders failed',
              );

              this.setConnected(false);
            },

            () => this.setLoading(false),
          ),
        ),
      ),
  );
}