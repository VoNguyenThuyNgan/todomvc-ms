import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { effect } from '@angular/core';
import { TodoFilter } from '../../models/todo.model';

@Component({
  selector: 'app-todo-footer-component',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './todo-footer-component.html',
  styleUrl: './todo-footer-component.scss',
})
export class TodoFooterComponent {
  todosCount = input.required<number | null>();
  activeCount = input.required<number | null>();
  completedCount = input.required<number | null>();
  filter = input.required<TodoFilter | null>();

  clearCompletedChange = output<void>();

  onClearCompleted(): void {
    this.clearCompletedChange.emit();
  }
}
