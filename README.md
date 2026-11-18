## Pokretanje

### Baza (iz korena repoa)

```cmd
docker compose up -d
```

Sa pgAdmin-om:

```cmd
docker compose --profile tools up -d
```

### Backend (iz foldera `backend`)

```cmd
dotnet build Sep.slnx && for %p in (Parking.Api Psp.Api Bank.Api) do start "%p" cmd /k "dotnet run --no-build --project src\%p --launch-profile https"
```

### Frontend (iz foldera `frontend`)

Prvi put (iz foldera `frontend`):

```cmd
mkdir .cert
dotnet dev-certs https --trust
dotnet dev-certs https --export-path .cert/localhost.pem --format Pem --no-password
```

```cmd
for %p in (parking-web psp-web bank-web) do start "%p" cmd /k "cd %p && npx ng serve"
```

## Adrese

| Servis      | HTTPS                  | HTTP                  |
| ----------- | ---------------------- | --------------------- |
| Parking.Api | https://localhost:7001 | http://localhost:5001 |
| Psp.Api     | https://localhost:7002 | http://localhost:5002 |
| Bank.Api    | https://localhost:7003 | http://localhost:5003 |
| parking-web | https://localhost:4201 |                       |
| psp-web     | https://localhost:4202 |                       |
| bank-web    | https://localhost:4203 |                       |
| pgAdmin     |                        | http://localhost:5050 |

## Baze

| Baza       | Korisnik     | Lozinka (`.env`)      |
| ---------- | ------------ | --------------------- |
| parking_db | parking_user | `PARKING_DB_PASSWORD` |
| psp_db     | psp_user     | `PSP_DB_PASSWORD`     |
| bank_db    | bank_user    | `BANK_DB_PASSWORD`    |

- Sa računara (aplikacija, DBeaver i sl.): `localhost:5434`
- Iz pgAdmin-a: host `postgres`, port `5432`, korisnik `postgres` (`POSTGRES_PASSWORD`)

psql u kontejneru:

```cmd
docker exec -it sep-postgres psql -U parking_user -d parking_db
docker exec -it sep-postgres psql -U psp_user -d psp_db
docker exec -it sep-postgres psql -U bank_user -d bank_db
```
