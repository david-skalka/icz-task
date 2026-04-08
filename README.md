# ICZ Task — pohovorové zadání

Monorepo obsahuje **dvě části** technického zadání: REST API pro správu úkolů (backend) a SPA dashboard (frontend). Původní znění úkolů je v adresářích `Api/doc/` a `client/doc/`; implementační rozhodnutí a kontext pro review jsou v souborech `spec-notes.md` vedle nich.

## Struktura repozitáře

| Složka | Popis |
|--------|--------|
| [`Api/`](Api/) | ASP.NET Core webová aplikace (`IczTask`), integrační testy (`IczTaskTest`) |
| [`client/`](client/) | Angular SPA — přihlášení, tabulka úkolů, CRUD, volání API |

GitHub Actions ([`.github/workflows/build.yml`](.github/workflows/build.yml)) spouští lint a produkční build Angularu, zkopíruje výstup do `Api/IczTask/wwwroot`, pak `dotnet test` a `dotnet publish` — stejný model nasazení jako „API hostuje statiku“.

## Backend (`Api/`)

- **.NET 9** (v zadání je zmínka .NET 10 / Hybrid Cache u .NET 9; viz [`Api/doc/spec-notes.md`](Api/doc/spec-notes.md)).
- **EF Core** + **SQLite** (`main.db` dle connection stringu v `appsettings.json`), migrace při startu.
- **Úkoly (Task):** CRUD na `/api/tasks`, filtrování `?name=...`, validace (název min. 5 znaků atd.).
- **Hybrid Cache** (`Microsoft.Extensions.Caching.Hybrid`) — výchozí expirace 2 minuty, invalidace při změnách úkolů.
- **Swagger / OpenAPI** v Development prostředí.
- **Autentizace:** místo `X-Api-Key` z původního zadání je po **domluvě** použito **JWT** (`Authorization: Bearer`). Podrobnosti v [`Api/doc/spec-notes.md`](Api/doc/spec-notes.md).

### Lokální spuštění API

```bash
cd Api
dotnet run --project IczTask/IczTask.csproj
```

Výchozí HTTP profil naslouchá na **http://localhost:5278** (viz `IczTask/Properties/launchSettings.json`). Swagger: `http://localhost:5278/swagger` (v Development).

### Testovací přihlášení (JWT)

Endpoint `POST /api/auth/login` s tělem JSON:

```json
{ "login": "host", "password": "icz" }
```

Odpověď obsahuje token pro hlavičku `Authorization: Bearer <token>` u chráněných endpointů úkolů.

### Testy

```bash
cd Api
dotnet test IczTask.sln
```

## Frontend (`client/`)

- **Angular** (CLI ~20), **Angular Material**, **TypeScript**.
- **OpenAPI klient** generovaný do `src/app/api-client/` (skript `npm run update-api` v `package.json` — vyžaduje běžící API se Swaggerem).
- Přihlášení a uložení tokenu přes službu účtu; chování vůči původnímu zadání s API klíčem je popsáno v [`client/doc/spec-notes.md`](client/doc/spec-notes.md).

### Lokální vývoj (proxy na API)

```bash
cd client
npm ci
npm start
```

Aplikace běží typicky na **http://localhost:4200**; požadavky na `/api` jdou přes [`proxy.conf.json`](client/proxy.conf.json) na `http://localhost:5278`. Nejdřív tedy spusťte API, pak klienta.

### Build a lint

```bash
cd client
npm run lint
npm run build -- --configuration production
```

## Dokumentace zadání

- [Task Management REST API — zadání](Api/doc/spec.md)
- [Task Management Dashboard — zadání](client/doc/spec.md)
- [API — poznámky k implementaci](Api/doc/spec-notes.md)
- [Frontend — poznámky k implementaci](client/doc/spec-notes.md)
