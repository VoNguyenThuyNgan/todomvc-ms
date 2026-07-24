export interface UpdateTodoRequest {
  title: string;
  isCompleted: boolean;
  dueAt: string | null;
}