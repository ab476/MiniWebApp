# 🧩 Dummy API Project Ideas (CRUD → Architecture Mastery)

This document lists **progressive dummy API ideas** designed to help you move from **basic CRUD** to **real-world, production-grade backend architecture**.  
Perfect for **ASP.NET Core, Node.js, or Spring**, and frontend consumers like **React / Next.js**.

---

## 🟢 Beginner – CRUD-Focused APIs

### 1. Todo / Task Manager API

#### Entities
- **Task**
  - `id`
  - `title`
  - `description`
  - `status`
  - `priority`
  - `dueDate`

#### Endpoints
- `GET /tasks`
- `POST /tasks`
- `PUT /tasks/{id}`
- `DELETE /tasks/{id}`

#### Practice
- Basic CRUD
- Request validation
- Pagination  
  - Example: `?page=1&pageSize=10`

---

### 2. Notes API

#### Entities
- **Note**
  - `id`
  - `title`
  - `content`
  - `tags[]`
  - `createdAt`

#### Extra Features
- Search by tag  
  - `GET /notes?tag=work`
- Soft delete (logical deletion)

---

### 3. Product Catalog API

#### Entities
- **Product**
  - `id`
  - `name`
  - `price`
  - `category`
  - `stock`

#### Practice
- Filtering  
  - `?minPrice=100`
- Sorting  
  - `?sort=price_desc`
- DTO vs Entity separation

---

## 🟡 Intermediate – Real-World API Patterns

### 4. User Authentication API

#### Entities
- **User**
  - `id`
  - `email`
  - `passwordHash`
  - `role`

#### Endpoints
- `POST /auth/register`
- `POST /auth/login`
- `GET /me` (JWT protected)

#### Practice
- JWT authentication
- Refresh tokens
- Password hashing

---

### 5. Blog / CMS API

#### Entities
- **Post**
- **Comment**
- **Author**

#### Features
- Nested resources  
  - `/posts/{id}/comments`
- Draft vs Published states
- Role-based access (admin / editor)

---

### 6. Expense Tracker API

#### Entities
- **Expense**
  - `amount`
  - `category`
  - `date`
  - `notes`

#### Practice
- Aggregations  
  - `/expenses/summary`
- Monthly reports
- Date-based filtering

---

## 🔵 Advanced – Industry-Level APIs

### 7. E-Commerce Order API

#### Entities
- User
- Product
- Cart
- Order
- OrderItem

#### Features
- Transaction handling
- Order status workflow
- Inventory reduction

#### Bonus
- Idempotent order creation  
  - `POST /orders`

---

### 8. Booking / Appointment API

#### Examples
- Doctor appointments
- Salon bookings
- Meeting room scheduling

#### Challenges
- Time-slot conflicts
- Concurrency handling
- Optimistic locking

---

### 9. Notification API

#### Entities
- **Notification**
  - `type`
  - `payload`
  - `isRead`

#### Practice
- Background jobs
- Webhooks
- Retry logic

---

## 🔴 Expert – Interview & Architecture Grade

### 10. Banking / Wallet API (Dummy)

#### Entities
- **Account**
- **Transaction**

#### Rules
- Debit / credit constraints
- Atomic balance updates
- Ledger-style storage

#### Practice
- Database transactions
- Event sourcing concepts

---

### 11. Rate-Limited Public API

#### Features
- API keys
- Rate limiting
  - `429 Too Many Requests`
- Usage tracking

---

### 12. Cache-Aware API

#### Add
- `ETag`
- `If-None-Match`
- `304 Not Modified`

#### Perfect For
- ASP.NET Core + React Query
- HTTP caching mastery

---

## 🧠 How to Level Up Any Dummy API

Add these features to **any** project above:

- ✅ Pagination
- ✅ Filtering & sorting
- ✅ Authentication & roles
- ✅ ETag / cache validation
- ✅ Swagger / OpenAPI
- ✅ Unit + integration tests

---

## 🔧 Tech Stack Suggestions (Optional)

- **Backend:** ASP.NET Core / Node.js / Spring Boot  
- **Database:** SQLite → PostgreSQL  
- **Frontend Consumer:** React / Next.js  
- **Client Caching:** React Query

---

> 💡 Tip:  
> Build **one API per level** instead of many small ones. Depth > breadth for interviews.
