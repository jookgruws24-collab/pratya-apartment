import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import App from "./App.tsx";
import { AuthProvider } from "./auth/AuthContext";

// ธีมสีของ MUI (ปรับแต่งได้ตามชอบ)
const theme = createTheme({
  palette: {
    primary: { main: "#1976d2" },
  },
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      {/* CssBaseline รีเซ็ต style พื้นฐานให้เหมือนกันทุกเบราว์เซอร์ */}
      <CssBaseline />
      <AuthProvider>
        <App />
      </AuthProvider>
    </ThemeProvider>
  </StrictMode>
);
