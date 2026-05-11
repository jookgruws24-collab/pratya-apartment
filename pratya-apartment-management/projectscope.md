# Project Scope Definition

## Project Name
Pratya Apartment Management System

---

# Project Goal

Develop a full-stack web application for apartment management using modern cloud-native architecture.

The project must support:
- Frontend + Backend separation
- JWT Authentication
- File Upload
- PDF Generation
- PostgreSQL Database
- Docker Containerization
- CI/CD Deployment Pipeline
- Azure Cloud Deployment

The project is intended for learning:
- Full Stack Development
- Clean Architecture
- Docker
- GitHub Actions
- Azure Deployment
- DevOps Fundamentals

---

# Technical Stack

## Frontend

Framework:
- React
- Vite
- TypeScript

Libraries:
- MUI
- Chart.js
- Axios
- React Router DOM
- Secure Local Storage

Frontend Requirements:
- Responsive UI
- Authentication flow
- Dashboard page
- File upload page
- Chart visualization
- API integration

---

## Backend

Framework:
- .NET 10 Web API

Architecture:
- Clean Architecture

Libraries:
- Entity Framework Core
- Dapper
- JWT Authentication
- PDFSharp

Backend Requirements:
- REST API
- JWT Authentication
- CRUD APIs
- File Upload API
- PDF Export API
- PostgreSQL Integration
- Azure Blob Storage Integration

---

# Database

Database Engine:
- PostgreSQL

ORM:
- Entity Framework Core

Optional:
- Dapper for optimized queries

---

# Cloud Services

Azure Services:
- Azure Container Registry (ACR)
- Azure Container Apps
- Azure Database for PostgreSQL
- Azure Blob Storage

---

# Deployment Requirements

The application must:
- Run inside Linux containers
- Support Docker deployment
- Use GitHub Actions CI/CD pipeline
- Automatically build and deploy on push to main branch

---

# Authentication Requirements

Authentication Type:
- JWT Bearer Token

Requirements:
- Login API
- Register API
- Protected endpoints
- Token validation
- Frontend token storage
- Axios interceptor for token attachment

---

# File Upload Requirements

The system must support:
- Upload file from frontend
- Store file in Azure Blob Storage
- Save file metadata/path in PostgreSQL

Allowed file examples:
- PDF
- JPG
- PNG

Do NOT store files directly in database.

---

# PDF Requirements

The system must support:
- Generate PDF report
- Export payment or tenant information
- Use PDFSharp only

The PDF library must support Linux containers.

---

# Docker Requirements

The project must include:
- Frontend Dockerfile
- Backend Dockerfile
- docker-compose.yml

The application must run locally using Docker Compose.

---

# CI/CD Requirements

GitHub Actions pipeline must:
1. Build frontend image
2. Build backend image
3. Push images to Azure Container Registry
4. Deploy to Azure Container Apps

---

# Recommended Features

## Authentication
- Login
- Register
- Logout

## Dashboard
- Summary cards
- Payment statistics
- ChartJS graphs

## Tenant Management
- Create tenant
- Edit tenant
- Delete tenant
- View tenant list

## Payment Management
- Create payment
- Payment history
- Payment status

## File Upload
- Upload slip/documents
- View uploaded files

## PDF Export
- Export report PDF

---

# Non-Functional Requirements

- Clean code structure
- Environment variable support
- Production-ready Docker setup
- Linux container compatibility
- Proper folder separation
- Git version control
- API error handling
- Basic logging

---

# Out of Scope

The following are NOT required:
- Microservices
- Kubernetes
- Redis
- WebSocket
- Advanced RBAC
- Multi-tenant architecture
- Complex payment gateway
- Real-time notification
- Mobile application

---

# Folder Structure

frontend/
backend/

backend/src/
  API/
  Application/
  Domain/
  Infrastructure/

docker-compose.yml
README.md

---

# Development Rules

- Use TypeScript strictly
- Avoid Windows-only libraries
- Keep architecture simple and maintainable
- Prefer Docker-first development
- Keep secrets in environment variables
- Do not hardcode credentials
- Use async/await properly
- Follow REST API conventions

---

# AI Assistant Constraints

When helping with this project:

- Stay inside defined project scope
- Do not introduce unnecessary technologies
- Do not overengineer architecture
- Keep implementation beginner-to-intermediate friendly
- Prefer practical production-ready solutions
- Ensure Linux container compatibility
- Prioritize deployment compatibility
- Prioritize maintainability over complexity

---

# Primary Learning Objectives

This project is mainly focused on learning:

- GitHub workflow
- Docker
- CI/CD
- Azure deployment
- Clean Architecture
- JWT Authentication
- File Storage
- Full Stack Development
- Cloud-native deployment flow

---

# Primary Learning Objectives

This project is mainly focused on learning:

- GitHub workflow
- Docker
- CI/CD
- Azure deployment
- Clean Architecture
- JWT Authentication
- File Storage
- Full Stack Development
- Cloud-native deployment flow

---

# Environment Variables

Use environment variables for:
- JWT Secret
- Database Connection String
- Azure Blob Storage Connection String
- Azure Container Registry Credentials

Never hardcode secrets in source code.

---

# Simplicity Priority

Priority order:
1. Simplicity
2. Maintainability
3. Deployment readiness
4. Learning experience

Avoid enterprise-level complexity unless explicitly required.

---