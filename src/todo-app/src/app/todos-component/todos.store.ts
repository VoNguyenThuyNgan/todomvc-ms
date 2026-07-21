import { inject, Injectable } from '@angular/core';
import { TodosState } from './todos.state';
import { ComponentStore } from '@ngrx/component-store';
import { todo } from 'node:test';
import { catchError, EMPTY, filter, finalize, switchMap, tap } from 'rxjs';
import { Todo, TodoFilter } from '../core/models/todo.model';
import { TodoApiService } from '../core/services/todo-api.service';
import { title } from 'process';
import { error } from 'console';

const initialState: TodosState = {
  todos: [],
  filter: 'all',
  loading: false,
};

@Injectable()
export class TodosStore extends ComponentStore<TodosState> {
  private readonly todoApi = inject(TodoApiService);
  constructor() {
    super(initialState);
    console.log('TodosStore initialized');
  }

  // Selector
  readonly todos$ = this.select((state) => state.todos);
  readonly filter$ = this.select((state) => state.filter);
  readonly loading$ = this.select((state) => state.loading);

  readonly filteredTodos$ = this.select(this.todos$, this.filter$, (todos, filter) => {
    switch (filter) {
      case 'active':
        return todos.filter((todo) => !todo.isCompleted);
      case 'completed':
        return todos.filter((todo) => todo.isCompleted);
      default:
        return todos;
    }
  });

  readonly activeCount$ = this.select(
    this.todos$,
    (todos) => todos.filter((todo) => !todo.isCompleted).length,
  );

  readonly completedCount$ = this.select(
    this.todos$,
    (todos) => todos.filter((todo) => todo.isCompleted).length,
  );

  readonly todosCount$ = this.select(this.todos$, (todos) => todos.length);

  // Updater
  readonly setTodos = this.updater((state, todos: Todo[]) => ({ ...state, todos }));

  readonly setFilter = this.updater((state, filter: TodoFilter) => ({ ...state, filter }));

  readonly setLoading = this.updater((state, loading: boolean) => ({ ...state, loading }));

  readonly toggleTodoInState = this.updater((state, id: string) => ({
    ...state,
    todos: state.todos.map((todo) =>
      todo.id === id
        ? {
            ...todo,
            isCompleted: !todo.isCompleted,
          }
        : todo,
    ),
  }));

  readonly addTodoToState = this.updater((state, todo: Todo) => ({
    ...state,
    todos: [todo, ...state.todos],
  }));

  readonly updateTodoInState = this.updater(
    (
      state,
      data: {
        id: string;
        title: string;
      },
    ) => ({
      ...state,

      todos: state.todos.map((todo) =>
        todo.id === data.id
          ? {
              ...todo,
              title: data.title,
            }
          : todo,
      ),
    }),
  );

  readonly removeTodoFromState = this.updater((state, id: string) => ({
    ...state,

    todos: state.todos.filter((todo) => todo.id !== id),
  }));

  readonly clearCompletedFromState = this.updater((state) => ({
    ...state,

    todos: state.todos.filter((todo) => !todo.isCompleted),
  }));

  readonly toggleAllInState = this.updater((state, isCompleted: boolean) => ({
    ...state,

    todos: state.todos.map((todo) => ({
      ...todo,
      isCompleted,
    })),
  }));

  // Effect
  readonly loadTodos = this.effect<void>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap(() =>
        this.todoApi.getTodos().pipe(
          tap((todos) => {
            console.log('Todos on loadtodos: ', todos);
            this.setTodos(todos);
          }),
          catchError((error) => {
            console.error(error);
            return EMPTY;
          }),
          finalize(() => this.setLoading(false)),
        ),
      ),
    ),
  );

  readonly addTodo = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((title) =>
        this.todoApi.createTodo(title).pipe(
          tap((todo) => this.addTodoToState(todo)),
          catchError((error) => {
            console.error(error);
            return EMPTY;
          }),
          finalize(() => this.setLoading(false)),
        ),
      ),
    ),
  );

  readonly updateTodo = this.effect<{
    id: string;
    title: string;
  }>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
      }),

      switchMap((data) => {
        const todo = this.get().todos.find((t) => t.id === data.id);

        if (!todo) {
          return EMPTY;
        }

        return this.todoApi
          .updateTodo(todo.id, {
            title: data.title,
            isCompleted: todo.isCompleted,
          })
          .pipe(
            tap(() => {
              this.updateTodoInState(data);
            }),

            catchError((error) => {
              console.error('Update Todo failed', error);

              return EMPTY;
            }),

            finalize(() => {
              this.setLoading(false);
            }),
          );
      }),
    ),
  );

  readonly deleteTodo = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),

      switchMap((id) =>
        this.todoApi.deleteTodo(id).pipe(
          tap(() => {
            this.removeTodoFromState(id);
          }),

          catchError((error) => {
            console.error('Delete Todo failed', error);

            return EMPTY;
          }),

          finalize(() => {
            this.setLoading(false);
          }),
        ),
      ),
    ),
  );

  readonly toggleTodo = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((id) =>
        this.todoApi.toggleTodo(id).pipe(
          tap(() => this.toggleTodoInState(id)),
          catchError((error) => {
            console.error(error);
            return EMPTY;
          }),
          finalize(() => this.setLoading(false)),
        ),
      ),
    ),
  );

  readonly clearCompleted = this.effect<void>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),

      switchMap(() =>
        this.todoApi.clearCompleted().pipe(
          tap(() => {
            this.clearCompletedFromState();
          }),

          catchError((error) => {
            console.error('Clear completed failed', error);

            return EMPTY;
          }),

          finalize(() => {
            this.setLoading(false);
          }),
        ),
      ),
    ),
  );

  readonly toggleAll = this.effect<boolean>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((IsCompleted) =>
        this.todoApi
          .toggleAll({
            IsCompleted,
          })
          .pipe(
            tap(() => {
              this.toggleAllInState(IsCompleted);
            }),

            catchError((error) => {
              console.error('Toggle All failed', error);
              return EMPTY;
            }),
            finalize(() => {
              this.setLoading(false);
            }),
          ),
      ),
    ),
  );
}
