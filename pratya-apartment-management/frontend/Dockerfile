# ---------- ขั้นที่ 1: build ไฟล์ static ด้วย Vite ----------
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

# ที่อยู่ backend ถูกฝังตอน build (ต้องกำหนดผ่าน build arg)
ARG VITE_API_BASE_URL=http://localhost:5034
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY . .
RUN npm run build

# ---------- ขั้นที่ 2: เสิร์ฟไฟล์ด้วย nginx ----------
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
