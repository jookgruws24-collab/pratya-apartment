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
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import api from "../api/client";
import type { Bill, Room, Status, Tenant } from "../types";

const emptyForm = {
  roomId: "",
  tenantId: "",
  rentAmount: 0,
  waterAmount: 0,
  electricAmount: 0,
  commonFeeAmount: 0,
  lateFeeAmount: 0,
  billStatusId: 1,
  billingMonth: new Date().toISOString().slice(0, 7), // "YYYY-MM"
};

export default function BillsPage() {
  const [bills, setBills] = useState<Bill[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(emptyForm);

  async function load() {
    const [b, r, t, s] = await Promise.all([
      api.get<Bill[]>("/api/bill"),
      api.get<Room[]>("/api/room"),
      api.get<Tenant[]>("/api/tenant"),
      api.get<Status[]>("/api/status/bills"),
    ]);
    setBills(b.data);
    setRooms(r.data);
    setTenants(t.data);
    setStatuses(s.data);
  }

  useEffect(() => {
    load();
  }, []);

  function openCreate() {
    setForm(emptyForm);
    setOpen(true);
  }

  async function save() {
    // แปลง "YYYY-MM" ให้เป็นวันที่แบบเต็มก่อนส่งให้ backend
    const payload = {
      ...form,
      billingMonth: new Date(`${form.billingMonth}-01`).toISOString(),
    };
    await api.post("/api/bill", payload);
    setOpen(false);
    load();
  }

  async function remove(id: string) {
    if (!confirm("ยืนยันการลบบิลนี้?")) return;
    await api.delete(`/api/bill/${id}`);
    load();
  }

  // ดาวน์โหลด PDF (ต้องใช้ token จึงต้องดึงเป็น blob ผ่าน axios)
  async function downloadPdf() {
    const res = await api.get("/api/report/bills/pdf", {
      responseType: "blob",
    });
    const url = URL.createObjectURL(res.data);
    const a = document.createElement("a");
    a.href = url;
    a.download = "bills-report.pdf";
    a.click();
    URL.revokeObjectURL(url);
  }

  const baht = (n: number) => n.toLocaleString("th-TH");

  return (
    <Box>
      <Stack
        direction="row"
        sx={{ justifyContent: "space-between", alignItems: "center", mb: 2 }}
      >
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          บิล
        </Typography>
        <Stack direction="row" spacing={1}>
          <Button
            variant="outlined"
            startIcon={<PictureAsPdfIcon />}
            onClick={downloadPdf}
          >
            ออก PDF
          </Button>
          <Button variant="contained" onClick={openCreate}>
            + สร้างบิล
          </Button>
        </Stack>
      </Stack>

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ห้อง</TableCell>
              <TableCell>ผู้เช่า</TableCell>
              <TableCell>เดือน</TableCell>
              <TableCell align="right">รวม (บาท)</TableCell>
              <TableCell>สถานะ</TableCell>
              <TableCell align="right">จัดการ</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {bills.map((b) => (
              <TableRow key={b.id}>
                <TableCell>{b.room?.roomNumber ?? "-"}</TableCell>
                <TableCell>
                  {b.tenant
                    ? `${b.tenant.firstName} ${b.tenant.lastName}`
                    : "-"}
                </TableCell>
                <TableCell>{b.billingMonth.slice(0, 7)}</TableCell>
                <TableCell align="right">{baht(b.totalAmount)}</TableCell>
                <TableCell>{b.billStatus?.name ?? b.billStatusId}</TableCell>
                <TableCell align="right">
                  <IconButton
                    onClick={() => remove(b.id)}
                    size="small"
                    color="error"
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
            {bills.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  ยังไม่มีบิล
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>สร้างบิล</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              select
              label="ห้อง"
              value={form.roomId}
              onChange={(e) => setForm({ ...form, roomId: e.target.value })}
              fullWidth
            >
              {rooms.map((r) => (
                <MenuItem key={r.id} value={r.id}>
                  {r.roomNumber}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              select
              label="ผู้เช่า"
              value={form.tenantId}
              onChange={(e) => setForm({ ...form, tenantId: e.target.value })}
              fullWidth
            >
              {tenants.map((t) => (
                <MenuItem key={t.id} value={t.id}>
                  {t.firstName} {t.lastName}
                </MenuItem>
              ))}
            </TextField>

            <Stack direction="row" spacing={2}>
              <TextField
                label="ค่าเช่า"
                type="number"
                value={form.rentAmount}
                onChange={(e) =>
                  setForm({ ...form, rentAmount: Number(e.target.value) })
                }
                fullWidth
              />
              <TextField
                label="ค่าน้ำ"
                type="number"
                value={form.waterAmount}
                onChange={(e) =>
                  setForm({ ...form, waterAmount: Number(e.target.value) })
                }
                fullWidth
              />
            </Stack>

            <Stack direction="row" spacing={2}>
              <TextField
                label="ค่าไฟ"
                type="number"
                value={form.electricAmount}
                onChange={(e) =>
                  setForm({ ...form, electricAmount: Number(e.target.value) })
                }
                fullWidth
              />
              <TextField
                label="ค่าส่วนกลาง"
                type="number"
                value={form.commonFeeAmount}
                onChange={(e) =>
                  setForm({ ...form, commonFeeAmount: Number(e.target.value) })
                }
                fullWidth
              />
            </Stack>

            <Stack direction="row" spacing={2}>
              <TextField
                label="ค่าปรับล่าช้า"
                type="number"
                value={form.lateFeeAmount}
                onChange={(e) =>
                  setForm({ ...form, lateFeeAmount: Number(e.target.value) })
                }
                fullWidth
              />
              <TextField
                label="เดือน"
                type="month"
                value={form.billingMonth}
                onChange={(e) =>
                  setForm({ ...form, billingMonth: e.target.value })
                }
                fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
              />
            </Stack>

            <TextField
              select
              label="สถานะ"
              value={form.billStatusId}
              onChange={(e) =>
                setForm({ ...form, billStatusId: Number(e.target.value) })
              }
              fullWidth
            >
              {statuses.map((s) => (
                <MenuItem key={s.id} value={s.id}>
                  {s.name}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>ยกเลิก</Button>
          <Button
            variant="contained"
            onClick={save}
            disabled={!form.roomId || !form.tenantId}
          >
            บันทึก
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
