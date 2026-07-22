import { Todo, TodoFilter } from "../../models/todo.model";
export interface TodosState {
    todos: Todo[];
    filter: TodoFilter;
    loading: boolean;
    error?: string;
}