# PulseOps 🚀
> Real-Time Service Monitoring & Health Check Platform

PulseOps, mikroservislerin ve web servislerinin canlılığını (uptime), performansını ve metriklerini anlık takip eden, çoklu kiracı (Multi-Tenant) destekli modern bir izleme platformudur.

## 🛠️ Teknolojiler ve Mimari
- **Backend:** .NET 8 Web API
- **Mimari:** Clean Architecture (Domain, Application, Infrastructure, Presentation)
- **Veritabanı:** PostgreSQL (Docker containerized)
- **Güvenlik:** JWT (JSON Web Token), BCrypt Password Hashing
- **Dokümantasyon:** Swashbuckle / Swagger OpenAPI

## 🔒 Güvenlik & Özellikler
- **Multi-Tenant:** Organizasyon tabanlı kullanıcı yönetimi.
- **Güvenli Auth:** BCrypt ile şifrelenmiş parola saklama ve Claim tabanlı JWT doğrulama.
- **RESTful Endpoints:** Tam dokümante edilmiş `/api/Auth/register` ve `/api/Auth/login` süreçleri.

## 🚀 Projeyi Yerelde Çalıştırma

1. **PostgreSQL Container'ını Başlatın:**
   ```bash
   docker run --name pulseops-postg -e POSTGRES_PASSWORD=postgres -p 5433:5432 -d postgres