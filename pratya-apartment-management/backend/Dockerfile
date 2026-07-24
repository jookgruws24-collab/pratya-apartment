# ---------- ขั้นที่ 1: build โปรเจกต์ ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# คัดลอกเฉพาะไฟล์ .csproj ก่อน เพื่อให้ Docker cache ขั้น restore ได้
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/API/API.csproj src/API/
RUN dotnet restore src/API/API.csproj

# คัดลอกโค้ดที่เหลือแล้ว publish
COPY . .
RUN dotnet publish src/API/API.csproj -c Release -o /app/publish

# ---------- ขั้นที่ 2: image สำหรับรันจริง (เล็กกว่า) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# ติดตั้งฟอนต์ DejaVu ไว้ให้ PDFsharp ใช้สร้าง PDF บน Linux
RUN apt-get update \
    && apt-get install -y --no-install-recommends fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# ฟังพอร์ต 8080 ภายใน container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]
