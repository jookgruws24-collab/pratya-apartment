import {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from "react";
import secureLocalStorage from "../api/storage";
import api, { TOKEN_KEY } from "../api/client";

const USERNAME_KEY = "auth_username";

interface AuthContextType {
  token: string | null;
  username: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  // อ่านค่า token/username ที่เคยเก็บไว้ (เผื่อผู้ใช้รีเฟรชหน้า)
  const [token, setToken] = useState<string | null>(
    () => secureLocalStorage.getItem(TOKEN_KEY) as string | null
  );
  const [username, setUsername] = useState<string | null>(
    () => secureLocalStorage.getItem(USERNAME_KEY) as string | null
  );

  function saveAuth(newToken: string, newUsername: string) {
    secureLocalStorage.setItem(TOKEN_KEY, newToken);
    secureLocalStorage.setItem(USERNAME_KEY, newUsername);
    setToken(newToken);
    setUsername(newUsername);
  }

  async function login(user: string, password: string) {
    const res = await api.post("/api/auth/login", {
      username: user,
      password,
    });
    saveAuth(res.data.token, res.data.username);
  }

  async function register(user: string, password: string) {
    const res = await api.post("/api/auth/register", {
      username: user,
      password,
    });
    saveAuth(res.data.token, res.data.username);
  }

  function logout() {
    secureLocalStorage.removeItem(TOKEN_KEY);
    secureLocalStorage.removeItem(USERNAME_KEY);
    setToken(null);
    setUsername(null);
  }

  return (
    <AuthContext.Provider
      value={{ token, username, login, register, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// hook ช่วยเรียกใช้ context ได้สะดวก
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth ต้องอยู่ภายใน <AuthProvider>");
  }
  return ctx;
}
