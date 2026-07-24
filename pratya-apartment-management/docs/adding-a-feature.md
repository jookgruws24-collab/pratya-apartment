# 3. ลองเพิ่มฟีเจอร์ใหม่ทีละขั้น

วิธีที่ดีที่สุดในการเข้าใจโปรเจกต์คือ "ลองเพิ่มของเอง"
บทนี้จะพาเพิ่มฟีเจอร์ตัวอย่างแบบครบวงจร ตั้งแต่ฐานข้อมูลจนถึงหน้าเว็บ

> ตัวอย่าง: เพิ่มฟิลด์ **เบอร์โทร (Phone)** ให้กับผู้เช่า (Tenant)

การเพิ่มของใหม่ในระบบนี้มักเดินตาม 5 ขั้นเสมอ — จำ pattern นี้ไว้ใช้ได้ตลอด

---

## ภาพรวม 5 ขั้น

```
Domain → Application → Infrastructure → API → Frontend
(ข้อมูล)  (สัญญา/DTO)   (ทำจริง/DB)     (endpoint) (หน้าเว็บ)
```

---

## ขั้นที่ 1 — Domain: เพิ่มฟิลด์ในข้อมูลหลัก

แก้ไฟล์ [`backend/src/Domain/Entities/Tenant.cs`](../backend/src/Domain/Entities/Tenant.cs)

```csharp
public class Tenant
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;   // 👈 เพิ่มบรรทัดนี้
    public bool IsDeleted { get; set; } = false;
}
```

---

## ขั้นที่ 2 — Application: เพิ่มใน DTO

แก้ [`CreateTenantDto.cs`](../backend/src/Application/DTOs/CreateTenantDto.cs)
และ [`UpdateTenantDto.cs`](../backend/src/Application/DTOs/UpdateTenantDto.cs)
เพิ่มฟิลด์ `Phone` ในทั้งสองไฟล์:

```csharp
public string Phone { get; set; } = string.Empty;
```

> DTO คือ "รูปแบบข้อมูลที่รับจากผู้ใช้" แยกจาก Entity เพื่อความปลอดภัย
> (ผู้ใช้ไม่ควรส่งค่า `Id` หรือ `IsDeleted` มาเองได้)

---

## ขั้นที่ 3 — Infrastructure: สร้าง Migration

ฟิลด์ใหม่ต้องเพิ่มคอลัมน์ในฐานข้อมูลด้วย EF Core จะช่วยสร้างให้:

```bash
cd backend
dotnet ef migrations add AddPhoneToTenant \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/API/API.csproj
```

คำสั่งนี้จะสร้างไฟล์ migration ใหม่ (โค้ดที่บอกว่าจะเพิ่มคอลัมน์ `Phone`)
ตารางจริงจะถูกอัปเดต **อัตโนมัติตอน backend เริ่มทำงานครั้งถัดไป**
(เพราะใน `Program.cs` มี `db.Database.Migrate();`)

> 💡 ถ้ายังไม่มีคำสั่ง `dotnet ef` ให้ติดตั้งก่อน: `dotnet tool install --global dotnet-ef`

---

## ขั้นที่ 4 — API: รับค่าใน Controller

แก้ [`TenantController.cs`](../backend/src/API/Controllers/TenantController.cs)
ในเมธอด `Create` และ `Update` ให้ก็อปค่า `Phone` ลง entity:

```csharp
var tenant = new Tenant
{
    Id = Guid.NewGuid(),
    FirstName = dto.FirstName,
    LastName = dto.LastName,
    RoomNumber = dto.RoomNumber,
    Phone = dto.Phone            // 👈 เพิ่มบรรทัดนี้
};
```

ลองรัน backend แล้วทดสอบผ่าน Swagger (<http://localhost:5034/swagger>) ได้เลย

---

## ขั้นที่ 5 — Frontend: แสดงและกรอกในหน้าเว็บ

### 5.1 เพิ่มใน type
แก้ [`frontend/src/types.ts`](../frontend/src/types.ts)

```ts
export interface Tenant {
  id: string;
  firstName: string;
  lastName: string;
  roomNumber: string;
  phone: string;   // 👈 เพิ่ม
}
```

### 5.2 เพิ่มช่องกรอกและคอลัมน์ในตาราง
แก้ [`frontend/src/pages/TenantsPage.tsx`](../frontend/src/pages/TenantsPage.tsx)

- เพิ่ม `phone: ""` ในค่าเริ่มต้นของฟอร์ม (`emptyForm`)
- เพิ่ม `<TextField label="เบอร์โทร" ... />` ในกล่อง dialog
- เพิ่ม `<TableCell>เบอร์โทร</TableCell>` ในหัวตาราง และ `<TableCell>{t.phone}</TableCell>` ในแถวข้อมูล

เสร็จแล้ว! รีเฟรชหน้าเว็บก็จะเพิ่ม/แก้ไข/เห็นเบอร์โทรได้

---

## สรุป pattern ที่ใช้ซ้ำได้

| อยากทำอะไร | ต้องแตะไฟล์ไหนบ้าง |
|-----------|-------------------|
| เพิ่มฟิลด์ข้อมูล | Entity → DTO → Migration → Controller → Frontend |
| เพิ่ม endpoint ใหม่ | Interface → Repository → Controller (→ เรียกจาก Frontend) |
| เพิ่มหน้าเว็บใหม่ | สร้างไฟล์ใน `pages/` → เพิ่ม `<Route>` ใน `App.tsx` → เพิ่มเมนูใน `Layout.tsx` |

อ่านต่อ: [Docker & การ Deploy →](deployment.md)
