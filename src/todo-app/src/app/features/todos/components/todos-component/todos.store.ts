import { inject, Injectable } from '@angular/core';
import { TodosState } from './todos.state';
import { ComponentStore } from '@ngrx/component-store';
import { EMPTY, switchMap, tap } from 'rxjs';
import { TodoApiService } from '../../services/todo-api.service';
import { Todo, TodoFilter } from '../../models/todo.model';
import { handleEffect } from '../../../../core/utils/effect.helper';
import { CreateTodoRequest } from '../../dtos/create-todo-request';
import { request } from 'http';

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
  readonly setError = this.updater((state, error: string | undefined) => ({ ...state, error }));
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
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),

      switchMap(() =>
        handleEffect(
          this.todoApi.getTodos(),
          (todos) => this.setTodos(todos),
          (err) => this.setError(err.message ?? 'Load Todos failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly addTodo = this.effect<CreateTodoRequest>((trigger$) =>
    trigger$.pipe(
      tap(() => {
        this.setLoading(true);
        this.setError(undefined);
      }),
      switchMap((request) =>
        handleEffect(
          this.todoApi.createTodo(request),
          (todo) => this.addTodoToState(todo),
          (err) => this.setError(err.message ?? 'Add Todo failed'),
          () => this.setLoading(false),
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
        this.setError(undefined);
      }),

      switchMap((data) => {
        const todo = this.get().todos.find((todo) => todo.id === data.id);
        if (!todo) {
          this.setLoading(false);
          this.setError('Todo not found');
          return EMPTY;
        }

        return handleEffect(
          this.todoApi.updateTodo(todo.id, {
            title: data.title,
            isCompleted: todo.isCompleted,
            dueAt: todo.dueAt
          }),
          () => this.updateTodoInState(data),
          (err) => this.setError(err.message ?? 'Update Todo failed'),
          () => this.setLoading(false),
        );
      }),
    ),
  );

  readonly deleteTodo = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((id) =>
        handleEffect(
          this.todoApi.deleteTodo(id),
          () => this.removeTodoFromState(id),
          (err) => this.setError(err.message ?? 'Delete Todo failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly toggleTodo = this.effect<string>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((id) =>
        handleEffect(
          this.todoApi.toggleTodo(id),
          () => this.toggleTodoInState(id),
          (err) => this.setError(err.message ?? 'Toggle Todo failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly clearCompleted = this.effect<void>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),

      switchMap(() =>
        handleEffect(
          this.todoApi.clearCompleted(),
          () => this.clearCompletedFromState(),
          (err) => this.setError(err.message ?? 'Clear completed failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );

  readonly toggleAll = this.effect<boolean>((trigger$) =>
    trigger$.pipe(
      tap(() => this.setLoading(true)),
      switchMap((isCompleted) =>
        handleEffect(
          this.todoApi.toggleAll({
            isCompleted,
          }),
          () => this.toggleAllInState(isCompleted),
          (err) => this.setError(err.message ?? 'Toggle All failed'),
          () => this.setLoading(false),
        ),
      ),
    ),
  );
}
