import { Link as RouterLink, Outlet, useLocation } from "react-router-dom";
import {
  AppBar,
  Box,
  Button,
  Container,
  Toolbar,
  Typography,
} from "@mui/material";
import { useAuth } from "../auth/AuthContext";

// เมนูด้านบน
const navItems = [
  { label: "แดชบอร์ด", path: "/" },
  { label: "ผู้เช่า", path: "/tenants" },
  { label: "ห้อง", path: "/rooms" },
  { label: "บิล", path: "/bills" },
  { label: "ไฟล์", path: "/files" },
];

export default function Layout() {
  const { username, logout } = useAuth();
  const location = useLocation();

  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ mr: 3, fontWeight: 700 }}>
            Pratya Apartment
          </Typography>

          <Box sx={{ display: "flex", gap: 1, flexGrow: 1, flexWrap: "wrap" }}>
            {navItems.map((item) => (
              <Button
                key={item.path}
                component={RouterLink}
                to={item.path}
                color="inherit"
                variant={
                  location.pathname === item.path ? "outlined" : "text"
                }
              >
                {item.label}
              </Button>
            ))}
          </Box>

          <Typography sx={{ mr: 2 }}>{username}</Typography>
          <Button color="inherit" onClick={logout}>
            ออกจากระบบ
          </Button>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ py: 4, flexGrow: 1 }}>
        {/* หน้าย่อยแต่ละหน้าจะถูกแสดงตรงนี้ */}
        <Outlet />
      </Container>
    </Box>
  );
}
