import { Todo, TodoFilter } from "../core/models/todo.model";

export interface TodosState {
    todos: Todo[];
    filter: TodoFilter;
    loading: boolean;
}