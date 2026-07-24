// ชนิดข้อมูล (types) ที่ตรงกับที่ backend ส่งมา

export interface Status {
  id: number;
  name: string;
}

export interface Tenant {
  id: string;
  firstName: string;
  lastName: string;
  roomNumber: string;
}

export interface Room {
  id: string;
  roomNumber: string;
  floor: number;
  imageUrl?: string | null;
  roomStatusId: number;
  roomStatus?: Status | null;
}

export interface Bill {
  id: string;
  roomId: string;
  tenantId: string;
  rentAmount: number;
  waterAmount: number;
  electricAmount: number;
  commonFeeAmount: number;
  lateFeeAmount: number;
  totalAmount: number;
  billStatusId: number;
  billStatus?: Status | null;
  billingMonth: string;
  createdAt: string;
  room?: Room | null;
  tenant?: Tenant | null;
}

export interface UploadedFile {
  id: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  url: string;
  uploadedAt: string;
}

export interface DashboardSummary {
  totalRooms: number;
  totalTenants: number;
  totalBills: number;
  totalRevenue: number;
  unpaidAmount: number;
  monthlyRevenue: { month: string; amount: number }[];
}
