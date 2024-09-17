# SecilStoreCase Konfigürasyon Yönetimi Projesi

Bu proje, farklı uygulamaların (Service-A, Service-B) merkezi bir veritabanından dinamik olarak konfigürasyonlarını yönetmelerini ve erişmelerini sağlar. Konfigürasyon değerleri uygulama yeniden başlatılmadan güncellenebilir ve periyodik olarak yenilenir.

## İçindekiler
- [Proje Özeti](#proje-özeti)
- [Mimari](#mimari)
- [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
- [ConfigurationReader Kütüphanesi](#configurationreader-kütüphanesi)
- [Kurulum ve Yükleme](#kurulum-ve-yükleme) 
- [API Endpoints](#api-endpoints)
- [Notlar](#notlar)

---

## Proje Özeti

SecilStoreCase projesi, birden fazla servis için dinamik konfigürasyon yönetimi sağlar. Service-A ve Service-B gibi uygulamalar, merkezi bir veritabanından konfigürasyonlarını alır ve sadece kendi `ApplicationName`'lerine ait kayıtları görüntüleyebilir. Proje, aşağıdaki bileşenlerden oluşur:
- **Service-A ve Service-B:** Konfigürasyon API'sini kullanarak konfigürasyonları yöneten iki istemci uygulaması.
- **Konfigürasyon API'si:** Merkezi bir API, konfigürasyonları yönetmek ve sunmak için kullanılır.
- **ConfigurationReader Kütüphanesi:** Diğer uygulamalara entegre edilebilen, bağımsız bir .NET kütüphanesi.

## Mimari

Proje, üç ana bileşenden oluşan modüler bir mimariyi takip eder:
1. **Configuration API:** Konfigürasyonları depolayan ve sağlayan merkezi API.
2. **ConfigurationReader:** Uygulamaların merkezi veritabanından dinamik olarak veri almasını sağlayan bir kütüphane.
3. **Service-A ve Service-B:** Bu servisler, Configuration API ile etkileşimde bulunarak kendi konfigürasyonlarını alır ve yönetir.

Her servis, kendi **ApplicationName**'i ile sadece kendi ilgili konfigürasyonlarını alır.

---

## Kullanılan Teknolojiler

- **.NET 8**
- **ASP.NET Core MVC** (Service-A ve Service-B için)
- **Entity Framework Core** (Veritabanı erişimi için)
- **Microsoft SQL Server** (Merkezi konfigürasyon depolaması)
- **AutoMapper** (DTO ve Entity dönüşümleri için)
- **Swagger** (API dokümantasyonu için)
- **Newtonsoft.Json** (JSON serileştirme işlemleri için)
- **Dependency Injection**
- **ADO.NET** (ConfigurationReader'da doğrudan SQL erişimi)

---

## ConfigurationReader Kütüphanesi

`ConfigurationReader` kütüphanesi, projelerin merkezi konfigürasyon veritabanından dinamik olarak veri almasını sağlar. Kütüphane şu özellikleri içerir:
- **Dinamik Konfigürasyon Çekme:** Belirli aralıklarla konfigürasyonlar yenilenir.
- **Merkezi Depolama:** Tüm konfigürasyon kayıtları tek bir SQL veritabanında tutulur.
- **Otomatik Yenileme:** Konfigürasyon değerleri periyodik olarak güncellenir.

### `ConfigurationReader` Kütüphanesi Nasıl Kullanılır:
```csharp
// ConfigurationReader'ı servise entegre et
var configurationReader = new ConfigurationReader(
    applicationName: "SERVICE-A", 
    connectionString: "your-connection-string", 
    refreshTimerIntervalInMs: 60000
);

// Tüm konfigürasyonları çek
var configs = await configurationReader.GetAllConfigurations();

// Bir anahtar üzerinden konfigürasyon değeri çek
var siteName = configurationReader.GetValue<string>("SiteName");
 ```

## Kurulum ve Yükleme

Proje bağımlılıklarını doğru şekilde kurduğunuzdan emin olun. Aşağıdaki adımları takip ederek kurulumu tamamlayabilirsiniz:

### 1. Bağımlılıkları Yükleme
Service-A ve Service-B gibi uygulamalara `ConfigurationReader` kütüphanesini entegre etmek için projelerinize `Microsoft.Data.SqlClient` paketini yükleyin.

```bash
dotnet add package Microsoft.Data.SqlClient --version 5.2.2
```
### 2. Veritabanı Bağlantı Dizesi Ekleme

Her uygulamanın `appsettings.json` dosyasına veritabanı bağlantı dizesini ekleyin.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-database-connection-string"
  },
  "ApplicationSettings": {
    "ApplicationName": "SERVICE-A"
  }
}
```

### 3. ConfigurationReader Kütüphanesini Entegre Etme

Service-A veya Service-B gibi projelerde, ConfigurationReader kütüphanesini şu şekilde kullanabilirsiniz:

```charp
var configurationReader = new ConfigurationReader(
    applicationName: "SERVICE-A", 
    connectionString: Configuration.GetConnectionString("DefaultConnection"), 
    refreshTimerIntervalInMs: 60000
);
```

## API Endpoints

Proje, merkezi bir Configuration API üzerinden konfigürasyon yönetimini sağlar. Bu API, CRUD işlemlerini destekler:

 - GET /api/configuration: Tüm aktif konfigürasyonları alır.
 - GET /api/configuration/{id}: Belirli bir konfigürasyon kaydını getirir.
 - POST /api/configuration: Yeni bir konfigürasyon ekler.
 - PUT /api/configuration/{id}: Mevcut bir konfigürasyonu günceller.
 - DELETE /api/configuration/{id}: Bir konfigürasyonu siler.

 ```bash
# Tüm konfigürasyonları çek
curl -X GET "https://localhost:7020/api/configuration"

# Yeni bir konfigürasyon ekle
curl -X POST "https://localhost:7020/api/configuration" \
    -H "Content-Type: application/json" \
    -d '{"Name": "MaxUserCount", "Type": "int", "Value": "100", "IsActive": true, "ApplicationName": "SERVICE-A"}'
```
## Notlar

 - Bağımlılıklar: Service-A ve Service-B projelerine Microsoft.Data.SqlClient 5.2.2 paketinin eklenmesi gerekmektedir.
 - Veritabanı Bağlantısı: Her proje, appsettings.json dosyasına veritabanı bağlantı dizesini eklemelidir.
 - Dinamik Güncellemeler: Konfigürasyonlar, belirlenen periyodik sürelerde otomatik olarak güncellenir.
 - ApplicationName Bazlı Erişim: Her servis yalnızca kendi ApplicationName'ine ait konfigürasyonları görebilir.







