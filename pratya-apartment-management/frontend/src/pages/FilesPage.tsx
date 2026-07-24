import { useEffect, useRef, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Link,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import api from "../api/client";
import type { UploadedFile } from "../types";

const baseURL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5034";

export default function FilesPage() {
  const [files, setFiles] = useState<UploadedFile[]>([]);
  const [error, setError] = useState("");
  const [uploading, setUploading] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  async function load() {
    const res = await api.get<UploadedFile[]>("/api/file");
    setFiles(res.data);
  }

  useEffect(() => {
    load();
  }, []);

  async function handleUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;

    setError("");
    setUploading(true);
    try {
      const formData = new FormData();
      formData.append("file", file);
      await api.post("/api/file/upload", formData);
      await load();
    } catch {
      setError("อัปโหลดไม่สำเร็จ (รองรับ JPG, PNG, PDF ไม่เกิน 5 MB)");
    } finally {
      setUploading(false);
      if (inputRef.current) inputRef.current.value = "";
    }
  }

  // ถ้า url เป็น path (/uploads/..) ให้เติม baseURL ข้างหน้า
  function fullUrl(url: string) {
    return url.startsWith("http") ? url : `${baseURL}${url}`;
  }

  const kb = (n: number) => `${(n / 1024).toFixed(1)} KB`;

  return (
    <Box>
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 700 }}>
        ไฟล์
      </Typography>

      <Stack spacing={2}>
        {error && <Alert severity="error">{error}</Alert>}

        <Box>
          <Button
            variant="contained"
            startIcon={<UploadFileIcon />}
            component="label"
            disabled={uploading}
          >
            {uploading ? "กำลังอัปโหลด..." : "อัปโหลดไฟล์"}
            <input
              ref={inputRef}
              type="file"
              hidden
              accept=".jpg,.jpeg,.png,.pdf"
              onChange={handleUpload}
            />
          </Button>
        </Box>

        <Paper>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>ชื่อไฟล์</TableCell>
                <TableCell>ชนิด</TableCell>
                <TableCell align="right">ขนาด</TableCell>
                <TableCell>เปิดดู</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {files.map((f) => (
                <TableRow key={f.id}>
                  <TableCell>{f.fileName}</TableCell>
                  <TableCell>{f.contentType}</TableCell>
                  <TableCell align="right">{kb(f.sizeBytes)}</TableCell>
                  <TableCell>
                    <Link
                      href={fullUrl(f.url)}
                      target="_blank"
                      rel="noopener"
                    >
                      เปิด
                    </Link>
                  </TableCell>
                </TableRow>
              ))}
              {files.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} align="center">
                    ยังไม่มีไฟล์
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </Paper>
      </Stack>
    </Box>
  );
}
