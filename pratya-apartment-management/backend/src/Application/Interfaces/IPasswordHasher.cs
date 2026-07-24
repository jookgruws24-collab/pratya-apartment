namespace Application.Interfaces;

public interface IPasswordHasher
{
    // เปลี่ยนรหัสผ่านเป็นค่า hash เพื่อเก็บลงฐานข้อมูล
    string Hash(string password);

    // ตรวจว่ารหัสผ่านที่กรอกมา ตรงกับค่า hash ที่เก็บไว้ไหม
    bool Verify(string password, string passwordHash);
}
