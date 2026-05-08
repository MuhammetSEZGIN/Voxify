# Update Flow

Yeni sürüm yayınlamak için izlenecek adımlar.

## 1. Tauri ile build al

```bash
tauri build
```

`src-tauri/target/release/bundle/nsis/` altında şu dosyalar oluşur:
- `Voxify_1.0.0_x64-setup.nsis.zip`
- `Voxify_1.0.0_x64-setup.nsis.zip.sig`

## 2. GitHub Release oluştur

```bash
git tag v1.0.0
git push origin v1.0.0

gh release create v1.0.0 \
  Voxify_1.0.0_x64-setup.nsis.zip \
  Voxify_1.0.0_x64-setup.nsis.zip.sig \
  --title "v1.0.0" \
  --notes "Sürüm notları"
```

Yüklenen `.nsis.zip` dosyasının GitHub URL formatı:

```text
https://github.com/{kullanici}/{repo}/releases/download/v1.0.0/Voxify_1.0.0_x64-setup.nsis.zip
```

## 3. Signature değerini al

`.sig` dosyasının içeriğini kopyala — bu tek satır base64 string, DB'ye olduğu gibi girecek:

```bash
cat Voxify_1.0.0_x64-setup.nsis.zip.sig
# dW50cnVzdGVkIGNvbW1lbnQ6...
```

## 4. SQLite veritabanını güncelle

Sunucuda container'a bağlan:

```bash
docker exec -it versioncontrolservice sh
apt-get update && apt-get install -y sqlite3
sqlite3 /app/data/version_control.db
```

### İlk seed (boş veritabanı)

```sql
INSERT INTO Releases (Id, Version, Notes, PubDate, IsLatest)
VALUES (
    lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(6))),
    '1.3.0',
    'Yeni özellikler: Otomatik güncelleme',
    '2026-05-07T00:00:00Z',
    1
);

INSERT INTO ReleaseArtifacts (ReleaseId, Target, Signature, Url)
VALUES (
    (SELECT Id FROM Releases WHERE Version = '1.3.0'),
    'windows-x86_64',
    'dW50cnVzdGVkIGNvbW1lbnQ6IG1pbmlzaWduIHB1YmxpYyBrZXk6IDEzNzM2ODNFM0NBQzY1RUIKUldUclphdzhQbWh6RTdpRDZzSlFwRy83UkRjMllXcys0N3RVTk5sUE1ZUkN0UDdMdU9ZMzFlRTIK',
    'https://github.com/MuhammetSEZGIN/voxify-react/releases/download/v1.3.0/Voxify_1.3.0_x64-setup.nsis.zip'
);
```

### Sonraki sürüm güncellemesi

```sql
-- Eski sürümün latest flagini kaldır
UPDATE Releases SET IsLatest = 0 WHERE IsLatest = 1;

-- Yeni sürümü ekle
INSERT INTO Releases (Id, Version, Notes, PubDate, IsLatest)
VALUES (
    lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(2))) || '-' || lower(hex(randomblob(6))),
    '1.3.1',
    'Deneme amaçlı oluşturuldu',
    '2026-05-07T00:00:00Z',
    1
);

-- Artifact ekle
INSERT INTO ReleaseArtifacts (ReleaseId, Target, Signature, Url)
VALUES (
    (SELECT Id FROM Releases WHERE Version = '1.3.1'),
    'windows-x86_64',
    'dW50cnVzdGVkIGNvbW1lbnQ6IG1pbmlzaWduIHB1YmxpYyBrZXk6IDEzNzM2ODNFM0NBQzY1RUIKUldUclphdzhQbWh6RTdpRDZzSlFwRy83UkRjMllXcys0N3RVTk5sUE1ZUkN0UDdMdU9ZMzFlRTIK',
    'https://github.com/MuhammetSEZGIN/voxify-react/releases/download/v1.3.1/Voxify_1.3.1_x64-setup.nsis.zip'
);
```

### Kontrol

```sql
SELECT r.Version, r.IsLatest, a.Target, a.Url
FROM Releases r
JOIN ReleaseArtifacts a ON a.ReleaseId = r.Id;
```
