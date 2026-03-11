import { createApiClient } from "./client";
import { UserProfile } from "../types";

const base = "/Profile";

export async function getProfile(token: string): Promise<UserProfile | null> {
  const client = createApiClient(token);
  try {
    const { data } = await client.get<UserProfile>(`${base}/getUserProfile`);
    return data;
  } catch (err: any) {
    if (err?.response?.status === 404) {
      return null;
    }
    throw err;
  }
}

export async function registerProfile(token: string, profile: UserProfile) {
  const client = createApiClient(token);
  await client.post(`${base}/registerProfile`, profile);
}

export async function updateProfile(token: string, profile: UserProfile) {
  const client = createApiClient(token);
  await client.put(`${base}/updateRegisterProfile`, profile);
}

