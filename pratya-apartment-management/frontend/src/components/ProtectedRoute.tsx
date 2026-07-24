import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../auth/AuthContext";

// ครอบหน้าที่ต้องล็อกอินก่อน ถ้ายังไม่ล็อกอินให้เด้งไปหน้า /login
export default function ProtectedRoute({ children }: { children: ReactNode }) {
  const { token } = useAuth();

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
}
