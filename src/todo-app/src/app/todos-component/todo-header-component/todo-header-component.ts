import { Component, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-header-component',
  imports: [FormsModule],
  standalone: true,
  templateUrl: './todo-header-component.html',
  styleUrl: './todo-header-component.scss',
})
export class TodoHeaderComponent {
    title = '';

    @Output()
    add = new EventEmitter<string>();

    addTodo(): void {
      const text = this.title.trim();

      if (!text) return;

      this.add.emit(text);

      this.title = ''
    }
}
