export type Priority = 1 | 2 | 3;
export type Status = 0 | 1 | 2 | 3 | 4;
export type Gender = 0 | 1 | 2; // Girl=0, Male=1, Undefiend=2 (per backend enum)

export interface Task {
  uuid: string;
  title: string;
  description?: string | null;
  deadline?: string | null;
  priority: Priority;
  status: Status;
  boardUuid?: string | null;
}

export interface UserProfile {
  firstName: string;
  lastName: string;
  gender: Gender;
}

export interface AuthTokens {
  token: string;
  refreshToken?: string;
}

export interface Board {
  uuid: string;
  title: string;
  description?: string | null;
}

