# 🛒 E-Commerce Platform

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Professional-green?logo=csharp)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-ORM-success)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-blue)
![License](https://img.shields.io/badge/License-MIT-yellow)
![Tests](https://img.shields.io/badge/Tests-xUnit%20%2B%20FakeItEasy-success)

> A scalable, feature-rich e-commerce platform with **real-time communication**, advanced analytics, flexible promotions, and secure payment integrations.

---

## 📌 Features

### 🏗 Architecture & Quality
- **Onion Architecture** for maintainability and scalability.
- **Unit Testing** with **xUnit** & **FakeItEasy**.
- **Entity Framework Core** for efficient ORM mapping.

### 💬 Real-Time Communication
- **SignalR**-powered live chat between customers and support.

### 📊 Admin Dashboard
- Manage **Orders, Offers, Products, Roles, Users**.
- Track **Total Revenue** & **Website Analytics**.

### 💡 Promotions & Offers
- Fixed & Percentage Discounts.
- Promo Codes.
- Buy-One-Get-One-Free deals.

### ❤️ Customer Experience
- Wishlist & Shopping Cart.
- Advanced Search (fuzzy + keyword relevance).
- Sorting by Price, Date, Rating.
- Ratings & Reviews with aggregated scores.

### 💳 Payments
- PayPal, Stripe, and Cash on Delivery.

### 📧 Email Notifications
- Registration confirmation.
- Payment receipts.
- New offer alerts.

---

## 🛠 Tech Stack
| Layer | Technology |
|-------|------------|
| **Backend** | C#, ASP.NET Core 8 |
| **Database** | SQL Server, EF Core |
| **Real-Time** | SignalR |
| **Testing** | xUnit, FakeItEasy |
| **Payments** | PayPal API, Stripe API |
| **Email** | SMTP |

---

## 📐 Architecture

[Client] → [ASP.NET Core MVC] → [Application Layer] → [Domain Layer] → [Infrastructure Layer] → [SQL Server]

*(Onion Architecture ensures separation of concerns and testability.)*

---

🧪 Testing

   AAA (Arrange–Act–Assert) pattern.

   In-memory database for safe testing.
---

## 🌐 Live Demo

🔗 [Souq Live](https://souq.runasp.net/)

---
## 📄 License

MIT License
