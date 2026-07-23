# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository layout

This repo currently contains one real codebase, under `Web services/Login/`:

- `AdministracionPersonal.WebService.sln` — Visual Studio solution (.NET Framework 4.8.1, WCF).
- `AdministracionPersonal.WebService/` — the WCF service host (`ServicioAutenticacion.svc`), IIS/IIS Express web project. Depends on the three projects below.
- `AdministracionPersonal.WebService.Modelos/` — DTOs shared across the service boundary (`[DataContract]` classes) plus internal (non-serialized) models.
- `AdministracionPersonal.WebService.LogicaNegocio/` — business logic layer (`Servicios/`), including the AES-GCM password verification and the authentication rules.
- `AdministracionPersonal.WebService.AccesoDatos/` — data access layer (`Repositorios/`), MySQL via Dapper.
- `AdministracionPersonal.WebService.PruebasWeb/` — a small ASP.NET WebForms test harness (`Default.aspx`) that calls the service through a hand-built `ChannelFactory<IServicioAutenticacion>` client (no service reference needed).

`Web php/` currently only contains an empty placeholder file and has no code yet.

## Architecture

This is a classic 4-layer WCF service, one login/authentication use case end to end:

```
PruebasWeb (WebForms test client)
        │  ChannelFactory<IServicioAutenticacion> over BasicHttpBinding
        ▼
WebService (WCF host: IServicioAutenticacion / ServicioAutenticacion.svc.cs)
        │  delegates entirely to LogicaNegocio; the .svc class has no business rules
        ▼
LogicaNegocio (AutenticacionServicio, CriptografiaServicio)
        │  uses AccesoDatos repositories via their interfaces
        ▼
AccesoDatos (UsuarioRepositorio, BitacoraRepositorio via RepositorioBase) — MySQL (Dapper + MySqlConnector)
```

Key points to know before changing this code:

- **The `.svc.cs` class is a thin adapter.** `ServicioAutenticacion` only forwards to `IAutenticacionServicio`; it also wires up its own dependencies in `CrearServicio()` by reading `ConfigurationManager` (`AdministracionPersonalDb` connection string, `Security:AesKey`, `Sesion:DuracionMinutos` app settings). Business logic changes belong in `LogicaNegocio`, not here.
- **Password verification replicates an existing scheme, it isn't free to change.** `CriptografiaServicio` re-implements AES-256-GCM using BouncyCastle (because `System.Security.Cryptography.AesGcm` doesn't exist on .NET Framework 4.8). The stored format is `AESGCM:iv:tag:cipherdata` (base64 parts; 12-byte nonce, 16-byte tag) and **must** stay byte-for-byte compatible with a separate system ("Alcance 1") that writes these hashes — the `Security:AesKey` app setting must equal that system's `Encryption:Key`.
- **Authentication rules are deliberately preserved from a prior REST implementation** (see comments in `AutenticacionServicio`): empty fields and wrong credentials return the exact same message (`"Usuario y/o contraseña incorrectos."`); a non-`ACTIVO` user is rejected without counting an attempt or checking the password; 3 failed attempts flips the user to `BLOQUEADO`; every outcome (including "user doesn't exist" and technical errors) is written to `bitacora` via `IBitacoraRepositorio`. Any change to these rules should keep that traceability intact.
- **`UsuarioRepositorio.ObtenerPorUsuario` intentionally does not filter by `estado`** — it needs to distinguish "user not found" from "user found but blocked/inactive" for the audit log (`bitacora`).
- **`BitacoraRepositorio.Registrar` swallows its own exceptions** (logs via `Trace.TraceError`) so an audit-log failure never breaks the login flow.
- **The session `Token` is just an opaque GUID-derived string**, not a JWT — `AutenticacionServicio.GenerarToken()` is a placeholder marking "session started"; if real signed tokens are needed later, that's the only place to change.
- Internal DTOs like `UsuarioAutenticacion` (which carries `PasswordHash`) are plain classes without `[DataContract]` — never add serialization attributes to a type that carries a password hash, since only `CredencialesUsuario`/`ResultadoAutenticacion` should ever cross the service boundary.

## Build/run

This is a .NET Framework 4.8.1 WCF solution, built with MSBuild/Visual Studio (or `dotnet` if the SDK-style tooling is available), NuGet `PackageReference`s (Dapper, MySqlConnector, BouncyCastle.Cryptography) restored per-project. There is no CI config, test project, or lint config in the repo — `AdministracionPersonal.WebService.PruebasWeb` is a manual WebForms harness for exercising the service in a browser, not an automated test suite.

Configuration (connection string, AES key, session duration) lives in `Web.config` under `AdministracionPersonal.WebService/`, with `Web.Debug.config`/`Web.Release.config` transforms. Note the checked-in `Web.config` currently contains a live-looking MySQL connection string with a plaintext password — treat any config file in this repo as potentially sensitive before sharing or copying it elsewhere.
