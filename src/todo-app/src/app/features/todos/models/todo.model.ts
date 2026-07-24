export interface Todo {
  id: string;
  title: string;
  isCompleted: boolean;
  dueAt: string | null;
}

export type TodoFilter = 'all' | 'active' | 'completed';

export interface UpdateTodoInput {
  id: string;
  title: string;
  dueAt: string | null;
}
