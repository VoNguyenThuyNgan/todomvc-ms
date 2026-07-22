import { Component, ElementRef, input, output, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Todo } from '../../../models/todo.model';

@Component({
  selector: 'app-todo-item-component',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './todo-item-component.html',
  styleUrl: './todo-item-component.scss',
})
export class TodoItemComponent {
  todo = input.required<Todo>();
  toggleTodoChange = output<string>(); //emit(id)
  removeTodoChange = output<string>(); //emit(id)
  updateTodoChange = output<{
    id: string;
    title: string;
  }>();

  editing = false;
  draftTitle = '';

  inputRef = viewChild<ElementRef<HTMLInputElement>>('todoInputRef');

  onToggle(): void {
    this.toggleTodoChange.emit(this.todo().id);
  }

  onRemove(): void {
    this.removeTodoChange.emit(this.todo().id);
  }

  onStartEdit(): void {
    this.editing = true;
    this.draftTitle = this.todo().title;
    setTimeout(() => {
      (this.inputRef()?.nativeElement.focus(), 0);
    });
  }

  onCommitEdit(): void {
    if (!this.editing) return;
    const text = this.draftTitle.trim();
    this.editing = false;

    if (!text) {
      this.removeTodoChange.emit(this.todo().id);
      return;
    }

    if (text !== this.todo().title) {
      this.updateTodoChange.emit({
        id: this.todo().id,
        title: text,
      });
    }
  }

  onCancelEdit(): void {
    this.draftTitle = this.todo().title;
    this.editing = false;
  }
}
