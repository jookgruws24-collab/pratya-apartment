import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import DashboardPage from "./pages/DashboardPage";
import TenantsPage from "./pages/TenantsPage";
import RoomsPage from "./pages/RoomsPage";
import BillsPage from "./pages/BillsPage";
import FilesPage from "./pages/FilesPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* หน้าที่เข้าได้โดยไม่ต้องล็อกอิน */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* หน้าที่ต้องล็อกอินก่อน (ครอบด้วย ProtectedRoute + Layout) */}
        <Route
          element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }
        >
          <Route path="/" element={<DashboardPage />} />
          <Route path="/tenants" element={<TenantsPage />} />
          <Route path="/rooms" element={<RoomsPage />} />
          <Route path="/bills" element={<BillsPage />} />
          <Route path="/files" element={<FilesPage />} />
        </Route>

        {/* ถ้าเข้า path แปลก ๆ ให้กลับหน้าแรก */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
