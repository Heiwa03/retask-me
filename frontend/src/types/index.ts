export type Priority = 1 | 2 | 3;
export type Status = 0 | 1 | 2 | 3 | 4;

export interface Task {
  uuid: string;
  title: string;
  description?: string | null;
  deadline?: string | null;
  priority: Priority;
  status: Status;
}

export interface AuthTokens {
  token: string;
  refreshToken?: string;
}

