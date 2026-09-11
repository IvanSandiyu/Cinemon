# 🎬 Cinemon

Sistema de reserva de entradas de cine que combina un backend con arquitectura limpia y CQRS, y un frontend en Blazor Server. Nació como proyecto de aprendizaje y portafolio para practicar diseño backend en serio: separación de responsabilidades, comandos y consultas, autenticación basada en roles y una integración real con una API externa (TMDB).

> ?? **En desarrollo.** Proyecto en evoluci��n constante: Este no es el dise?o final, va a haber mejoras
> y nuevas funcionalidades en el futuro. 



![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=.net)
![Blazor Server](https://img.shields.io/badge/Blazor%20Server-8.0-512BD4?style=flat-square&logo=blazor)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0.28-512BD4?style=flat-square&logo=.net)
![MediatR](https://img.shields.io/badge/MediatR-14.2-03C4A1?style=flat-square)
![FluentValidation](https://img.shields.io/badge/FluentValidation-12.1-4FD16B?style=flat-square)
![Radzen Blazor](https://img.shields.io/badge/Radzen-11.3-00A8E1?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=flat-square&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat-square&logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger)
![TMDB](https://img.shields.io/badge/TMDB-integration-01B4E4?style=flat-square&logo=tmdb)

---

## 🧭 Índice

1. [Motivación y decisiones de diseño](#-motivación-y-decisiones-de-diseño)
2. [Funcionalidades principales](#-funcionalidades-principales)
3. [Tecnologías utilizadas](#-tecnologías-utilizadas)
4. [Arquitectura y estructura del proyecto](#-arquitectura-y-estructura-del-proyecto)
5. [Capturas](#-capturas)
6. [Cómo correrlo localmente](#-cómo-correrlo-localmente)
7. [Decisiones de seguridad](#-decisiones-de-seguridad-implementadas)
8. [Roadmap](#-posibles-mejoras-a-futuro)
9. [Licencia y contacto](#-licencia-y-contacto)

## 💡 Motivación y decisiones de diseño

Elegí el dominio de un **cine** porque es rico pero acotado: tiene entidades claras (películas, salas, funciones, butacas, reservas, usuarios), reglas de negocio concretas (una butaca no puede reservarse dos veces, una sala no puede tener funciones solapadas) y un flujo de usuario real que va desde explorar el catálogo hasta pagar una reserva. Es un terreno ideal para mostrar arquitectura sin que el dominio se trague el contenido.

Con eso en mente, quería practicar:

- **Clean Architecture y CQRS**: separar el dominio de los frameworks, y modelar cada caso de uso como un comando o una consulta explícita.
- **Autenticación y autorización por roles** con JWT, algo esencial en cualquier sistema real (Cliente vs. Admin).
- **Integración con una API externa** (TMDB) para no tener que inventar datos de películas y aprender a aislar esa dependencia.
- **Reglas de concurrencia reales**: dos personas reservando la misma butaca al mismo tiempo.

### Decisiones que tomé (y por qué)

- **CQRS con MediatR en vez de servicios tradicionales.** Commandos como "crear reserva" y queries como "traer funciones" tienen preocupaciones muy distintas: un comando muta estado y debe validar reglas de negocio al escribir; una query solo lee y no debería tener efectos secundarios. Separarlos hace cada pieza más chica, fácil de testear y fácil de leer. MediatR me da un pipeline donde encajo la validación con FluentValidation de forma transversal (vía `ValidationBehavior`), sin tocar un solo handler.
- **FluentValidation para las reglas de entrada.** Evitó que la validación se desperdigara entre el controlador, la capa de aplicación o el frontend. Cada comando tiene su validador y falla antes de llegar a la lógica de negocio.
- **Repository pattern + Unit of Work sobre EF Core.** Aunque EF Core ya es un repositorio, acoté el acceso a datos detrás de interfaces en `Application` para que el dominio y los casos de uso no dependan de EF Core ni de SQL Server. Si mañana cambia el motor, solo se toca `Infrastructure`.
- **Blazor Server + Radzen para el frontend.** Quería un solo lenguaje (C#) en toda la solución, y Blazor Server me dejó reutilizar DTOs y modelos mentales desde el backend sin escribir JavaScript. Radzen aporta componentes (tablas, botones, navegación, temas) que aceleran el panel de administración.
- **Minimal API para los endpoints.** Para un portfolio, las rutas quedan legibles y agrupadas por recurso (`MapReservaEndpoints()`, `MapPeliculaEndpoint()`...), con la autorización declarada en el mismo lugar.
- **Migraciones automáticas al iniciar** (`Database.MigrateAsync()` en el seed): al clonar y correr la API, la base se crea sola. Cómodo para demo, aunque en producción preferiría migraciones explícitas.

### Un desafío puntual: la butaca doble-reservada 👇

El peor caso de concurrencia del sistema es que dos usuarios seleccionen la misma butaca y reserven al mismo tiempo. Lo resolví con **tres capas que se refuerzan entre sí**:

1. **Validación previa**: el handler de creación consulta si esas butacas siguen libres antes de insertar (`ButacasDisponiblesAsync`).
2. **Constraint única en la base de datos**: `ReservasButacas` tiene un índice único sobre `(FuncionId, ButacaId)`, así que físicamente es imposible reservar dos veces la misma butaca para la misma función, sin importar cuántos procesos entren en simultáneo.
3. **Transacción + manejo de excepción**: si dos requests concurrentes pasan la validación a la vez, el que pierde la carrera recibe un `DbUpdateException` que se transforma en `ConflictException` (409) y se devuelve un 409 claro al frontend con el mensaje "Una o más butacas ya están reservadas".

También probé que el frontend muestre las butacas ya ocupadas (vía un DTO con `Ocupada`) para que la selección visual refleje el estado real. Una regla análoga evita funciones solapadas en la misma sala consultando la superposición antes de crear la función.

## 🧩 Funcionalidades principales

**Para el usuario (Rol Cliente)**

- 📝 **Registro y login** con contraseñas hasheadas y sesión persistida (JWT en el navegador).
- 🎞�?**Cartelera** con películas activas: sinopsis, duración, clasificación (ATP/+13/+16/+18), géneros, tráiler y afiches traídos de TMDB.
- 📅 **Próximos estrenos** y detalle de película.
- 🎟�?**Selección visual de butacas** por función: mapa interactivo de la sala, butacas ocupadas en gris, y precio/idioma/formato únicos por función.
- �?**Reserva de entradas** y pantalla de confirmación con el detalle.
- 📖 **Historial de reservas** ("Mis compras") con opción de cancelar y liberar las butacas.
- 👤 **Mi cuenta** con datos personales y cierre de sesión.

**Para el administrador (Rol Admin)**

- 🎬 **Gestión de películas**: alta manual o buscando en **TMDB** (afiche, sinopsis y duración se completan solos), edición, activar/desactivar.
- 🕒 **Gestión de funciones**: crear, editar, finalizar y cancelar; evita solapadas en la misma sala y 3D en salas IMAX.
- 🏷�?**Gestión de salas** (con butacas generadas por tamaño: estándar e IMAX).
- 📋 **Panel de reservas**: filtros por sala, función y estado, búsqueda por cliente o película.
- 👥 **Panel de usuarios**: búsqueda, filtros por rol/estado y detalle de sus reservas.
- 🔑 **Registro de administradores** (solo un admin existente puede crear otro).

## 🛠�?Tecnologías utilizadas

### Backend �?`Cinemon.Api` / `Cinemon.Application`
- **.NET 8** (SDK 8.0.410, fijado en `global.json`) �?ASP.NET Core **Minimal API**.
- **CQRS con MediatR 14.2.0** �?comandos y queries por caso de uso.
- **FluentValidation 12.1.1** �?validación transversal vía pipeline behavior.
- **Autenticación JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.28) �?HMAC-SHA256, issuer `Cinemon`, audiencia `CinemonApi`.
- **Hashing de contraseñas** con `PasswordHasher<object>` de ASP.NET Core Identity.
- **Swagger / OpenAPI** (Swashbuckle 6.6.2) con esquema de autorización Bearer, activo en Desarrollo.
- Manejador global de excepciones que traduce errores de dominio a códigos HTTP (`400/401/404/409`).

### Frontend �?`Cinemon.Web`
- **Blazor Server** (interactive, prerrenderizado desactivado en páginas con sesión).
- **Radzen Blazor 11.3.0** �?componentes UI de main layout, tablas, botones, temas (`material`).
- **Bootstrap 5** para estilos adicionales + **Font Awesome** para iconos.
- Consumo de la API vía `HttpClient` con token Bearer adjuntado por servicio.

### Base de datos �?`Cinemon.Infrastructure`
- **SQL Server** con **Entity Framework Core 8.0.28** (`Microsoft.EntityFrameworkCore.SqlServer`).
- Migraciones EF (`InitialMigration` �?`AddPeliculaGenero` �?`AddUsuarioPasswordHash` �?`AddTmdbId` �?`AddTmdbPosterAndBackdropPaths`).
- Seed inicial: 5 géneros, 4 salas (2 estándar + 1 estándar grande + 1 IMAX) y sus butacas (80, 120, 150 y 220).

### Integraciones externas
- **TMDB API** (`api.themoviedb.org/3`) �?búsqueda, detalle, clasificación por país (AR/US) e imágenes (poster `w500`, backdrop original).

### Testing / Herramientas
- **xUnit**: aún no hay tests. Muy pendiente en el [roadmap](#-posibles-mejoras-a-futuro) (expliqué el *approach* en la sección de la butaca doble-reservada).
- **Swagger** como documentación interactiva de la API.
- **`dotnet user-secrets`** para los secretos locales (no hay claves en el repo).

## 🏛�?Arquitectura y estructura del proyecto

### Clean Architecture

Mantuve la solución en cinco proyectos separados siguiendo el estilo "onion" / Clean Architecture: la lógica de negocio vive en el centro, y las capas externas (Web, base de datos, integraciones) dependen *hacia adentro* pero no al revés. Así, el **dominio** y los **casos de uso** no saben nada de EF Core, de SQL Server ni de la Web, y los cambios de framework quedan confinados a una sola capa.

```
                  ┌─────────────────────────────�?                  �?          Cinemon.Web       �? Blazor Server + Radzen (solo UI)
                  └──────────────┬──────────────�?                                 �? HTTP + JWT
                  ┌──────────────▼──────────────�?                  �?    Cinemon.Api             �? Minimal API, Swagger, auth/roles
                  └──────────────┬──────────────�?                                 �? CQRS (MediatR)
                  ┌──────────────▼──────────────�?                  �?  Cinemon.Application       �? Comandos/Queries, validación, DTOs
                  └───────┬──────────────┬──────�?                          �?             �?                 ┌────────▼───────�? ┌───▼────────────────────────�?                 �?Cinemon.Domain �? �?Cinemon.Infrastructure     �? EF Core(SQL), JWT,
                 �?Entidades,     �? �?repos, seed, TMDB, hashing �? password hashing
                 �?enums, reglas  �? └────────────────────────────�?                 └────────────────�?```

Regla de dependencias: **Domain** no depende de nada; **Application** �?Domain; **Infrastructure** �?Application y Domain; **Api** �?Application e Infrastructure; **Web** consume la Api por HTTP.

### Árbol de carpetas real

```
Cinemon.sln
├── Cinemon.Domain/                  # Núcleo: sin dependencias externas
�?  ├── Entidades/                   # Pelicula, Sala, Butaca, Funcion, Reserva, Usuario, ...
�?  �?  ├── Butacas/  Funcion/  Generos/  Peliculas/  Reservas/  Salas/  Usuarios/
�?  ├── Enums/                       # Rol, IdiomaFuncion, Formato, EstadoFuncion, TipoSala...
�?  ├── Exceptions/                  # BusinessRuleException, ConflictException, ...
�?  └── Interfaces/
├── Cinemon.Application/             # Casos de uso (CQRS + validación)
�?  ├── Abstractions/                # I*Repository, IUnitOfWork, ICurrentUserService
�?  ├── Behaviors/                   # ValidationBehavior (pipeline / validation)
�?  ├── Commands + Queries por feature:
�?  �?  ├── Funciones/  Peliculas/  Reservas/  Usuarios/  Butacas/  Tmdb/
�?  ├── DTOs/                        # Respuestas para la API
�?  └── Interfaces/
├── Cinemon.Infrastructure/          # Adaptadores: EF Core, seguridad, TMDB
�?  ├── CinemonDbContext.cs
�?  ├── Configurations/              # Mapping de entidades (índices únicos, FKs)
�?  ├── Repositories/                # Implementaciones EF Core de las interfaces
�?  ├── Authentication/              # JwtTokenService, PasswordService
�?  ├── ExternalServices/Tmdb/       # HttpClient + mapeo a TMDB
�?  ├── Migrations/                  # Migraciones de la base de datos
�?  └── Seed/                        # Géneros, salas y butacas iniciales
├── Cinemon.Api/                     # Presentación de la API (Minimal API)
�?  ├── Endpoints/                   # Peliculas, Funciones, Salas, Butacas, Reservas, ...
�?  ├── Middleware/                  # GlobalExceptionHandler �?ProblemDetails
�?  ├── Services/                    # CurrentUserService
�?  └── Program.cs                   # DI, JWT, Swagger, seed automático
├── Cinemon.Web/                     # Frontend (Blazor Server)
�?  ├── Components/
�?  �?  ├── Layout/                  # MainLayout (Radzen), AccountArea, AdminGuard
�?  �?  ├── Pages/                   # Cartelera, Proximos Estrenos, Login, Butacas,
�?  �?  �?  └── Admin/               #   Películas, Funciones, Salas, Reservas, Usuarios
�?  �?  └── Shared/                  # Carátulas de película, etc.
�?  ├── Models/DTOs/                 # DTOs del frontend
�?  ├── Services/                    # ApiServices por recurso + AuthService
�?  └── wwwroot/                     # CSS, Bootstrap, JS, favicon
└── global.json                      # SDK .NET 8.0.410 fijo
```

## 📸 Capturas

### Home
![home](docs/screenshots/home.png)

### Cartelera
![Cartelera](docs/screenshots/cartelera_1.png)

### Seleccion pelicula
![Seleccion_pelicula](docs/screenshots/pelicula_home.png)

### Seleccion funcion
![Funcion_pelicula](docs/screenshots/funcion_pelicula.png)

### Login
![Login](docs/screenshots/login.png)

### Selección de butacas
![Selección de butacas](docs/screenshots/reserva_pelicula.png)

### Confirmación de reserva
![Reserva confirmada](docs/screenshots/confirmar_reserva.png)

### Reserva confirmada
![Reserva confirmada](docs/screenshots/reserva_confirmada.png)

### Mis compras
![Mis compras](docs/screenshots/mis_compras.png)

### Panel admin
![Panel admin](docs/screenshots/panel_admin.png)

### Panel de administración (películas)
![Panel admin películas](docs/screenshots/peliculas_admin.png)

### Crear pel��cula
![Crear pel��cula](docs/screenshots/crear_pelicula.png)

### Crear pel��cula con datos
![Crear pelicula con datos](docs/screenshots/crear_pelicula_2.png)

### Pel��cula creada
![Pelicula creada](docs/screenshots/pelicula_creada.png)

### Editar pel��cula
![Editar pel��cula](docs/screenshots/editar_pelicula.png)

### Crear funci��n
![Crear funci��n](docs/screenshots/crear_funcion.png)

### Antes de avanzar, mostramos las funciones que existieron, existen y existiran. Se pueden filtrar
![Funciones](docs/screenshots/ver_funcion.png)

### Podemos buscar funciones
![Funci��n pelicula](docs/screenshots/ver_funcion_pelicula.png)

### Podemos ver todas las reservas, tambien filtrando por sala y pel��culas
![Reservas](docs/screenshots/ver_reservas.png)

### Y tambien podemos ver los usuarios
![Usuarios](docs/screenshots/ver_usuarios.png)

### Salas - todavia no se ha implementado nada.

### Cartelera nueva
![Cartelera nueva](docs/screenshots/en_cartelera.png)

### Home pel��cula agregada
![Home pelicula creada](docs/screenshots/pelicula_creada_funcion.png)

### Funcion pel��cula agregada
![Pelicula creada](docs/screenshots/reservar_pelicula.png)

### Seleccionar butacas y realizamos la compra
![Seleccionar butacas](docs/screenshots/seleccionar_butacas.png)

### Verificamos en otra compra las butacas
![Verificacion butacas](docs/screenshots/verificar_butacas.png)

### Pr��ximos estrenos
![Pr��ximos estrenos](docs/screenshots/prox_estrenos.png)

### Candy - todav��a no se ha implementado nada

## 🚀 Cómo correrlo localmente

### Prerrequisitos

- **.NET 8 SDK** (el repo pinnea `8.0.410` vía `global.json`).
- **SQL Server** (LocalDB, Developer, Docker, lo que uses).
- Una **cuenta/API key de TMDB** para el access token (gratuita en [themoviedb.org](https://www.themoviedb.org/settings/api)).
- (Opcional) .NET EF CLI para comandos de migración: `dotnet tool install --global dotnet-ef`.

### Pasos

**1. Clonar el repositorio**

```bash
git clone <url-del-repo>
cd Cinemon
```

**2. Configurar los secretos con `dotnet user-secrets`**

Los valores sensibles (`connection string`, `Jwt:Key`, `Tmdb:AccessToken`) están intencionalmente vacíos en `appsettings.json` y se resuelven desde user-secrets. Ejecutá esto dentro de la carpeta `Cinemon.Api`:

```bash
cd Cinemon.Api

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CinemonDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "una-clave-aleatoria-larga-para-firmar-los-tokens"
dotnet user-secrets set "Tmdb:AccessToken" "tu-access-token-de-tmdb"
```

> El proyecto ya tiene un `UserSecretsId` definido, así que el archivo local `secrets.json` queda fuera del control de versiones (ver `.gitignore`).

**3. Levantar la base de datos**

La API aplica las migraciones automáticamente en el arranque (vía seed), así que con solo correrla la base se crea y se pobla (géneros, salas y butacas).

Si preferís aplicarlas a mano:

```bash
dotnet ef database update --project Cinemon.Infrastructure --startup-project Cinemon.Api
```

**4. Correr la API y el Web (dos terminales)**

```bash
# Terminal 1 �?API
cd Cinemon.Api
dotnet run

# Terminal 2 �?Web
cd Cinemon.Web
dotnet run
```

### URLs por defecto

| Proyecto | HTTP | HTTPS |
|---|---|---|
| **API** | `http://localhost:5137` | `https://localhost:7087` |
| **Web** | `http://localhost:5223` | `https://localhost:7033` |

Swagger de la API: `https://localhost:7087/swagger` (también es la `launchUrl` del perfil https).

> ⚠️ El Web apunta a la API en `https://localhost:7087/` de forma hardcodeada en `Program.cs`. Si el certificado de desarrollo no está confiado, aceptalo en el navegador la primera vez.

**5. Contraseñas**

No se siembra ningún usuario por defecto. El primer paso después de levantar es registrarte como cliente desde `/login` y, si querés probar el panel admin, tenés dos caminos: registrar un cliente y luego (desde otro usuario admin) promoverlo, o bien insertar un admin directamente en la base con el `Rol = 2`. *Nota: no existe endpoint público para crear un admin sin estar logueado como admin.*

## 🔒 Decisiones de seguridad implementadas

Sin entrar en detalles explotables, el sistema parte de una base sólida en materia de seguridad:

- **Autenticación JWT**: los tokens se firman con HMAC-SHA256 (clave fuera del repo, en user-secrets), con expiración de 60 minutos y validación de issuer/audiencia/lifetime.
- **Hashing de contraseñas**: se usa el `PasswordHasher` de ASP.NET Core Identity (salt + hash iterativo); en ningún momento se guarda la contraseña en claro.
- **Autorización por roles**: cada endpoint declara qué roles pueden ejecutarlo (`RequireRole("Admin")`, `RequireRole("Cliente")`, etc.) y el frontend además oculta/restringe el área admin. Solo un admin puede crear u activar otro usuario.
- **Protección contra acceso no autorizado a recursos de otros usuarios (IDOR)**: las consultas de reservas y datos personales validan que el recurso pertenezca al usuario autenticado; listados sensibles (`GET /api/reservas`) solo son accesibles por Admins.
- **Manejo centralizado de errores** que evita filtrar stack traces al cliente (todo pasa por `GlobalExceptionHandler` y se devuelve `ProblemDetails`).
- **Secretos nunca en el repo**: connection string, clave JWT y token de TMDB viajan por user-secrets y el `.gitignore` excluye los `appsettings.*.local.json`.

## 🗺�?Posibles mejoras a futuro

- �?�?**Tests automatizados** (xUnit) para los handlers y la regla de concurrencia de reservas (es la mejora #1 pendiente).
- **Paginación** en listados de películas, funciones y reservas (hoy se trae todo).
- **CI/CD**: el repo ya tiene `.github/workflows/` (vacío) listo para un pipeline de build + test.
- **Notificaciones por email** (confirmación y recordatorio de funciones).
- **Pagos reales** (Mercado Pago/Stripe) en lugar de reserva sin cobro.
- **Refresco de tokens** (`refresh token` + revocación).
- **Mejoras UX**: mapa de sala accesible y mobile-first, filtros por fecha en el cliente.
- **Ambientación**: soporte multi-idioma y moneda configurable.
- **Migraciones explícitas en producción** en lugar del auto-migrate del seed.


### Contacto

Hecho y mantenido con ❤️ por **Iván** como proyecto de portafolio.

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Perfil-0A66C2?style=flat-square&logo=linkedin)](https://linkedin.com/in/tu-usuario)

Si algo del proyecto te sirvió, te gustó o tenés feedback, no dudes en escribirme �?¡me encantaría saberlo! 😄