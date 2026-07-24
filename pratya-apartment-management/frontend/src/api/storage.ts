import secureImport from "react-secure-storage";

// react-secure-storage เป็นโมดูลแบบ CommonJS บางครั้ง bundler จะ import
// ได้เป็น object ที่ห่อ instance จริงไว้ใน .default อีกชั้น
// ตรงนี้จึงเลือกตัวที่ "มีเมธอด getItem จริง ๆ" ออกมาใช้ ให้ทำงานได้ทุกกรณี
type SecureStorage = {
  getItem: (key: string) => string | object | number | boolean | null;
  setItem: (
    key: string,
    value: string | object | number | boolean
  ) => void;
  removeItem: (key: string) => void;
  clear: () => void;
};

const candidate = secureImport as unknown as SecureStorage & {
  default?: SecureStorage;
};

const secureLocalStorage: SecureStorage =
  typeof candidate?.getItem === "function"
    ? candidate
    : (candidate.default as SecureStorage);

export default secureLocalStorage;
