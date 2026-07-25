# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository layout

- `Web services/Login/` — the .NET Framework 4.8.1 WCF solution (`AdministracionPersonal.WebService.sln`):
  - `AdministracionPersonal.WebService/` — the WCF service host, IIS/IIS Express web project. Depends on the three projects below. Hosts two services:
    - `ServicioAutenticacion.svc` / `IServicioAutenticacion` — login/authentication (see Architecture below).
    - `ServicioOferentes.svc` / `IServicioOferentes` — "Core2": given a `codigoPuesto` (job posting code), returns the candidates (`oferentes`) who meet that posting's requirements.
  - `AdministracionPersonal.WebService.Modelos/` — DTOs shared across the service boundary (`[DataContract]` classes: `CredencialesUsuario`, `ResultadoAutenticacion`, `OferenteApto`) plus internal (non-serialized) models (`UsuarioAutenticacion`).
  - `AdministracionPersonal.WebService.LogicaNegocio/` — business logic layer (`Servicios/`): `AutenticacionServicio` + `CriptografiaServicio` (login), `OferentesAptosServicio` (Core2).
  - `AdministracionPersonal.WebService.AccesoDatos/` — data access layer (`Repositorios/`), MySQL via Dapper: `UsuarioRepositorio`, `BitacoraRepositorio`, `OferenteRepositorio`, all via `RepositorioBase`.
  - `AdministracionPersonal.WebService.PruebasWeb/` — a small ASP.NET WebForms test harness (`Default.aspx`) that calls the service through a hand-built `ChannelFactory<IServicioAutenticacion>` client (no service reference needed).
- `Web php/` — a plain PHP front end for the login use case (no framework, no build step): `index.php` (login form) POSTs credentials, `data.php` (`autenticarWCF()`) hand-builds a raw SOAP envelope and calls `ServicioAutenticacion.svc` over cURL, `config.php` holds the WCF URL/SOAPAction/namespace constants, `bienvenida.php` is the post-login landing page gated on `$_SESSION['usuario']`, `logout.php` ends the session. This calls the WCF service directly over SOAP — it is not a service reference/generated client, so keep the envelope shape (`tem:`/`mod:` namespaces, `Autenticar` operation) in sync with `IServicioAutenticacion` by hand if that contract changes.
- `Prueba postman/` — a Postman collection for manually exercising `ServicioAutenticacion.svc`.

## Architecture

The login/authentication use case is a classic 4-layer WCF service, now with two front ends calling into it:

```
PruebasWeb (WebForms test client)          Web php (index.php → data.php)
  ChannelFactory<IServicioAutenticacion>      raw SOAP envelope via cURL
        over BasicHttpBinding                 to ServicioAutenticacion.svc
        └────────────────┬─────────────────────────┘
                          ▼
WebService (WCF host: IServicioAutenticacion / ServicioAutenticacion.svc.cs)
        │  delegates entirely to LogicaNegocio; the .svc class has no business rules
        ▼
LogicaNegocio (AutenticacionServicio, CriptografiaServicio)
        │  uses AccesoDatos repositories via their interfaces
        ▼
AccesoDatos (UsuarioRepositorio, BitacoraRepositorio via RepositorioBase) — MySQL (Dapper + MySqlConnector)
```

`ServicioOferentes` (Core2) follows the same pattern independently: `.svc.cs` → `OferentesAptosServicio` → `OferenteRepositorio`/`BitacoraRepositorio`, reading from a `vw_oferentes_aptos_puesto` MySQL view that already filters by postulación estado and requisito completion. It shares the `bitacora` audit convention (logs `SELECT`/`ERROR` entries) but has no authenticated-user context to log (`idUsuario` is always `null` in its `Bitacora()` calls, since Core2's acceptance criteria takes no token/user parameter).

Key points to know before changing this code:

- **The `.svc.cs` classes are thin adapters.** Both `ServicioAutenticacion` and `ServicioOferentes` only forward to their `LogicaNegocio` interface; they wire up their own dependencies in `CrearServicio()` by reading `ConfigurationManager` (`AdministracionPersonalDb` connection string, plus `Security:AesKey`/`Sesion:DuracionMinutos` for auth). Business logic changes belong in `LogicaNegocio`, not here.
- **Password verification replicates an existing scheme, it isn't free to change.** `CriptografiaServicio` re-implements AES-256-GCM using BouncyCastle (because `System.Security.Cryptography.AesGcm` doesn't exist on .NET Framework 4.8). The stored format is `AESGCM:iv:tag:cipherdata` (base64 parts; 12-byte nonce, 16-byte tag) and **must** stay byte-for-byte compatible with a separate system ("Alcance 1") that writes these hashes — the `Security:AesKey` app setting must equal that system's `Encryption:Key`.
- **Authentication rules are deliberately preserved from a prior REST implementation** (see comments in `AutenticacionServicio`): empty fields and wrong credentials return the exact same message (`"Usuario y/o contraseña incorrectos."`); a non-`ACTIVO` user is rejected without counting an attempt or checking the password; 3 failed attempts flips the user to `BLOQUEADO`; every outcome (including "user doesn't exist" and technical errors) is written to `bitacora` via `IBitacoraRepositorio`. Any change to these rules should keep that traceability intact, and `Web php/index.php` reuses the exact same generic message for its own empty-field check.
- **`UsuarioRepositorio.ObtenerPorUsuario` intentionally does not filter by `estado`** — it needs to distinguish "user not found" from "user found but blocked/inactive" for the audit log (`bitacora`).
- **`BitacoraRepositorio.Registrar` swallows its own exceptions** (logs via `Trace.TraceError`) so an audit-log failure never breaks the login flow.
- **The session `Token` is just an opaque GUID-derived string**, not a JWT — `AutenticacionServicio.GenerarToken()` is a placeholder marking "session started"; if real signed tokens are needed later, that's the only place to change.
- Internal DTOs like `UsuarioAutenticacion` (which carries `PasswordHash`) are plain classes without `[DataContract]` — never add serialization attributes to a type that carries a password hash, since only `CredencialesUsuario`/`ResultadoAutenticacion`/`OferenteApto` should ever cross the service boundary.
- **`OferenteApto` includes `IdOferente`** even though Core2's acceptance criteria only asks for name + identification — it's there because a later feature ("Core7"/"Core9", per the code comments) needs it to link through to a candidate detail view. Don't remove it as unused.

## Build/run

The WCF solution is .NET Framework 4.8.1, built with MSBuild/Visual Studio (or `dotnet` if the SDK-style tooling is available), NuGet `PackageReference`s (Dapper, MySqlConnector, BouncyCastle.Cryptography) restored per-project. There is no CI config, test project, or lint config in the repo — `AdministracionPersonal.WebService.PruebasWeb` is a manual WebForms harness for exercising the service in a browser, not an automated test suite. The `Prueba postman/` collection is another manual way to exercise `ServicioAutenticacion.svc` directly over SOAP.

`Web php/` has no build step or dependency manager — it's plain PHP requiring only a PHP runtime with `curl` and `dom` extensions (e.g. `php -S localhost:8000` from that directory) plus the WCF service already running at the URL hardcoded in `config.php` (`WCF_URL`, currently `http://localhost:62310/ServicioAutenticacion.svc`) — update that constant if the service's port changes.

Configuration (connection string, AES key, session duration) lives in `Web.config` under `AdministracionPersonal.WebService/`, with `Web.Debug.config`/`Web.Release.config` transforms. Note the checked-in `Web.config` currently contains a live-looking MySQL connection string with a plaintext password — treat any config file in this repo as potentially sensitive before sharing or copying it elsewhere.
