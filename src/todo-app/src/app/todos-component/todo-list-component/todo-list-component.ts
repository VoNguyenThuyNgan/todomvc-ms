import { Component, input, output } from '@angular/core';
import { TodoItemComponent } from './todo-item-component/todo-item-component';
import { Todo, UpdateTodoTitle } from '../../core/models/todo.model';

@Component({
  selector: 'app-todo-list-component',
  standalone: true,
  imports: [TodoItemComponent],
  templateUrl: './todo-list-component.html',
  styleUrl: './todo-list-component.scss',
})
export class TodoListComponent {
  todos = input.required<Todo[]>();

  toggle = output<string>();
  remove = output<string>();
  update = output<UpdateTodoTitle>();

  toggleAll = output<boolean>();

  allCompleted(): boolean {
    return this.todos().length > 0 && this.todos().every((todo) => todo.isCompleted);
  }

  toggleAllTodos(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.toggleAll.emit(checked);
  }
}
