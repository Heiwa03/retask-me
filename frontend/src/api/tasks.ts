import { createApiClient } from "./client";
import { Task } from "../types";

export interface TaskPayload {
  title: string;
  description?: string;
  deadline?: string | null;
  priority: number;
  status: number;
}

export async function fetchTasks(token: string): Promise<Task[]> {
  const client = createApiClient(token);
  const { data } = await client.get<Task[]>("/Task/tasks");
  return data;
}

export async function createTask(
  token: string,
  payload: TaskPayload,
): Promise<void> {
  const client = createApiClient(token);
  await client.post("/Task/createTask", payload);
}

export async function updateTask(
  token: string,
  taskUid: string,
  payload: TaskPayload,
): Promise<void> {
  const client = createApiClient(token);
  await client.put(`/Task/updateTask/${taskUid}`, payload);
}

export async function deleteTask(token: string, taskUid: string): Promise<void> {
  const client = createApiClient(token);
  await client.delete(`/Task/deleteTask/${taskUid}`);
}

