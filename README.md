# Span.Culturio Microservices

## Opis Projekta

Culturio je mikroservisna aplikacija koja povezuje kulturne ustanove (muzeji, galerije, kazališta) s korisnicima koji žele platiti mjesečnu pretplatu kako bi mogli posjetiti kulturne ustanove po povoljnijim cijenama.

Projekt je implementiran kao skup nezavisnih mikroservisa koji komuniciraju preko REST API-ja, s Entity Framework Core integracijom za upravljanje bazom podataka, JWT autentifikacijom za sigurnost i Seq distribuiranim logiranjem.

## Pokretanje Projekta

### Preduvjeti

- .NET 8 SDK
- SQL Server
- Docker (ukoliko se želi koristiti Seq)

### Instalacija

#### 1. Klonirajte repozitorij

```bash
git clone https://github.com/IvoGabud/Span.Culturio.Microservices.git
cd Span.Culturio.Microservices
```

#### 2. Konfigurirajte User Secrets

Svi mikroservisi dijele **isti** User Secrets ID: `culturio-microservices-secrets`

Kreirajte `secrets.json` sa sljedećim sadržajem:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CulturioMicroservices;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "6f47f0b3c2efa549bfdd7b1e6bed694a"
  }
}
```

#### 3. Pokrenite Seq Server (Docker)

```bash
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_NOAUTHENTICATION=True -v seq-data:/data -p 5341:80 datalust/seq
```

**Seq Web UI:** http://localhost:5341

Loggovi sadrže Application svojstvo koje govori kojem mikroservisu pripadaju.

#### 4. Kreirajte bazu podataka

Pokrenite migracije iz **Shared projekta** (samo jednom):

```bash
cd Span.Culturio.Shared
dotnet ef database update --startup-project ../Span.Culturio.Auth
```

Baza će se automatski popuniti s testnim podatcima:

- **Admin korisnik**: username: `admin`, password: `Admin123!`
- **3 paketa**: Osnovni paket (30 dana), Premium paket (90 dana), Godišnji paket (365 dana)
- **3 kulturna objekta**: Muzej Mimara, Hrvatsko narodno kazalište, Muzej suvremene umjetnosti
- **9 PackageCultureObject** relacija sa definiranim brojem posjeta

#### 5. Pokrenite mikroservise

Svaki mikroservis se pokreće zasebno (`dotnet run` iz direktorija mikroservisa ili putem Visual Studia).

## API Endpoints - Upute za Testiranje

### 1. AUTH SERVICE (Port 5001)

**Swagger:** https://localhost:5001/swagger

#### POST `/auth/register` - Registracija novog korisnika

```json
{
  "firstName": "Test",
  "lastName": "User",
  "email": "test.user@example.com",
  "username": "testuser",
  "password": "Test123!@#"
}
```

#### POST `/auth/login` - Prijava korisnika

```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

---

### 2. USERS SERVICE (Port 5002) - Admin Only

**Swagger:** https://localhost:5002/swagger

#### GET `/users?page=1&pageSize=10` - Dohvati popis korisnika

**Query parametri:**

- `page`: `1` (default)
- `pageSize`: `10` (default, max 100)

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

**Autorizacija:** Samo Admin role

#### GET `/users/{id}` - Dohvati korisnika po ID-u

**Primjer:** `/users/1`

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

---

### 3. CULTURE OBJECTS SERVICE (Port 5003)

**Swagger:** https://localhost:5003/swagger

#### POST `/culture-objects` - Kreiraj novi kulturni objekt

```json
{
  "name": "Moderna galerija Zagreb",
  "contactEmail": "info@moderna.hr",
  "address": "Andrije Hebranga 1",
  "zipCode": 10000,
  "city": "Zagreb",
  "adminUserId": 1
}
```

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

#### GET `/culture-objects` - Dohvati sve kulturne objekte

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

#### GET `/culture-objects/{id}` - Dohvati kulturni objekt po ID-u

**Primjer:** `/culture-objects/1`

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

---

### 4. PACKAGES SERVICE (Port 5005)

**Swagger:** https://localhost:5005/swagger

#### GET `/packages` - Dohvati sve pakete s kulturnim objektima

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

---

### 5. SUBSCRIPTIONS SERVICE (Port 5004)

**Swagger:** https://localhost:5004/swagger

#### POST `/subscriptions` - Kreiraj novu pretplatu

```json
{
  "userId": 1,
  "packageId": 1,
  "name": "Moja muzejska kartica"
}
```

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

#### GET `/subscriptions?userId=1` - Dohvati pretplate

**Query parametri (opcionalno):**

- `userId`: `1` (filtriraj po korisniku)

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

#### POST `/subscriptions/activate` - Aktiviraj pretplatu

```json
{
  "subscriptionId": 1
}
```

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

#### POST `/subscriptions/track-visit` - Zabilježi posjetu kulturnom objektu

```json
{
  "subscriptionId": 1,
  "cultureObjectId": 1
}
```

**Headers:**

```
Authorization: Bearer {vaš-jwt-token}
```

## Validacija

Implementirana je validacija ulaznih parametara koristeći **FluentValidation** biblioteku.

**Validatori:**

- `RegisterUserDtoValidator`

  - FirstName: obavezan, max 100 znakova
  - LastName: obavezan, max 100 znakova
  - Email: obavezan, validan email format, max 255 znakova
  - Username: obavezan, max 100 znakova, samo slova/brojevi/underscore
  - Password: obavezan, min 8 znakova, max 255 znakova, mora sadržavati veliko slovo, malo slovo, broj i specijalni znak
  - Role: opcionalno, mora biti "User" ili "Admin"

- `LoginDtoValidator`

  - Username: obavezan, max 100 znakova
  - Password: obavezan, max 255 znakova

- `CreateCultureObjectDtoValidator`

  - Name: obavezan, max 100 znakova
  - ContactEmail: obavezan, validan email format, max 255 znakova
  - Address: obavezan, max 250 znakova
  - ZipCode: mora biti > 0, validan hrvatski poštanski broj (10000-99999)
  - City: obavezan, max 250 znakova
  - AdminUserId: mora biti > 0

- `CreateSubscriptionDtoValidator`

  - UserId: mora biti > 0
  - PackageId: mora biti > 0
  - Name: obavezan, max 100 znakova

- `ActivateSubscriptionDtoValidator`

  - SubscriptionId: mora biti > 0

- `TrackVisitDtoValidator`
  - SubscriptionId: mora biti > 0
  - CultureObjectId: mora biti > 0
