# Kurbanlik Takip ve Dijital Pazarlik Otomasyon Sistemi (V1.0)

ASP.NET Core MVC mimarisi kullanılarak geliştirilmiş, canlı hayvan envanter yönetimini, borsa fiyat analitiğini ve alıcı-satıcı arasındaki pazarlık süreçlerini uçtan uca dijitalleştiren kurumsal bir ERP yazılımıdır.

Sistem, geleneksel hayvancılık ticaretindeki veri kayıplarını, küpe numarası karışıklıklarını ve mükerrer satış (Race Condition) risklerini ortadan kaldırmak amacıyla katı veri bütünlüğü kısıtları ve durum makineleri üzerine inşa edilmiştir.

---


---

## Teknolojik Altyapi (Tech Stack)

* Framework: .NET 10.0 (ASP.NET Core MVC)
* Programlama Dili: C# 12
* ORM / Veritabanı: Entity Framework Core 9.0 & SQL Server (MSSQL)
* Önyüz (Frontend): Bootstrap 5, Chart.js, ToastrJS, jQuery, FontAwesome 6

---

## One Cikan Muhendislik ve Mimari Cozumleri

1. Gram Bazli Hassas Agirlik Yonetimi: Kanatlı ve küçükbaş hayvanların yuvarlama hatası olmadan gramı gramına tutulabilmesi için veritabanında decimal(18,3) hassasiyeti mühürlenmiştir.
2. State Machine Pazarlik Dongusu: Masa tenisi mantığıyla çalışan sistemde, karşı teklif atıldığı an ilgili tarafın aksiyon butonları kilitlenerek sıranın karşı tarafa geçmesi ve mükerrer onay/ret işlemlerinin engellenmesi sağlanmıştır.
3. Muhendislik Karar Destek Sistemi (DSS): Hayvan ekleme ekranında, girilen ağırlığa ve kategori türüne göre borsa kurallarını koşturarak admine otomatik ideal taban fiyat önerisi sunan hevristik algoritma katmanı.
4. Veri Degismezligi (Data Immutability): Satışı onaylanmış ve sözleşmesi basılmış bir hayvanın kafa kağıdı verileri düzenlemeye ve silmeye karşı tamamen kilitlenerek denetim izi güvenliği sağlanmıştır.
5. Cascade Delete Koruması: İlişkisel veri tabanında zincirleme silme felaketlerini önlemek amacıyla, yazılım katmanında aktif bağımlılığı olan ana kategorilerin silinmesi engellenmiştir.

---

## Kurulum ve Calistirma (Setup Instructions)

### 1. Projeyi Klonlayın
```bash
git clone [https://github.com/Mtryn0748/Kurban_borsas-.git](https://github.com/Mtryn0748/Kurban_borsas-.git)
cd Kurban_borsas-
