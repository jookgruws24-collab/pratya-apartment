# 2. สถาปัตยกรรม & โครงสร้างโค้ด

เอกสารนี้อธิบายว่าโค้ดแต่ละส่วน "ทำอะไร" และ "คุยกันยังไง" แบบเข้าใจง่าย

---

## ภาพรวมทั้งระบบ

```
[ เบราว์เซอร์ ]
      │  (ผู้ใช้กดปุ่มบนเว็บ)
      ▼
[ Frontend: React ]  ──── ส่ง HTTP request พร้อม JWT token ───▶  [ Backend: .NET API ]
                                                                       │
                                                                       ▼
                                                            [ PostgreSQL + ที่เก็บไฟล์ ]
```

- **Frontend** = หน้าตาเว็บที่ผู้ใช้เห็นและกดใช้งาน
- **Backend** = สมองของระบบ รับ request, เช็คสิทธิ์, อ่าน/เขียนฐานข้อมูล
- **ฐานข้อมูล** = ที่เก็บข้อมูลถาวร (ผู้เช่า, ห้อง, บิล, ผู้ใช้)

---

## Backend: Clean Architecture

Backend แบ่งเป็น 4 ชั้น (layer) แต่ละชั้นมีหน้าที่ชัดเจน และ **ชั้นในไม่รู้จักชั้นนอก**
ทำให้แก้ไข/ทดสอบง่าย นี่คือหัวใจของ "Clean Architecture"

```
┌─────────────────────────────────────────────┐
│  API            (Controllers, Program.cs)     │  ← ชั้นนอกสุด รับ request
│  ┌───────────────────────────────────────┐   │
│  │  Infrastructure  (DB, ไฟล์, JWT, PDF)  │   │  ← งานที่ต้องต่อโลกภายนอก
│  │  ┌─────────────────────────────────┐  │   │
│  │  │  Application  (DTOs, Interfaces) │  │   │  ← กติกา/สัญญา
│  │  │  ┌───────────────────────────┐  │  │   │
│  │  │  │  Domain  (Entities)       │  │  │   │  ← ชั้นในสุด ข้อมูลหลัก
│  │  │  └───────────────────────────┘  │  │   │
│  │  └─────────────────────────────────┘  │   │
│  └───────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

### 🟦 Domain — "ข้อมูลหลักคืออะไร"
โฟลเดอร์: [`backend/src/Domain/Entities`](../backend/src/Domain/Entities)

เก็บ **โครงสร้างข้อมูลหลัก** (Entities) เป็น class ธรรมดา ไม่มี logic ซับซ้อน
เช่น `Tenant` (ผู้เช่า), `Room` (ห้อง), `Bill` (บิล), `User` (ผู้ใช้)

### 🟩 Application — "กติกาและสัญญา"
โฟลเดอร์: [`backend/src/Application`](../backend/src/Application)

- **DTOs** (Data Transfer Objects) = รูปแบบข้อมูลที่รับ-ส่งกับ frontend
  (เช่น `CreateTenantDto` คือข้อมูลตอนสร้างผู้เช่า)
- **Interfaces** = "สัญญา" ว่าต้องมีเมธอดอะไรบ้าง แต่ยังไม่บอกวิธีทำ
  (เช่น `ITenantRepository` บอกว่าต้องมี `GetAllAsync()` แต่ไม่บอกว่าดึงจาก DB ยังไง)

> ทำไมต้องมี Interface? เพื่อให้ชั้น API เรียกใช้ผ่าน "สัญญา" โดยไม่ต้องรู้ราย
> ละเอียดว่าเบื้องหลังใช้ EF Core หรืออะไร → เปลี่ยนวิธีทำได้โดยไม่กระทบ Controller

### 🟨 Infrastructure — "ลงมือทำจริง"
โฟลเดอร์: [`backend/src/Infrastructure`](../backend/src/Infrastructure)

ที่นี่คือที่ที่ interface ถูกทำให้เป็นจริง (implementation):

- `Repositories/` → อ่าน/เขียนฐานข้อมูลด้วย **Entity Framework Core**
- `Persistence/ApplicationDbContext.cs` → ตัวเชื่อมฐานข้อมูล + ข้อมูลเริ่มต้น (seed)
- `Auth/` → สร้าง JWT token และ hash รหัสผ่าน (BCrypt)
- `Storage/` → บันทึกไฟล์ลง disk หรือ Azure Blob
- `Pdf/` → สร้างไฟล์ PDF ด้วย PDFsharp

### 🟥 API — "ประตูรับ request"
โฟลเดอร์: [`backend/src/API`](../backend/src/API)

- `Controllers/` → รับ HTTP request แล้วเรียก repository/service ที่เหมาะสม
- `Program.cs` → จุดเริ่มโปรแกรม ตั้งค่าทุกอย่าง (ฐานข้อมูล, JWT, CORS, การผูก interface กับ implementation)

---

## การเดินทางของ 1 request (ตัวอย่าง: ดูรายชื่อผู้เช่า)

```
เบราว์เซอร์  GET /api/tenant  (แนบ token)
   │
   ▼
