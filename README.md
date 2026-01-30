# Span.Culturio Microservices

## Opis Projekta

Culturio je mikroservisna aplikacija koja povezuje kulturne ustanove (muzeji, galerije, kazališta) s korisnicima koji žele platiti mjesečnu pretplatu kako bi mogli posjetiti kulturne ustanove po povoljnijim cijenama.

Projekt je implementiran kao skup nezavisnih mikroservisa koji komuniciraju preko REST API-ja, s Entity Framework Core integracijom za upravljanje bazom podataka, JWT autentifikacijom za sigurnost i Seq distribuiranim logiranjem.

Aplikacija je dizajnirana za deployment u Kubernetes cluster (Minikube) s Ingress controllerom, ali može se pokrenuti i putem Docker Compose-a ili servis po servis bez Docker-a.

## Instalacija

```bash
git clone https://github.com/IvoGabud/Span.Culturio.Microservices.git
git checkout kubernetes-deployment
cd Span.Culturio.Microservices
```

## Pokretanje s Kubernetes (Minikube)

### Preduvjeti

- Minikube
- kubectl
- Docker

### Konfiguracija (opcionalno)

Moguće je prilagoditi konfiguraciju uređivanjem datoteka u `k8s/base/` direktoriju:

- **secrets.yaml** - lozinke i tajni ključevi (SA_PASSWORD, JWT_SECRET, Seq credentials)
- **configmap.yaml** - JWT postavke (issuer, audience, expiration)

Ako datoteke nisu uređene, koristit će se default vrijednosti prikladne za development okruženje.

### Pokretanje

```bash
./deploy.sh
```

**Napomene:** Na operacijskom sustavu Windows, skriptu je moguće pokrenuti koristeći alat Git bash ili neki njemu sličan. Izvođenje skripte može potrajati dulje vrijeme jer uključuje pokretanje Minikube-a i buildanje Docker slika.

Skripta pokreće Minikube, omogućuje Ingress addon, postavlja Docker environment na Minikube, builda Docker slike za sve mikroservise i primjenjuje Kubernetes manifeste iz `k8s/` direktorija. Po potrebi je moguće prilagoditi naredbe u skripti ili ih izvršiti samostalno.

Nakon završetka deploymenta, pokrenite `minikube tunnel` i pristupite aplikaciji na `http://localhost/swagger`.

**Objedinjeni Swagger UI:** U gornjem desnom kutu nalazi se **"Select a definition"** dropdown izbornik pomoću kojeg možete odabrati koji API želite testirati (Auth, Users, CultureObjects, Packages, Subscriptions).

### Pristup Seq logovima

Seq nije dostupan kroz Ingress jer je to admin/monitoring alat. Za pristup Seq-u koristite port-forward:

```bash
kubectl port-forward service/seq 5341:80 -n culturio
```

Seq UI je zatim dostupan na `http://localhost:5341` (login: `admin` / `Admin123!`).

### Brisanje deploymenta

```bash
kubectl delete namespace culturio
```

## Pokretanje s Docker Compose

### Preduvjeti

- Docker
- Docker-compose

### Konfiguracija (opcionalno)

Moguće je stvoriti `.env` datoteku prema `.env.example` predlošku za prilagodbu konfiguracije. Ako `.env` datoteka nije stvorena, koristit će se default vrijednosti.

```bash
cp .env.example .env
```

### Pokretanje

```bash
docker-compose up --build
```

Ova naredba pokreće:

- **SQL Server** - baza podataka (port 1433)
- **SQL Server Init** - inicijalizacija baza podataka (izvršava init-db.sql)
- **Auth Service** - autentifikacija (port 5001)
- **Users Service** - upravljanje korisnicima (port 5002)
- **CultureObjects Service** - kulturni objekti (port 5003)
- **Packages Service** - paketi pretplata (port 5004)
- **Subscriptions Service** - pretplate korisnika (port 5005)
- **API Gateway** - Ocelot gateway (port 5000)
- **Seq** - distribuirano logiranje (port 5341)

### API Gateway Endpoints

Svi servisi su dostupni kroz API Gateway na `http://localhost:5000`:

| Servis                 | Gateway Endpoint         |
| ---------------------- | ------------------------ |
| Auth Service           | `/api/auth/*`            |
| Users Service          | `/api/users/*`           |
| CultureObjects Service | `/api/culture-objects/*` |
| Packages Service       | `/api/packages/*`        |
| Subscriptions Service  | `/api/subscriptions/*`   |

**Objedinjeni Swagger UI:** http://localhost:5000/swagger - U gornjem desnom kutu nalazi se **"Select a definition"** dropdown izbornik pomoću kojeg možete odabrati koji API želite testirati (Auth, Users, CultureObjects, Packages, Subscriptions).

**Seq UI (logovi):** http://localhost:5341

### Zaustavljanje

```bash
docker-compose down
```

Zaustavljanje s brisanjem baze podataka:

```bash
docker-compose down -v
```

## Lokalno Pokretanje (bez Dockera)

### Preduvjeti

- .NET 8 SDK
- SQL Server

### Konfigurirajte User Secrets

Svi mikroservisi dijele **isti** User Secrets ID: `culturio-microservices-secrets`

Kreirajte `secrets.json` sa sljedećim sadržajem:

```json
{
  "ConnectionStrings": {
    "UsersConnection": "Server=localhost\\SQLEXPRESS;Database=Culturio.Users;Trusted_Connection=True;TrustServerCertificate=True",
    "CultureObjectsConnection": "Server=localhost\\SQLEXPRESS;Database=Culturio.CultureObjects;Trusted_Connection=True;TrustServerCertificate=True",
    "PackagesConnection": "Server=localhost\\SQLEXPRESS;Database=Culturio.Packages;Trusted_Connection=True;TrustServerCertificate=True",
    "SubscriptionsConnection": "Server=localhost\\SQLEXPRESS;Database=Culturio.Subscriptions;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "6f47f0b3c2efa549bfdd7b1e6bed694a"
  }
}
```

### Kreirajte baze podataka

Svaki mikroservis ima svoju zasebnu bazu podataka. Pokrenite migracije za svaki servis:

```bash
cd Span.Culturio.Auth
dotnet ef database update

cd ../Span.Culturio.CultureObjects
dotnet ef database update

cd ../Span.Culturio.Packages
dotnet ef database update

cd ../Span.Culturio.Subscriptions
dotnet ef database update
```

Baze će se automatski popuniti s testnim podatcima:

- **Culturio.Users**: Admin korisnik (username: `admin`, password: `Admin123!`)
- **Culturio.CultureObjects**: 3 kulturna objekta (Muzej Mimara, HNK, MSU)
- **Culturio.Packages**: 3 paketa (Osnovni, Premium, Godišnji) + PackageCultureObject relacije
- **Culturio.Subscriptions**: Prazna (pretplate se kreiraju kroz API)

### Pokrenite mikroservise

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
