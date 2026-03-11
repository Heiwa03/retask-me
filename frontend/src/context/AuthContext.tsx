import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { loginUser, registerUser } from "../api/auth";
import { AuthTokens } from "../types";

interface AuthContextProps {
  token: string | null;
  isReady: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (mail: string, password: string, repeatPassword: string) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextProps | undefined>(undefined);

const TOKEN_KEY = "retask-token";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const stored = localStorage.getItem(TOKEN_KEY);
    if (stored) {
      setToken(stored);
    }
    setIsReady(true);
  }, []);

  const persistTokens = useCallback((tokens: AuthTokens) => {
    setToken(tokens.token);
    localStorage.setItem(TOKEN_KEY, tokens.token);
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const result = await loginUser({ email, password });
      persistTokens(result);
    },
    [persistTokens],
  );

  const register = useCallback(
    async (mail: string, password: string, repeatPassword: string) => {
      await registerUser({ mail, password, repeatPassword });
      // Automatically log in after registration
      await login(mail, password);
    },
    [login],
  );

  const logout = useCallback(() => {
    setToken(null);
    localStorage.removeItem(TOKEN_KEY);
  }, []);

  const value = useMemo(
    () => ({
      token,
      isReady,
      isAuthenticated: Boolean(token),
      login,
      register,
      logout,
    }),
    [isReady, login, logout, register, token],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