TenantController.GetAll()            ← ชั้น API
   │  เรียกผ่าน interface
   ▼
ITenantRepository.GetAllAsync()      ← ชั้น Application (สัญญา)
   │  ตัวจริงคือ...
   ▼
TenantRepository.GetAllAsync()       ← ชั้น Infrastructure (ทำจริงด้วย EF Core)
   │  query ฐานข้อมูล
   ▼
PostgreSQL  → คืนรายชื่อผู้เช่า → ส่งกลับเป็น JSON ให้เบราว์เซอร์
```

จุดที่ "ผูก" interface เข้ากับตัวจริงอยู่ใน `Program.cs`:

```csharp
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
// แปลว่า: เมื่อใครขอ ITenantRepository ให้ส่ง TenantRepository ไปให้
```

เทคนิคนี้เรียกว่า **Dependency Injection (DI)** — Controller แค่ประกาศว่า
"ฉันต้องการ ITenantRepository" ระบบจะจัดหาตัวจริงมาให้เอง

---

## ระบบล็อกอิน (JWT) ทำงานยังไง

```
1. ผู้ใช้ล็อกอิน  → backend เช็ครหัสผ่าน (เทียบกับ hash ด้วย BCrypt)
2. ถ้าถูก        → backend สร้าง "token" (ข้อความยาว ๆ ที่เซ็นชื่อกำกับ) ส่งกลับ
3. frontend เก็บ token ไว้ แล้วแนบไปกับทุก request ถัดไป (Header: Authorization: Bearer <token>)
4. backend ตรวจ token ทุกครั้ง ถ้าถูกต้องและยังไม่หมดอายุ → อนุญาต
```

- ฝั่ง backend: ดู [`JwtTokenService.cs`](../backend/src/Infrastructure/Auth/JwtTokenService.cs)
  และการตั้งค่าตรวจ token ใน [`Program.cs`](../backend/src/API/Program.cs)
- Controller ที่ต้องล็อกอินก่อนจะมีป้าย `[Authorize]` กำกับไว้
- ฝั่ง frontend: [`api/client.ts`](../frontend/src/api/client.ts) มี **interceptor**
  ที่แนบ token ให้อัตโนมัติ และถ้าเจอ 401 (token หมดอายุ) จะเด้งกลับหน้า login

---

## การอัปโหลดไฟล์

```
frontend ส่งไฟล์  →  FileController.Upload()
                        │
                        ├─ 1) บันทึก "ตัวไฟล์" ลง disk (หรือ Azure Blob)  → ได้ url
                        └─ 2) บันทึก "ข้อมูลของไฟล์" (ชื่อ, ขนาด, url) ลงฐานข้อมูล
```

> **สำคัญ:** เราไม่เก็บตัวไฟล์ลงฐานข้อมูล เก็บแค่ "ที่อยู่ของไฟล์" (url) เท่านั้น
> ตัวไฟล์จริงอยู่บน disk/Blob — เป็นวิธีมาตรฐานที่ประหยัดและเร็วกว่า

การสลับระหว่าง disk กับ Azure Blob ทำได้แค่เปลี่ยนค่า `Storage:Provider`
(ดูรายละเอียดใน [deployment.md](deployment.md)) โดยไม่ต้องแก้โค้ด Controller เลย
เพราะทั้งคู่ทำตามสัญญาเดียวกันคือ `IFileStorageService`

---

## การออก PDF

[`PdfService.cs`](../backend/src/Infrastructure/Pdf/PdfService.cs) ใช้ **PDFsharp**
วาดตารางบิลลงบนหน้ากระดาษแล้วส่งกลับเป็นไฟล์

> เกร็ด: PDFsharp บน Linux ไม่รู้จักฟอนต์ในเครื่อง เราจึงมี `FileFontResolver`
> คอยบอกที่อยู่ไฟล์ฟอนต์ให้ (บน Windows ใช้ Arial, ใน Docker ใช้ DejaVu ที่ติดตั้งไว้)

---

## Frontend: โครงสร้าง

| โฟลเดอร์ | หน้าที่ |
|----------|---------|
| `src/api/` | ตัวเรียก API กลาง (axios) + แนบ token อัตโนมัติ |
| `src/auth/` | เก็บสถานะล็อกอิน (token, username) ด้วย React Context |
| `src/components/` | ส่วนที่ใช้ซ้ำ เช่น เมนูด้านบน (`Layout`), ตัวกันหน้า (`ProtectedRoute`) |
| `src/pages/` | หน้าต่าง ๆ: Login, Register, Dashboard, Tenants, Rooms, Bills, Files |
| `src/types.ts` | ชนิดข้อมูล (TypeScript) ที่ตรงกับที่ backend ส่งมา |
| `src/App.tsx` | กำหนดเส้นทาง (routing) ว่า url ไหนแสดงหน้าอะไร |

อ่านต่อ: [ลองเพิ่มฟีเจอร์ใหม่ทีละขั้น →](adding-a-feature.md)
