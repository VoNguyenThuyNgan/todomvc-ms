import { Component, input, output } from '@angular/core';
import { TodoItemComponent } from './todo-item-component/todo-item-component';
import { Todo, UpdateTodoTitle } from '../../models/todo.model';

@Component({
  selector: 'app-todo-list-component',
  standalone: true,
  imports: [TodoItemComponent],
  templateUrl: './todo-list-component.html',
  styleUrl: './todo-list-component.scss',
})
export class TodoListComponent {
  todos = input.required<Todo[] | null>();

  toggleTodoChange = output<string>();
  removeTodoChange = output<string>();
  updateTodoChange = output<UpdateTodoTitle>();
  toggleAllTodosChange = output<boolean>();

  allCompleted(): boolean {
    const todos = this.todos() ?? [];
    return todos.length > 0 && todos.every((todo) => todo.isCompleted);
  }

  onToggleAll(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.toggleAllTodosChange.emit(checked);
  }
}
