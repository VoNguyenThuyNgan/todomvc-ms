import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Todo, TodoFilter } from '../models/todo.model';
import { Observable } from 'rxjs';
import { UpdateTodoRequest } from '../dtos/update-todo.request';
import { ToggleAllTodosRequest } from '../dtos/toggle-all-todos.request';
import { CreateTodoRequest } from '../dtos/create-todo-request';
import { environment } from '../../../../enviroments/environment';

@Injectable({
  providedIn: 'root',
})
export class TodoApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiBaseUrl}/bff/todos`;

  getTodos(filter?: TodoFilter): Observable<Todo[]> {
    if (!filter || filter === 'all') {
      return this.http.get<Todo[]>(this.baseUrl);
    }

    return this.http.get<Todo[]>(`${this.baseUrl}?filter=${filter}`);
  }

  getTodo(id: string): Observable<Todo> {
    return this.http.get<Todo>(`${this.baseUrl}/${id}`);
  }

  createTodo(request: CreateTodoRequest): Observable<Todo> {
    return this.http.post<Todo>(this.baseUrl, {
      request,
    });
  }

  updateTodo(id: string, request: UpdateTodoRequest): Observable<Todo> {
    return this.http.put<Todo>(`${this.baseUrl}/${id}`, request);
  }

  toggleTodo(id: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/toggle`, {});
  }

  deleteTodo(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  clearCompleted(): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/completed`);
  }

  toggleAll(request: ToggleAllTodosRequest): Observable<void> {
  return this.http.patch<void>(
    `${this.baseUrl}/toggle-all`,
    request
  );
}
}
