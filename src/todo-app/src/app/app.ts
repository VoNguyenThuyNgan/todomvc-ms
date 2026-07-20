import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TodoApiService } from './core/services/todo-api.service';
import { Todo } from './core/models/todo.model';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly todoApi = inject(TodoApiService);

  todos: Todo[] = [];

  ngOnInit() {
    this.todoApi
      .getTodos()
      .subscribe({
        next: todos => {
          console.log(todos);

          this.todos = todos;
        },

        error: err => {
          console.error(err);
        }
      });
  }
}
