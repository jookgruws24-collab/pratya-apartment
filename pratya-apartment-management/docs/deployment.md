# 4. Docker & การ Deploy ขึ้น Azure

บทนี้อธิบายว่าโปรเจกต์ถูกห่อเป็น container อย่างไร และแนวทางนำขึ้น Azure

---

## Docker คืออะไร (ฉบับย่อ)

Docker ช่วยห่อโปรแกรม + ทุกอย่างที่มันต้องใช้ ไว้ใน "กล่อง" (image) เดียว
ทำให้รันที่ไหนก็เหมือนกัน ไม่ต้องเจอปัญหา "เครื่องฉันรันได้ แต่เครื่องเธอรันไม่ได้"

โปรเจกต์นี้มี 3 กล่อง:
- **db** — PostgreSQL (ใช้ image สำเร็จรูป)
- **backend** — .NET API (สร้างจาก [`backend/Dockerfile`](../backend/Dockerfile))
- **frontend** — เว็บ React เสิร์ฟด้วย nginx (สร้างจาก [`frontend/Dockerfile`](../frontend/Dockerfile))

`docker-compose.yml` คือไฟล์ที่บอกว่าจะรันทั้ง 3 กล่องพร้อมกันยังไง และคุยกันยังไง

---

## Dockerfile ทำงานเป็นขั้น (multi-stage)

ทั้ง backend และ frontend ใช้เทคนิค **multi-stage build** คือ:

1. **ขั้น build** — ใช้ image ตัวใหญ่ (มีเครื่องมือครบ) คอมไพล์โค้ด
2. **ขั้น run** — คัดลอกเฉพาะผลลัพธ์ไปใส่ image ตัวเล็ก เพื่อให้ image สุดท้ายเบา

> เกร็ด backend: ใน Dockerfile มีการ `apt-get install fonts-dejavu-core`
> เพราะ PDFsharp ต้องใช้ไฟล์ฟอนต์จริงในการวาดตัวอักษรลง PDF บน Linux

---

## เรื่อง Environment Variable (ค่า config)

**ห้าม hardcode ความลับ** (รหัสผ่าน, JWT secret) ลงในโค้ด ให้ส่งผ่าน environment variable แทน

.NET อ่าน env var แบบใช้ `__` (ขีดล่างสองอัน) แทนจุดของ config เช่น:

| ค่าใน appsettings.json | Environment Variable |
|------------------------|----------------------|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` |
| `Jwt:Secret` | `Jwt__Secret` |
| `Storage:Provider` | `Storage__Provider` |

ดูตัวอย่างการส่งค่าเหล่านี้ได้ใน [`docker-compose.yml`](../docker-compose.yml)

---

## สลับที่เก็บไฟล์เป็น Azure Blob Storage

ตอน dev เก็บไฟล์ลง disk (`Storage:Provider = "Local"`)
ตอน deploy จริงควรเก็บบน Azure Blob โดยตั้งค่า env var:

```
Storage__Provider=Azure
Storage__AzureConnectionString=<connection string ของ storage account>
Storage__AzureContainer=uploads
```

โค้ดจะสลับไปใช้ `AzureBlobStorageService` ให้เองโดยไม่ต้องแก้ Controller

---

## CI/CD ด้วย GitHub Actions

ไฟล์ [`.github/workflows/deploy.yml`](../../.github/workflows/deploy.yml)
(อยู่ที่ราก repo) จะทำงานอัตโนมัติทุกครั้งที่ push ขึ้น branch `main`:

```
push ขึ้น main
   │
   ▼
1. build image ของ backend และ frontend
2. push image ขึ้น Azure Container Registry (ACR)
3. สั่ง Azure Container Apps ให้ใช้ image เวอร์ชันใหม่ (deploy)
```

### สิ่งที่ต้องเตรียมบน Azure (ทำครั้งเดียว)
1. สร้าง **Resource Group**
2. สร้าง **Azure Container Registry (ACR)** — ที่เก็บ image
3. สร้าง **Azure Database for PostgreSQL** — ฐานข้อมูลจริง
4. สร้าง **Azure Container Apps** 2 ตัว (backend + frontend)
5. (ถ้าใช้อัปโหลดไฟล์) สร้าง **Storage Account** + Blob Container

### Secrets ที่ต้องตั้งใน GitHub
ไปที่ repo → Settings → Secrets and variables → Actions แล้วเพิ่ม:

| ชื่อ Secret | คือค่าอะไร |
|-------------|-----------|
| `AZURE_CREDENTIALS` | ผลลัพธ์ JSON จาก `az ad sp create-for-rbac ... --sdk-auth` |
| `ACR_NAME` | ชื่อ registry เช่น `myregistry` |
| `ACR_LOGIN_SERVER` | เช่น `myregistry.azurecr.io` |
| `RESOURCE_GROUP` | ชื่อ resource group |
| `BACKEND_CONTAINERAPP` | ชื่อ container app ของ backend |
| `FRONTEND_CONTAINERAPP` | ชื่อ container app ของ frontend |
| `API_BASE_URL` | url ของ backend ที่ผู้ใช้เข้าถึง (ฝังตอน build frontend) |

> อย่าลืมตั้ง environment variable ของ backend บน Container Apps ด้วย
> (connection string ฐานข้อมูลจริง, `Jwt__Secret`, และค่า Azure Storage)

---

## เช็กลิสต์ก่อน deploy จริง

- [ ] เปลี่ยน `Jwt__Secret` เป็นค่าสุ่มยาว ๆ (ไม่ใช้ค่า default)
- [ ] เปลี่ยนรหัสผ่านฐานข้อมูลให้แข็งแรง
- [ ] ตั้ง `Storage__Provider=Azure` และใส่ connection string
- [ ] ตรวจว่าไม่มีไฟล์ `.env` หรือความลับหลุดขึ้น git
- [ ] ทดสอบ `docker compose up --build` ในเครื่องให้ผ่านก่อน

---

กลับไปหน้าแรก: [README](../README.md)
