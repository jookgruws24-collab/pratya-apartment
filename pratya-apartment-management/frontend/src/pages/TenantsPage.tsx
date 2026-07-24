import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import api from "../api/client";
import type { Tenant } from "../types";

const emptyForm = { firstName: "", lastName: "", roomNumber: "" };

export default function TenantsPage() {
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState(emptyForm);

  async function load() {
    const res = await api.get<Tenant[]>("/api/tenant");
    setTenants(res.data);
  }

  useEffect(() => {
    load();
  }, []);

  function openCreate() {
    setEditingId(null);
    setForm(emptyForm);
    setOpen(true);
  }

  function openEdit(t: Tenant) {
    setEditingId(t.id);
    setForm({
      firstName: t.firstName,
      lastName: t.lastName,
      roomNumber: t.roomNumber,
    });
    setOpen(true);
  }

  async function save() {
    if (editingId) {
      await api.put(`/api/tenant/${editingId}`, form);
    } else {
      await api.post("/api/tenant", form);
    }
    setOpen(false);
    load();
  }

  async function remove(id: string) {
    if (!confirm("ยืนยันการลบผู้เช่าคนนี้?")) return;
    await api.delete(`/api/tenant/${id}`);
    load();
  }

  return (
    <Box>
      <Stack
        direction="row"
        sx={{ justifyContent: "space-between", alignItems: "center", mb: 2 }}
      >
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          ผู้เช่า
        </Typography>
        <Button variant="contained" onClick={openCreate}>
          + เพิ่มผู้เช่า
        </Button>
      </Stack>

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ชื่อ</TableCell>
              <TableCell>นามสกุล</TableCell>
              <TableCell>ห้อง</TableCell>
              <TableCell align="right">จัดการ</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tenants.map((t) => (
              <TableRow key={t.id}>
                <TableCell>{t.firstName}</TableCell>
                <TableCell>{t.lastName}</TableCell>
                <TableCell>{t.roomNumber}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => openEdit(t)} size="small">
                    <EditIcon fontSize="small" />
                  </IconButton>
                  <IconButton
                    onClick={() => remove(t.id)}
                    size="small"
                    color="error"
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
            {tenants.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} align="center">
                  ยังไม่มีข้อมูลผู้เช่า
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="xs">
        <DialogTitle>{editingId ? "แก้ไขผู้เช่า" : "เพิ่มผู้เช่า"}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="ชื่อ"
              value={form.firstName}
              onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              fullWidth
            />
            <TextField
              label="นามสกุล"
              value={form.lastName}
              onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              fullWidth
            />
            <TextField
              label="ห้อง"
              value={form.roomNumber}
              onChange={(e) => setForm({ ...form, roomNumber: e.target.value })}
              fullWidth
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>ยกเลิก</Button>
          <Button variant="contained" onClick={save}>
            บันทึก
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
