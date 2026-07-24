import axios from "axios";
import secureLocalStorage from "./storage";

// ที่อยู่ backend อ่านจากไฟล์ .env (ตัวแปรต้องขึ้นต้นด้วย VITE_)
const baseURL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5034";

export const TOKEN_KEY = "auth_token";

// สร้าง axios instance กลางไว้ใช้ทั้งแอป
const api = axios.create({
  baseURL,
});

// Interceptor ขาออก: แนบ JWT token ไปกับทุก request อัตโนมัติ
api.interceptors.request.use((config) => {
  const token = secureLocalStorage.getItem(TOKEN_KEY) as string | null;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor ขาเข้า: ถ้า token หมดอายุ/ไม่ถูกต้อง (401) ให้เด้งกลับหน้า login
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      secureLocalStorage.removeItem(TOKEN_KEY);
      // กันไม่ให้ redirect ซ้ำถ้าอยู่หน้า login อยู่แล้ว
      if (!window.location.pathname.startsWith("/login")) {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export default api;
