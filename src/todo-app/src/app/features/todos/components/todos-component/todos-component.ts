import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { provideComponentStore } from '@ngrx/component-store';
import { TodosStore } from './todos.store';
import { AsyncPipe } from '@angular/common';
import { Router } from '@angular/router';
import { NavigationEnd } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { TodoHeaderComponent } from '../todo-header-component/todo-header-component';
import { TodoListComponent } from '../todo-list-component/todo-list-component';
import { TodoFooterComponent } from '../todo-footer-component/todo-footer-component';

@Component({
  selector: 'app-todos-component',
  standalone: true,
  imports: [AsyncPipe, TodoHeaderComponent, TodoListComponent, TodoFooterComponent],
  providers: [provideComponentStore(TodosStore)],
  templateUrl: './todos-component.html',
  styleUrl: './todos-component.scss',
})
export class TodosComponent implements OnInit, OnDestroy {
  private readonly store = inject(TodosStore);
  private readonly router = inject(Router);
  private sub?: Subscription;

  readonly todos$ = this.store.todos$;
  readonly filteredTodos$ = this.store.filteredTodos$;
  readonly filter$ = this.store.filter$;
  readonly loading$ = this.store.loading$;
  readonly todosCount$ = this.store.todosCount$;
  readonly activeCount$ = this.store.activeCount$;
  readonly completedCount$ = this.store.completedCount$;

  ngOnInit(): void {
    this.store.loadTodos();
    this.sub = this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const url = this.router.url;

        if (url.includes('/active')) {
          this.store.setFilter('active');
        } else if (url.includes('/completed')) {
          this.store.setFilter('completed');
        } else {
          this.store.setFilter('all');
        }
      });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  addTodo(title: string): void {
    this.store.addTodo(title);
  }

  toggleTodo(id: string): void {
    this.store.toggleTodo(id);
  }

  removeTodo(id: string): void {
    this.store.deleteTodo(id);
  }

  updateTodo(data: { id: string; title: string }): void {
    this.store.updateTodo(data);
  }

  toggleAll(completed: boolean): void {
    this.store.toggleAll(completed);
  }

  clearCompleted(): void {
    this.store.clearCompleted();
  }
}
