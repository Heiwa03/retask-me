import { createApiClient } from "./client";
import { AuthTokens } from "../types";

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  mail: string;
  password: string;
  repeatPassword: string;
}

export async function loginUser(
  payload: LoginPayload,
): Promise<AuthTokens & { isVerified?: boolean }> {
  const client = createApiClient();
  const { data } = await client.post("/Auth/login", payload);
  return data;
}

export async function registerUser(payload: RegisterPayload) {
  const client = createApiClient();
  const { data } = await client.post("/Reg/register", payload);
  return data;
}

