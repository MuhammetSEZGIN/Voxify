# Update Flow

Bu dosya, yeni bir sürüm yayınlamak istediğinde izlenecek adımları ve SQLite tarafında kullanılacak SQL sorgusunu içerir.

## 1. Release dosyalarını hazırla

Yeni sürüm dosyalarını `VersionControlService/wwwroot/releases` altına koy.

Örnek:
- `Voxify_1.3.1_x64-setup.nsis.zip`
- `Voxify_1.3.1_x64-setup.nsis.zip.sig`
- `Voxify_1.3.1_x86-setup.nsis.zip`
- `Voxify_1.3.1_x86-setup.nsis.zip.sig`

## 2. SQLite veritabanını güncelle (HER ZAMAN)

Yeni release kaydını `Releases` tablosuna, platform dosyalarını da `ReleaseArtifacts` tablosuna ekle.

**Database path:** `VersionControlService/Database/version_control.db`

### İlk seed (boş veritabanı) - SQL sorgusu:

```sql
BEGIN TRANSACTION;

INSERT INTO Releases (Id, Version, Notes, PubDate, IsLatest)
VALUES (
    '11111111-1111-1111-1111-111111111110',
    '1.3.0',
    'Yeni özellikler:\n- Bildirim desteği\n- Ses cihazı seçimi',
    '2026-05-06T00:00:00Z',
    1
);

INSERT INTO ReleaseArtifacts (Id, Target, Signature, Url, ReleaseId)
VALUES
(
    '22222222-2222-2222-2222-222222222220',
    'windows-x86_64',
    'base64-signature-from-sig-file',
    'http://localhost:5005/releases/Voxify_1.3.0_x64-setup.nsis.zip',
    '11111111-1111-1111-1111-111111111110'
),
(
    '22222222-2222-2222-2222-222222222221',
    'windows-x86',
    'base64-signature-from-sig-file',
    'http://localhost:5005/releases/Voxify_1.3.0_x86-setup.nsis.zip',
    '11111111-1111-1111-1111-111111111110'
);

COMMIT;
```

### Sonraki sürüm güncellemesi - SQL sorgusu:

```sql
BEGIN TRANSACTION;

-- Eski sürümün latest flagini kaldır
UPDATE Releases
SET IsLatest = 0
WHERE IsLatest = 1;

-- Yeni sürümü ekle
INSERT INTO Releases (Id, Version, Notes, PubDate, IsLatest)
VALUES (
    '11111111-1111-1111-1111-111111111111',
    '1.3.1',
    'Yeni özellikler ve hata düzeltmeleri',
    '2026-05-06T00:00:00Z',
    1
);

-- Platform artifactlarını ekle
INSERT INTO ReleaseArtifacts (Id, Target, Signature, Url, ReleaseId)
VALUES
(
    '22222222-2222-2222-2222-222222222222',
    'windows-x86_64',
    'base64-signature-from-sig-file',
    'http://localhost:5005/releases/Voxify_1.3.1_x64-setup.nsis.zip',
    '11111111-1111-1111-1111-111111111111'
),
(
    '22222222-2222-2222-2222-222222222223',
    'windows-x86',
    'base64-signature-from-sig-file',
    'http://localhost:5005/releases/Voxify_1.3.1_x86-setup.nsis.zip',
    '11111111-1111-1111-1111-111111111111'
);

COMMIT;
```

## 3. Uygulamayı çalıştır

```bash
cd VersionControlService
dotnet build
dotnet run
```

Uygulama açılınca:
- SQLite dosyası otomatik oluşturulur
- DB boşsa: "Database is empty. Manual seed required..." mesajı
- DB doluysa: Veriler DB'den okunur

## 4. Admin panel (gelecek)

Sonraki aşamada admin panel doğrudan DB'ye INSERT/UPDATE yapacak. Aynı SQL mantığı uygulanacak.

## Notlar

- ✅ `appsettings.json` artık release bilgisi içermiyor
- ✅ Her update'de sadece DB güncellemen gerekiyor
- ✅ Release dosyaları (.zip, .sig) diskten servis ediliyor
- ✅ Signature base64 text olarak DB'de saklanıyor
