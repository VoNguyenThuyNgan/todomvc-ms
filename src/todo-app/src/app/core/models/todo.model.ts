export interface Todo {
  id: string;
  title: string;
  isCompleted: boolean;
}

export type TodoFilter = 'all' | 'active' | 'completed';

export interface UpdateTodoTitle {
  id: string;
  title: string;
}
