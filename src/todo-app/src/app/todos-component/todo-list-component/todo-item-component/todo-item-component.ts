import { Component, ElementRef, input, output, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Todo } from '../../../core/models/todo.model';

@Component({
  selector: 'app-todo-item-component',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './todo-item-component.html',
  styleUrl: './todo-item-component.scss',
})
export class TodoItemComponent {
    todo = input.required<Todo>();
    toggle = output<string>(); //emit(id)
    remove = output<string>(); //emit(id)
    update = output<{
      id: string;
      title: string
    }>();

    editing = false;
    title = '';

    inputRef = viewChild<ElementRef<HTMLInputElement>>('todoInputRef');

    toggleTodo(): void {
      this.toggle.emit(this.todo().id);
    }

    removeTodo(): void {
      this.remove.emit(this.todo().id);
    }

    startEdit(): void {
      this.editing = true;

      this.title = this.todo().title;

      queueMicrotask(() => {
        this.inputRef()?.nativeElement.focus();
      })
    }

    commitEdit(): void {
      if (!this.editing) return;

      const text = this.title.trim();

      this.editing = false;

      if (!text) {
        this.remove.emit(this.todo().id);
        return
      }

      if (text === this.todo().title) return;

      this.update.emit({
        id: this.todo().id,
        title: text
      })
    }

    cancelEdit(): void {
      this.title = this.todo().title;
      this.editing = false;
    }
}
