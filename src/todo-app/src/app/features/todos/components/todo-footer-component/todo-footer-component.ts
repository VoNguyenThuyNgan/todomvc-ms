import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TodoFilter } from '../../core/models/todo.model';
import { effect } from '@angular/core';

@Component({
  selector: 'app-todo-footer-component',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './todo-footer-component.html',
  styleUrl: './todo-footer-component.scss',
})
export class TodoFooterComponent {
  constructor() {
  effect(() => {
    console.log({
      todos: this.todosCount(),
      active: this.activeCount(),
      completed: this.completedCount(),
      filter: this.filter()
    });
  });
}
  todosCount = input.required<number>();
  activeCount = input.required<number>();
  completedCount = input.required<number>();
  filter = input.required<TodoFilter>();

  clearCompleted = output<void>();

  clearCompletedTodos(): void {
    this.clearCompleted.emit();
  }
}
