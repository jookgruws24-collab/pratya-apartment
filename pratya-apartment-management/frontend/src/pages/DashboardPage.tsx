import { useEffect, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  CircularProgress,
  Paper,
  Typography,
} from "@mui/material";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import { Bar } from "react-chartjs-2";
import api from "../api/client";
import type { DashboardSummary } from "../types";

// ต้องลงทะเบียนส่วนประกอบของ Chart.js ก่อนใช้งาน
ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

function StatCard({ label, value }: { label: string; value: string }) {
  return (
    <Card sx={{ flex: "1 1 180px", minWidth: 180 }}>
      <CardContent>
        <Typography color="text.secondary" variant="body2">
          {label}
        </Typography>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>
          {value}
        </Typography>
      </CardContent>
    </Card>
  );
}

export default function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api
      .get<DashboardSummary>("/api/dashboard/summary")
      .then((res) => setSummary(res.data))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!summary) {
    return <Typography>โหลดข้อมูลไม่สำเร็จ</Typography>;
  }

  const baht = (n: number) =>
    n.toLocaleString("th-TH", { style: "currency", currency: "THB" });

  const chartData = {
    labels: summary.monthlyRevenue.map((m) => m.month),
    datasets: [
      {
        label: "รายได้ (บาท)",
        data: summary.monthlyRevenue.map((m) => m.amount),
        backgroundColor: "#1976d2",
      },
    ],
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 700 }}>
        แดชบอร์ด
      </Typography>

      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 2, mb: 4 }}>
        <StatCard label="ห้องทั้งหมด" value={String(summary.totalRooms)} />
        <StatCard label="ผู้เช่าทั้งหมด" value={String(summary.totalTenants)} />
        <StatCard label="บิลทั้งหมด" value={String(summary.totalBills)} />
        <StatCard label="รายได้ (จ่ายแล้ว)" value={baht(summary.totalRevenue)} />
        <StatCard label="ยอดค้างชำระ" value={baht(summary.unpaidAmount)} />
      </Box>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          รายได้รายเดือน
        </Typography>
        {summary.monthlyRevenue.length === 0 ? (
          <Typography color="text.secondary">
            ยังไม่มีข้อมูลรายได้ (ลองเพิ่มบิลและตั้งสถานะเป็น Paid)
          </Typography>
        ) : (
          <Box sx={{ maxWidth: 700 }}>
            <Bar
              data={chartData}
              options={{ responsive: true, plugins: { legend: { display: false } } }}
            />
          </Box>
        )}
      </Paper>
    </Box>
  );
}
