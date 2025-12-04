# 🏋️‍♂️ Spor Salonu Yönetim Sistemi (Gym Management System)

Bu proje, **Web Programlama** dersi proje ödevi kapsamında geliştirilmiş, ASP.NET Core MVC tabanlı kapsamlı bir spor salonu yönetim ve randevu sistemidir.

## 📋 Proje Hakkında
Sistem, spor salonu yöneticilerinin (Admin) hizmetleri ve antrenörleri kolayca yönetmesine, üyelerin ise antrenör uygunluk durumuna göre online randevu almasına olanak tanır. Projenin en önemli özelliği, **Yapay Zeka (AI)** desteği ile kullanıcılara fiziksel özelliklerine göre kişiselleştirilmiş egzersiz planları sunmasıdır.

## 🌟 Temel Özellikler

### 1. Yönetim ve CRUD İşlemleri
* **Hizmet Yönetimi:** Spor salonu hizmetlerinin (Örn: Yoga, Pilates, Fitness) eklenmesi, düzenlenmesi ve silinmesi.
* **Antrenör Yönetimi:** Antrenörlerin uzmanlık alanları ile sisteme kaydedilmesi.

### 2. Akıllı Randevu Sistemi
* Kullanıcılar, seçtikleri hizmet ve antrenör için randevu alabilirler.
* **Çakışma Kontrolü (Conflict Detection):** Sistem, aynı antrenöre aynı saatte ikinci bir randevu alınmasını otomatik olarak engeller.

### 3. 🤖 Yapay Zeka Koçu (AI Coach)
* Kullanıcılar; kilo, boy ve hedeflerini (Örn: Kilo verme, Kas yapma) girerek yapay zeka tarafından oluşturulan özel tavsiyeler ve planlar alabilirler.

### 4. REST API ve Raporlama
* Proje, veritabanındaki randevu verilerini filtrelemek ve listelemek için **LINQ** sorgularını kullanan bir REST API içerir (ReportingApiController).

### 5. Güvenlik ve Yetkilendirme
* **Admin Paneli:** Sadece yetkili yöneticiler hizmet ve antrenör ekleyebilir.
* **Üye Paneli:** Kayıtlı kullanıcılar randevu alabilir ve AI koçunu kullanabilir.

## ⚙️ Kullanılan Teknolojiler

* **Platform:** .NET 8.0 (ASP.NET Core MVC)
* **Veritabanı:** SQL Server (Entity Framework Core - Code First)
* **Front-End:** Bootstrap 5, HTML5, CSS3, JavaScript
* **Veri Erişim:** LINQ, EF Core

## 🚀 Kurulum ve Çalıştırma Adımları

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin:

1.  **Projeyi Klonlayın:**
    ```bash
    git clone [https://github.com/HuseyinHasanato/GymManagementSystem.git](https://github.com/HuseyinHasanato/GymManagementSystem.git)
    ```

2.  **Veritabanını Oluşturun:**
    * Projeyi Visual Studio'da açın.
    * **Package Manager Console** penceresini açın.
    * Aşağıdaki komutu çalıştırarak veritabanını oluşturun:
        ```powershell
        Update-Database
        ```

3.  **Projeyi Başlatın:**
    * Uygulamayı çalıştırın (Run).
    * Veritabanı Seed (DbSeeder) sayesinde Admin kullanıcısı otomatik oluşturulacaktır.
    * **Admin Giriş Bilgileri:**
        * **Email:** [ÖğrenciNumarası]@sakarya.edu.tr
        * **Şifre:** sau

## 👤 Hazırlayan

* **Ad Soyad:** Hüseyin Hasanato
* **Öğrenci No:** G211210581
* **Ders Grubu:** 2-C

---
