import { createApiClient } from "./client";

export async function askAgent(token: string, message: string) {
  const client = createApiClient(token);
  const { data } = await client.post<{ result: string }>("/Ai/AiAssist", message);
  return data.result;
}

