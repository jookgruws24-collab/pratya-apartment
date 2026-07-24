import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
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
import type { Room, Status } from "../types";

const emptyForm = {
  roomNumber: "",
  floor: 1,
  roomStatusId: 1,
  imageUrl: "",
};

export default function RoomsPage() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState(emptyForm);

  async function load() {
    const [roomRes, statusRes] = await Promise.all([
      api.get<Room[]>("/api/room"),
      api.get<Status[]>("/api/status/rooms"),
    ]);
    setRooms(roomRes.data);
    setStatuses(statusRes.data);
  }

  useEffect(() => {
    load();
  }, []);

  function openCreate() {
    setEditingId(null);
    setForm(emptyForm);
    setOpen(true);
  }

  function openEdit(r: Room) {
    setEditingId(r.id);
    setForm({
      roomNumber: r.roomNumber,
      floor: r.floor,
      roomStatusId: r.roomStatusId,
      imageUrl: r.imageUrl ?? "",
    });
    setOpen(true);
  }

  async function save() {
    if (editingId) {
      await api.put(`/api/room/${editingId}`, form);
    } else {
      await api.post("/api/room", form);
    }
    setOpen(false);
    load();
  }

  async function remove(id: string) {
    if (!confirm("ยืนยันการลบห้องนี้?")) return;
    await api.delete(`/api/room/${id}`);
    load();
  }

  return (
    <Box>
      <Stack
        direction="row"
        sx={{ justifyContent: "space-between", alignItems: "center", mb: 2 }}
      >
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          ห้อง
        </Typography>
        <Button variant="contained" onClick={openCreate}>
          + เพิ่มห้อง
        </Button>
      </Stack>

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>เลขห้อง</TableCell>
              <TableCell>ชั้น</TableCell>
              <TableCell>สถานะ</TableCell>
              <TableCell align="right">จัดการ</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rooms.map((r) => (
              <TableRow key={r.id}>
                <TableCell>{r.roomNumber}</TableCell>
                <TableCell>{r.floor}</TableCell>
                <TableCell>{r.roomStatus?.name ?? r.roomStatusId}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => openEdit(r)} size="small">
                    <EditIcon fontSize="small" />
                  </IconButton>
                  <IconButton
                    onClick={() => remove(r.id)}
                    size="small"
                    color="error"
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
            {rooms.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} align="center">
                  ยังไม่มีข้อมูลห้อง
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="xs">
        <DialogTitle>{editingId ? "แก้ไขห้อง" : "เพิ่มห้อง"}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="เลขห้อง"
              value={form.roomNumber}
              onChange={(e) => setForm({ ...form, roomNumber: e.target.value })}
              fullWidth
            />
            <TextField
              label="ชั้น"
              type="number"
              value={form.floor}
              onChange={(e) =>
                setForm({ ...form, floor: Number(e.target.value) })
              }
              fullWidth
            />
            <TextField
              select
              label="สถานะ"
              value={form.roomStatusId}
              onChange={(e) =>
                setForm({ ...form, roomStatusId: Number(e.target.value) })
              }
              fullWidth
            >
              {statuses.map((s) => (
                <MenuItem key={s.id} value={s.id}>
                  {s.name}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              label="ลิงก์รูปภาพ (ไม่บังคับ)"
              value={form.imageUrl}
              onChange={(e) => setForm({ ...form, imageUrl: e.target.value })}
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
