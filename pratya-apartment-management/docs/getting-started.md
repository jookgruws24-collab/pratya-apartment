# 1. เริ่มต้นใช้งาน (ติดตั้ง & รัน)

คู่มือนี้จะพารันโปรเจกต์ให้ขึ้นในเครื่องของคุณ มี 2 วิธี เลือกวิธีใดวิธีหนึ่ง:

- **วิธี A – Docker** (ง่ายสุด แนะนำสำหรับมือใหม่) รันคำสั่งเดียวได้ทุกอย่าง
- **วิธี B – รันแยกทีละส่วน** (เหมาะตอนกำลังพัฒนา เพราะแก้โค้ดแล้วเห็นผลทันที)

---

## วิธี A – รันด้วย Docker (คำสั่งเดียวจบ)

### สิ่งที่ต้องมี
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### ขั้นตอน

```bash
# อยู่ในโฟลเดอร์ pratya-apartment-management
cp .env.example .env          # คัดลอกไฟล์ค่า config (แก้รหัสผ่าน/JWT ได้ตามใจ)
docker compose up --build     # สร้าง image และรันทั้ง 3 ส่วน
```

Docker จะรันให้ 3 อย่าง:

| Service | คืออะไร | เปิดที่ |
|---------|---------|---------|
| `db` | ฐานข้อมูล PostgreSQL | localhost:5432 |
| `backend` | .NET Web API | <http://localhost:5034/swagger> |
| `frontend` | เว็บ React | <http://localhost:3000> |

> ตารางในฐานข้อมูลจะถูกสร้างอัตโนมัติตอน backend เริ่มทำงาน (ผ่าน EF Core Migrations)
> ไม่ต้องรันคำสั่งสร้างตารางเอง

### หยุดการทำงาน
กด `Ctrl + C` หรือรัน `docker compose down`
(ถ้าอยากลบข้อมูลในฐานข้อมูลด้วย ใช้ `docker compose down -v`)

---

## วิธี B – รันแยกทีละส่วน (สำหรับพัฒนา)

### สิ่งที่ต้องมี
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) เวอร์ชัน 20 ขึ้นไป
- PostgreSQL ที่รันอยู่ (จะลงเองหรือใช้ `docker compose up db` เฉพาะฐานข้อมูลก็ได้)

### 1) เตรียมฐานข้อมูล
แก้ connection string ในไฟล์
[`backend/src/API/appsettings.json`](../backend/src/API/appsettings.json)
ให้ตรงกับ PostgreSQL ของคุณ (Host, Port, Database, Username, Password)

### 2) รัน Backend

```bash
cd backend
dotnet run --project src/API/API.csproj
```

- API จะเปิดที่ <http://localhost:5034>
- หน้า Swagger (ไว้ทดลองเรียก API): <http://localhost:5034/swagger>
- ตารางฐานข้อมูลจะถูกสร้าง/อัปเดตอัตโนมัติตอนเริ่มโปรแกรม

### 3) รัน Frontend (เปิดอีก terminal หนึ่ง)

```bash
cd frontend
npm install        # ครั้งแรกครั้งเดียว
npm run dev
```

- เว็บจะเปิดที่ <http://localhost:5173> (ค่าเริ่มต้นของ Vite)
- ไฟล์ [`frontend/.env`](../frontend/.env) กำหนดว่า frontend จะเรียก backend ที่ url ไหน
  (ค่าเริ่มต้นคือ `http://localhost:5034`)

---

## ลองใช้งานครั้งแรก

1. เปิดเว็บ → กด **"สมัครสมาชิก"** สร้างบัญชี (เช่น admin / password123)
2. ระบบจะพาเข้าหน้า **แดชบอร์ด** อัตโนมัติ
3. ลองไปหน้า **ห้อง** เพิ่มห้องสัก 1 ห้อง
4. ไปหน้า **ผู้เช่า** เพิ่มผู้เช่า
5. ไปหน้า **บิล** สร้างบิล แล้วกด **"ออก PDF"** เพื่อดาวน์โหลดรายงาน
6. ไปหน้า **ไฟล์** ลองอัปโหลดรูปหรือ PDF

---

## เจอปัญหา?

| อาการ | สาเหตุ / วิธีแก้ |
|-------|-----------------|
| backend รันไม่ขึ้น ฟ้อง connection | PostgreSQL ยังไม่เปิด หรือ connection string ผิด |
| เว็บเรียก API ไม่ได้ (Network Error) | backend ยังไม่รัน หรือ url ใน `frontend/.env` ผิด |
| ล็อกอินแล้วเด้งออกตลอด | token หมดอายุ/ผิด ลองสมัคร-ล็อกอินใหม่ |
| อัปโหลดไฟล์ไม่ได้ | ไฟล์ใหญ่เกิน 5 MB หรือไม่ใช่ JPG/PNG/PDF |

อ่านต่อ: [สถาปัตยกรรม & โครงสร้างโค้ด →](architecture.md)
