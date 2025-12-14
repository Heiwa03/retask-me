import { createApiClient } from "./client";
import { Board, Task } from "../types";

const base = "/Board";

export async function getBoards(token: string): Promise<Board[]> {
  const client = createApiClient(token);
  const { data } = await client.get<Board[]>(`${base}/getBoardTask`);
  return data;
}

export async function createBoard(token: string, payload: { title: string; description?: string }) {
  const client = createApiClient(token);
  await client.post(`${base}/createBoard`, payload);
}

export async function updateBoard(
  token: string,
  boardUuid: string,
  payload: { title?: string; description?: string },
) {
  const client = createApiClient(token);
  await client.put(`${base}/updateBoard`, payload, { params: { boardUuid } });
}

export async function deleteBoard(token: string, boardUuid: string) {
  const client = createApiClient(token);
  await client.delete(`${base}/deleteBoard`, { params: { boardUuid } });
}

export async function addTaskToBoard(token: string, boardUuid: string, taskUuid: string) {
  const client = createApiClient(token);
  await client.post(`${base}/addTaskToBoard`, null, { params: { boardUuid, taskUuid } });
}

export async function removeTaskFromBoard(token: string, boardUuid: string, taskUuid: string) {
  const client = createApiClient(token);
  await client.delete(`${base}/deleteTaskFromBoard`, { params: { boardUuid, taskUuid } });
}

export async function getBoardTasks(token: string, boardUuid: string): Promise<Task[]> {
  const client = createApiClient(token);
  const { data } = await client.get<Task[]>(`${base}/getTasksfromBoard`, { params: { boardUuid } });
  return data;
}

