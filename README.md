# ApiFunkosConjunta — API de gestión de Funkos

## 1. Descripción del proyecto

**ApiFunkosConjunta** es una API desarrollada con **ASP.NET Core y .NET 10** para gestionar un catálogo de Funkos, sus categorías y sus proveedores.

El proyecto utiliza una arquitectura por capas que separa los controladores, los servicios de aplicación, los repositorios y la persistencia. Además, combina distintos sistemas de almacenamiento según el tipo de entidad:

- **PostgreSQL** para Funkos y categorías.
- **MongoDB** para proveedores.
- **Redis** como caché distribuida para las consultas del catálogo.

La API permite realizar operaciones CRUD independientes para Funkos, categorías y proveedores. Las consultas REST son públicas, mientras que las operaciones de creación, actualización y eliminación requieren autenticación mediante JWT y el rol `Administrador`.

Además de la API REST, el proyecto incorpora:

- **Swagger/OpenAPI** para documentar y probar los endpoints.
- **GraphQL**, mediante HotChocolate, para consultar el catálogo.
- **SignalR** para enviar notificaciones en tiempo real sobre las operaciones realizadas con Funkos.
- **Email**, mediante MailKit y un servicio de envío en segundo plano.
- Pruebas unitarias y de integración con NUnit, Moq, FluentAssertions y Testcontainers.
- Colecciones de peticiones de Bruno para comprobar manualmente los endpoints.

La infraestructura se ejecuta mediante Docker Compose, con configuraciones separadas para desarrollo y producción.

---

## 2. Objetivos

Los objetivos principales del proyecto son:

- Desarrollar una API con ASP.NET Core y .NET 10.
- Gestionar Funkos, categorías y proveedores mediante tres CRUD independientes.
- Utilizar PostgreSQL para almacenar Funkos y categorías.
- Utilizar MongoDB para almacenar proveedores.
- Generar identificadores de categoría de 11 caracteres siguiendo el formato utilizado por YouTube.
- Utilizar identificadores `ObjectId` de MongoDB para los proveedores.
- Mantener la relación entre Funkos, categorías y proveedores.
- Incorporar búsquedas por identificador, filtros por nombre y paginación.
- Almacenar la ruta relativa de la imagen de cada Funko.
- Incorporar caché distribuida mediante Redis e invalidación de caché al modificar los datos.
- Proteger las operaciones de escritura mediante autenticación JWT y autorización por roles.
- Aplicar inyección de dependencias y separación de responsabilidades.
- Centralizar la coordinación de las operaciones del catálogo.
- Gestionar errores mediante resultados tipados y errores de dominio.
- Incorporar documentación interactiva mediante Swagger/OpenAPI.
- Permitir consultas al catálogo mediante GraphQL.
- Enviar notificaciones en tiempo real mediante SignalR.
- Incorporar un sistema de envío de correos electrónicos.
- Disponer de pruebas unitarias y pruebas de integración de repositorios con Testcontainers.
- Comprobar los endpoints mediante colecciones de Bruno.
- Utilizar Docker Compose para levantar la API y los servicios de infraestructura.

---

## 3. Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| .NET 10 / ASP.NET Core | Plataforma y framework de la API |
| C# | Lenguaje de programación |
| Entity Framework Core 10 | Acceso a datos relacionales |
| PostgreSQL 18 | Persistencia de Funkos y categorías |
| Npgsql | Proveedor de Entity Framework Core para PostgreSQL |
| MongoDB 8 | Persistencia de proveedores |
| MongoDB.Driver | Cliente de acceso a MongoDB |
| Redis 8 | Caché distribuida |
| `Microsoft.Extensions.Caching.StackExchangeRedis` | Integración de Redis con `IDistributedCache` |
| JWT Bearer | Autenticación mediante tokens JWT |
| `CSharpFunctionalExtensions` | Resultados y gestión de errores |
| `System.Text.Json` | Serialización de datos almacenados en caché |
| Swashbuckle | Documentación interactiva de la API (Swagger/OpenAPI) |
| HotChocolate 16 | Implementación de GraphQL |
| SignalR | Comunicación y notificaciones en tiempo real |
| MailKit | Envío de correos electrónicos mediante SMTP |
| MailDev | Servidor SMTP y visualización de correos durante el desarrollo |
| NUnit 5 | Framework de pruebas |
| Moq | Creación de mocks |
| FluentAssertions | Aserciones de pruebas |
| Testcontainers | Contenedores de PostgreSQL y MongoDB para pruebas |
| Docker / Docker Compose | Ejecución de la infraestructura |
| Bruno | Pruebas manuales de los endpoints HTTP |

---

## 4. Arquitectura

La API sigue una arquitectura por capas. Los controladores reciben las peticiones HTTP y delegan las operaciones en `CatalogoService`, que coordina los servicios específicos. Estos servicios utilizan los repositorios para acceder a sus respectivos sistemas de persistencia.

```text
                         ┌──────────────────────┐
                         │     Controllers      │
                         │ Funkos / Categorías  │
                         │ Proveedores / Auth   │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │    CatalogoService   │
                         │ Coordinación general │
                         │ Validación cruzada   │
                         │ Caché Redis          │
                         └──────────┬───────────┘
                                    │
                 ┌─────────────────┼─────────────────┐
                 ▼                 ▼                 ▼
          ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
          │ Categoria    │ │ Funko        │ │ Proveedor    │
          │ Service      │ │ Service      │ │ Service      │
          └──────┬───────┘ └──────┬───────┘ └──────┬───────┘
                 │                │                │
                 ▼                ▼                ▼
          ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
          │ Categoria    │ │ Funko        │ │ Proveedor    │
          │ Repository   │ │ Repository   │ │ Repository   │
          └──────┬───────┘ └──────┬───────┘ └──────┬───────┘
                 │                │                │
                 ▼                ▼                ▼
             PostgreSQL       PostgreSQL         MongoDB


        ┌─────────────────────────────────────────────┐
        │              Funcionalidades                │
        │                                             │
        │  GraphQL       SignalR        Email         │
        │  HotChocolate  FunkoHub       MailKit       │
        │                                             │
        │  Redis         JWT            Swagger       │
        └─────────────────────────────────────────────┘
```

La responsabilidad de cada capa es:

- **Controllers**: exponen los endpoints HTTP, reciben DTO y traducen los resultados a respuestas HTTP.
- **CatalogoService**: coordina las operaciones de las tres entidades, comprueba las relaciones entre ellas y gestiona la caché.
- **Services**: contienen la lógica específica de cada entidad, además de los servicios de autenticación y correo.
- **Repositories**: encapsulan el acceso a PostgreSQL o MongoDB.
- **Models**: representan las entidades del dominio.
- **Dto**: contienen los datos de entrada y salida de la API.
- **Mappers**: convierten entre modelos y DTO.
- **Infrastructure**: configura la inyección de dependencias, la persistencia, Swagger y CORS.
- **Data**: contiene el contexto de Entity Framework Core, las configuraciones y los seeders.
- **Errors**: agrupa los errores de dominio y de las distintas capas.
- **GraphQL**: contiene las consultas y los tipos de respuesta de GraphQL.
- **Hubs**: contiene los hubs de SignalR.
- **Common**: agrupa elementos compartidos, como paginación, serialización, ejemplos y respuestas HTTP.

Los controladores utilizan `ICatalogoService`; los servicios individuales quedan como dependencias internas de la coordinación del catálogo.

---

## 5. Estructura del proyecto

La solución está organizada en dos proyectos principales: la API, que contiene la lógica de negocio y los diferentes componentes de la aplicación, y el proyecto de pruebas, que contiene los tests automatizados.

```text
ApiFunkosConjunta/
├── .dockerignore
├── .env.example
├── .env.prod.example
├── .gitignore
├── .run/
│   └── compose.prod.yaml_ Compose Deployment.run.xml
├── ApiFunkosConjunta/
│   ├── Common/
│   │   ├── Configuration/
│   │   │   └── SmtpSettings.cs
│   │   ├── Examples/
│   │   │   ├── ActualizarFunkoDtoExample.cs
│   │   │   ├── CrearCategoriaDtoExample.cs
│   │   │   ├── CrearFunkoDtoExample.cs
│   │   │   ├── CrearProveedorDtoExample.cs
│   │   │   └── LoginDtoExample.cs
│   │   ├── Http/
│   │   │   └── ApiProblem.cs
│   │   ├── Json/
│   │   │   └── ObjectIdJsonConverter.cs
│   │   └── Pagination/
│   │       └── PagedResult.cs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CategoriasController.cs
│   │   ├── FunkosController.cs
│   │   └── ProveedoresController.cs
│   ├── Data/
│   │   ├── Configurations/
│   │   │   ├── CategoriaConfiguration.cs
│   │   │   ├── FunkoConfiguration.cs
│   │   │   └── UsuarioConfiguration.cs
│   │   ├── Seed/
│   │   │   ├── CategoriaSeeder.cs
│   │   │   ├── FunkoSeeder.cs
│   │   │   ├── ProveedorSeeder.cs
│   │   │   └── UsuarioSeeder.cs
│   │   └── AppDbContext.cs
│   ├── Dto/
│   │   ├── Auth/
│   │   │   ├── LoginDto.cs
│   │   │   └── LoginResponseDto.cs
│   │   ├── Categorias/
│   │   │   ├── ActualizarCategoriaDto.cs
│   │   │   ├── CacheCategoriaDto.cs
│   │   │   ├── CategoriaDto.cs
│   │   │   └── CrearCategoriaDto.cs
│   │   ├── Funkos/
│   │   │   ├── ActualizarFunkoDto.cs
│   │   │   ├── CacheFunkoDto.cs
│   │   │   ├── CrearFunkoDto.cs
│   │   │   └── FunkoDto.cs
│   │   └── Proveedores/
│   │       ├── ActualizarProveedorDto.cs
│   │       ├── CacheProveedorDto.cs
│   │       ├── CrearProveedorDto.cs
│   │       └── ProveedorDto.cs
│   ├── Errors/
│   │   ├── ApiErrors.cs
│   │   ├── AuthErrors.cs
│   │   ├── CatalogoServiceErrors.cs
│   │   ├── DomainErrors.cs
│   │   ├── ProveedorErrors.cs
│   │   ├── RepositoryErrors.cs
│   │   └── ServiceErrors.cs
│   ├── GraphQL/
│   │   ├── Queries/
│   │   │   └── Query.cs
│   │   └── Types/
│   │       ├── CategoriasPaginadas.cs
│   │       └── FunkosPaginados.cs
│   ├── Hubs/
│   │   └── FunkoHub.cs
│   ├── IdGenerators/
│   │   ├── YouTubeIdAttribute.cs
│   │   └── YouTubeIdValueGenerator.cs
│   ├── Infrastructure/
│   │   ├── Filters/
│   │   │   └── SwaggerExamplesFilter.cs
│   │   ├── Persistence/
│   │   │   └── PersistenceExtensions.cs
│   │   ├── CorsConfig.cs
│   │   ├── DependencyInjection.cs
│   │   ├── EmailConfig.cs
│   │   └── SwaggerConfig.cs
│   ├── Mappers/
│   │   ├── CategoriaMapper.cs
│   │   ├── FunkoMapper.cs
│   │   └── ProveedorMapper.cs
│   ├── Models/
│   │   ├── Categoria.cs
│   │   ├── Funko.cs
│   │   ├── Proveedor.cs
│   │   └── Usuario.cs
│   ├── Repositories/
│   │   ├── Implementations/
│   │   │   ├── CategoriaRepository.cs
│   │   │   ├── FunkoRepository.cs
│   │   │   └── ProveedorRepository.cs
│   │   └── Interfaces/
│   │       ├── ICategoriaRepository.cs
│   │       ├── IFunkoRepository.cs
│   │       └── IProveedorRepository.cs
│   ├── Services/
│   │   ├── Email/
│   │   │   ├── EmailBackgroundService.cs
│   │   │   ├── EmailTemplates.cs
│   │   │   ├── IEmailService.cs
│   │   │   ├── MailKitEmailService.cs
│   │   │   └── MemoryEmailService.cs
│   │   ├── Implementations/
│   │   │   ├── AuthService.cs
│   │   │   ├── CategoriaService.cs
│   │   │   ├── FunkoService.cs
│   │   │   └── ProveedorService.cs
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── ICategoriaService.cs
│   │   │   ├── IFunkoService.cs
│   │   │   └── IProveedorService.cs
│   │   ├── CatalogoService.cs
│   │   └── ICatalogoService.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── ApiFunkosConjunta.csproj
│   ├── ApiFunkosConjunta.http
│   ├── Dockerfile
│   ├── Program.cs
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   └── appsettings.json
├── ApiFunkosConjunta.Tests/
│   ├── Common/
│   │   ├── Configuration/
│   │   │   └── SmtpSettingsTest.cs
│   │   ├── Examples/
│   │   │   └── ExamplesTest.cs
│   │   ├── Http/
│   │   │   └── ApiProblemTest.cs
│   │   ├── Json/
│   │   │   └── ObjectIdJsonConverterTest.cs
│   │   └── Pagination/
│   │       └── PagedResultTest.cs
│   ├── Controllers/
│   │   ├── Auth/
│   │   │   └── AuthControllerTest.cs
│   │   ├── Categorias/
│   │   │   ├── CategoriaControllerCasosCorrectosTest.cs
│   │   │   ├── CategoriaControllerCasosIncorrectosTest.cs
│   │   │   └── CategoriaControllerTestBase.cs
│   │   ├── Funkos/
│   │   │   ├── FunkoControllerCasosCorrectosTest.cs
│   │   │   ├── FunkoControllerCasosIncorrectosTest.cs
│   │   │   ├── FunkoControllerNotificacionesTest.cs
│   │   │   └── FunkoControllerTestBase.cs
│   │   └── Proveedores/
│   │       ├── ProveedorControllerCasosCorrectosTest.cs
│   │       ├── ProveedorControllerCasosIncorrectosTest.cs
│   │       └── ProveedorControllerTestBase.cs
│   ├── Data/
│   │   └── Seed/
│   │       ├── CategoriaSeederTest.cs
│   │       ├── FunkoSeederTest.cs
│   │       ├── PostgresSeederTestBase.cs
│   │       ├── ProveedorSeederTest.cs
│   │       └── UsuarioSeederTest.cs
│   ├── Errors/
│   │   └── DomainErrorsTest.cs
│   ├── GraphQL/
│   │   └── Queries/
│   │       └── QueryTest.cs
│   ├── Hubs/
│   │   └── FunkoHubTest.cs
│   ├── Infrastructure/
│   │   ├── Filters/
│   │   │   └── SwaggerExamplesFilterTest.cs
│   │   ├── CorsConfigTest.cs
│   │   ├── DependencyInjectionTest.cs
│   │   ├── EmailConfigTest.cs
│   │   ├── PersistenceExtensionsTest.cs
│   │   └── SwaggerConfigTest.cs
│   ├── Mappers/
│   │   ├── Categorias/
│   │   │   └── CategoriaMapperCasosCorrectosTest.cs
│   │   └── Proveedores/
│   │       └── ProveedorMapperCasosCorrectosTest.cs
│   ├── Repositories/
│   │   ├── Categorias/
│   │   │   ├── CategoriaRepositoryCasosCorrectosTest.cs
│   │   │   ├── CategoriaRepositoryCasosIncorrectosTest.cs
│   │   │   └── CategoriaRepositoryTestBase.cs
│   │   ├── Funkos/
│   │   │   ├── FunkoRepositoryCasosCorrectosTest.cs
│   │   │   ├── FunkoRepositoryCasosIncorrectosTest.cs
│   │   │   └── FunkoRepositoryTestBase.cs
│   │   └── Proveedores/
│   │       ├── ProveedorRepositoryCasosCorrectosTest.cs
│   │       ├── ProveedorRepositoryCasosIncorrectosTest.cs
│   │       └── ProveedorRepositoryTestBase.cs
│   ├── Services/
│   │   ├── Auth/
│   │   │   ├── AuthServiceCasosCorrectosTest.cs
│   │   │   ├── AuthServiceCasosIncorrectosTest.cs
│   │   │   └── AuthServiceTestBase.cs
│   │   ├── Email/
│   │   │   ├── EmailBackgroundServiceTest.cs
│   │   │   ├── MailKitEmailServiceTest.cs
│   │   │   └── MemoryEmailServiceTest.cs
│   │   ├── Funkos/
│   │   │   ├── FunkoServiceCasosCorrectosTest.cs
│   │   │   ├── FunkoServiceCasosIncorrectosTest.cs
│   │   │   ├── FunkoServiceNotificacionesTest.cs
│   │   │   └── FunkoServiceTestBase.cs
│   │   ├── Categorias/
│   │   │   ├── CategoriaServiceCasosCorrectosTest.cs
│   │   │   ├── CategoriaServiceCasosIncorrectosTest.cs
│   │   │   └── CategoriaServiceTestBase.cs
│   │   ├── Proveedores/
│   │   │   ├── ProveedorServiceCasosCorrectosTest.cs
│   │   │   ├── ProveedorServiceCasosIncorrectosTest.cs
│   │   │   └── ProveedorServiceTestBase.cs
│   │   ├── CatalogoServiceCacheTest.cs
│   │   ├── CatalogoServiceCasosCorrectosTest.cs
│   │   ├── CatalogoServiceCasosIncorrectosTest.cs
│   │   └── CatalogoServiceTestBase.cs
│   └── ApiFunkosConjunta.Tests.csproj
├── ApiFunkosConjunta.slnx
├── compose.local.yaml
├── compose.prod.yaml
├── compose.yaml
└── coverage.runsettings
```

### 5.1. Descripción de las carpetas principales

- **Common:** componentes compartidos, configuración SMTP, ejemplos para Swagger, gestión de errores HTTP, conversión JSON y paginación.
- **Controllers:** controladores REST para autenticación, categorías, Funkos y proveedores.
- **Data:** contexto de PostgreSQL, configuraciones de entidades y seeders de datos iniciales.
- **Dto:** objetos de transferencia de datos para autenticación, categorías, Funkos y proveedores.
- **Errors:** definición centralizada de errores de dominio, servicios, repositorios y API.
- **GraphQL:** consultas GraphQL y tipos de respuesta paginada.
- **Hubs:** implementación de SignalR para las notificaciones relacionadas con Funkos.
- **IdGenerators:** generación de identificadores personalizados para categorías.
- **Infrastructure:** configuración de inyección de dependencias, persistencia, CORS, Email y Swagger.
- **Mappers:** conversión entre modelos y DTO.
- **Models:** entidades principales del dominio.
- **Repositories:** interfaces e implementaciones para el acceso a PostgreSQL y MongoDB.
- **Services:** lógica de negocio, autenticación, catálogo, gestión de entidades y envío de correos electrónicos.
- **ApiFunkosConjunta.Tests:** pruebas automatizadas organizadas por componente, incluyendo componentes comunes, controladores, servicios, repositorios, mappers, seeders, infraestructura, GraphQL y SignalR.

---

## 6. Modelo de datos y persistencia

El proyecto distribuye las entidades entre PostgreSQL y MongoDB.

### 6.1. Funko — PostgreSQL

La entidad `Funko` contiene:

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Identificador autoincremental |
| `Nombre` | `string` | Nombre del Funko |
| `CategoriaId` | `string` | Identificador de la categoría |
| `ProveedorId` | `string` | ObjectId del proveedor almacenado como texto |
| `Stock` | `int` | Unidades disponibles |
| `Precio` | `decimal` | Precio del Funko |
| `Imagen` | `string` | Ruta relativa de la imagen |

La tabla se denomina `funkos`. El precio utiliza precisión `(10,2)` y la ruta de imagen se almacena como texto, no como archivo binario ni como URL absoluta.

Ejemplo de ruta:

```text
imagenes/funkos/spider-man.png
```

### 6.2. Categoría — PostgreSQL

La entidad `Categoria` contiene:

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `string` | Identificador generado de 11 caracteres |
| `Nombre` | `string` | Nombre de la categoría |

El identificador se genera mediante `YouTubeIdValueGenerator`, utilizando un alfabeto de 64 caracteres y una longitud de 11 posiciones.

### 6.3. Proveedor — MongoDB

La entidad `Proveedor` contiene:

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `ObjectId` | Identificador de MongoDB |
| `Nombre` | `string` | Nombre del proveedor |
| `Contacto` | `string` | Información de contacto |
| `Pais` | `string` | País del proveedor |

La colección de MongoDB se denomina `proveedores`.

### 6.4. Relaciones

La relación lógica del catálogo es:

```text
Proveedor (1) ─────── (*) Funko (*) ─────── (1) Categoría
```

Cada Funko pertenece a una categoría y a un proveedor. Una categoría o un proveedor pueden estar asociados a varios Funkos.

La relación con `Categoria` se configura mediante una clave externa real en PostgreSQL, con borrado restringido (`Restrict`).

La relación con `Proveedor` atraviesa dos bases de datos: `ProveedorId` se guarda en PostgreSQL como una cadena de 24 caracteres y `CatalogoService` comprueba que el proveedor exista en MongoDB antes de crear o actualizar un Funko. No existe una clave externa SQL hacia MongoDB.

Además, `CatalogoService` impide eliminar una categoría o un proveedor cuando todavía existen Funkos asociados.

---

## 7. API REST y endpoints

La API utiliza la ruta base `/api`.

### 7.1. Funkos

| Método | Endpoint | Acceso | Operación |
|---|---|---|---|
| GET | `/api/funkos` | Público | Listar Funkos con filtro y paginación |
| GET | `/api/funkos/{id}` | Público | Obtener un Funko por ID |
| POST | `/api/funkos` | Administrador | Crear un Funko |
| PUT | `/api/funkos/{id}` | Administrador | Actualizar un Funko |
| DELETE | `/api/funkos/{id}` | Administrador | Eliminar un Funko |

### 7.2. Categorías

| Método | Endpoint | Acceso | Operación |
|---|---|---|---|
| GET | `/api/categorias` | Público | Listar categorías con filtro y paginación |
| GET | `/api/categorias/{id}` | Público | Obtener una categoría por ID |
| POST | `/api/categorias` | Administrador | Crear una categoría |
| PUT | `/api/categorias/{id}` | Administrador | Actualizar una categoría |
| DELETE | `/api/categorias/{id}` | Administrador | Eliminar una categoría |

### 7.3. Proveedores

| Método | Endpoint | Acceso | Operación |
|---|---|---|---|
| GET | `/api/proveedores` | Público | Listar proveedores con filtro y paginación |
| GET | `/api/proveedores/{id}` | Público | Obtener un proveedor por ObjectId |
| POST | `/api/proveedores` | Administrador | Crear un proveedor |
| PUT | `/api/proveedores/{id}` | Administrador | Actualizar un proveedor |
| DELETE | `/api/proveedores/{id}` | Administrador | Eliminar un proveedor |

### 7.4. Autenticación

| Método | Endpoint | Acceso | Operación |
|---|---|---|---|
| POST | `/api/auth/login` | Público | Validar las credenciales y obtener un JWT |

Los tres endpoints de listado aceptan los siguientes parámetros de consulta:

| Parámetro | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `nombre` | `string` | Sin filtro | Filtra por nombre |
| `pagina` | `int` | `1` | Número de página |
| `tamanoPagina` | `int` | `10` | Elementos por página; máximo 100 |

Las respuestas de listado utilizan `PagedResult<T>`, que incluye los datos, el total de registros, la página y el tamaño de página.

Ejemplo:

```http
GET /api/funkos?nombre=batman&pagina=1&tamanoPagina=10
```

### 7.5. Documentación interactiva (Swagger/OpenAPI)

La API publica su documentación mediante **Swashbuckle**. En desarrollo está disponible la interfaz web de Swagger UI:

```text
http://localhost:5087/swagger
```

Y el documento OpenAPI en formato JSON:

```text
http://localhost:5087/swagger/v1/swagger.json
```

La configuración vive en `Infrastructure/SwaggerConfig.cs` e incluye:

| Elemento | Para qué sirve |
|---|---|
| `SwaggerDoc("v1", ...)` | Título, versión, descripción y contacto de la API |
| `AddSecurityDefinition("Bearer", ...)` | Esquema de seguridad JWT y botón **Authorize** |
| `IncludeXmlComments(...)` | Comentarios XML de los controladores y DTO |
| `EnableAnnotations()` | Atributos de documentación de Swagger |
| `ExampleFilters()` | Ejemplos de petición mediante `IExamplesProvider<T>` |
| `OperationFilter<SwaggerExamplesFilter>()` | Filtro propio que añade ejemplos `ProblemDetails` a las respuestas de error |

Para probar los endpoints protegidos desde Swagger UI:

1. Abre `http://localhost:5087/swagger`.
2. Pulsa **Authorize**, pega el token (sin la palabra `Bearer`) y confirma.
3. Ejecuta `POST /api/auth/login` para obtener el token si aún no lo tienes.

Las respuestas de error se documentan mediante `ProblemDetails` (RFC 9457), de modo que la documentación y la respuesta real utilizan el mismo formato.

### 7.6. CORS

La política de CORS se registra en `Infrastructure/CorsConfig.cs`:

| Entorno | Política | Comportamiento |
|---|---|---|
| Desarrollo | `AllowAll` | Permite cualquier origen, método y cabecera |
| Producción | `ProductionPolicy` | Solo los orígenes de la sección `Cors:AllowedOrigins` |

En producción los orígenes se configuran mediante `appsettings.Production.json` o variables de entorno como `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1`, etc.

---

## 8. GraphQL

La API incorpora GraphQL mediante **HotChocolate 16**.

El endpoint GraphQL está disponible tanto en Development como en Production:

```text
http://localhost:5087/graphql
http://localhost:5000/graphql
```

La interfaz gráfica **Nitro** está habilitada únicamente en Development. En Production se mantiene operativo el endpoint GraphQL, pero no se expone la interfaz gráfica.

### 8.1. Consultas disponibles

La implementación se limita a consultas (*Queries*), sin Mutations ni Subscriptions.

| Consulta | Descripción |
|---|---|
| `estado` | Comprueba que GraphQL está funcionando |
| `funkos` | Obtiene Funkos con filtro y paginación |
| `funko(id)` | Obtiene un Funko por identificador |
| `categorias` | Obtiene categorías con filtro y paginación |
| `categoria(id)` | Obtiene una categoría por identificador |

Las consultas de listado admiten los argumentos:

- `nombre`: filtro opcional por nombre.
- `pagina`: número de página.
- `tamanoPagina`: número de elementos por página.

Las respuestas paginadas contienen `items`, `total`, `pagina` y `tamanoPagina`.

### 8.2. Ejemplos de consultas

Comprobar el estado:

```graphql
query {
  estado
}
```

Obtener Funkos:

```graphql
query {
  funkos(pagina: 1, tamanoPagina: 10) {
    items {
      id
      nombre
      precio
      stock
    }
    total
    pagina
    tamanoPagina
  }
}
```

Filtrar Funkos por nombre:

```graphql
query {
  funkos(nombre: "Batman", pagina: 1, tamanoPagina: 10) {
    items {
      id
      nombre
    }
    total
  }
}
```

Obtener un Funko por ID:

```graphql
query {
  funko(id: 1) {
    id
    nombre
    categoriaId
    proveedorId
    precio
    stock
    imagen
  }
}
```

Obtener categorías:

```graphql
query {
  categorias(pagina: 1, tamanoPagina: 10) {
    items {
      id
      nombre
    }
    total
  }
}
```

Obtener una categoría por ID:

```graphql
query {
  categoria(id: "ID_CATEGORIA") {
    id
    nombre
  }
}
```

GraphQL utiliza los servicios de aplicación existentes (`IFunkoService` e `ICategoriaService`), evitando duplicar la lógica de negocio y el acceso a datos.

---

## 9. SignalR

El proyecto incorpora **SignalR** para enviar notificaciones en tiempo real cuando se realizan operaciones sobre Funkos.

El hub está disponible en:

```text
/hubs/funkos
```

La clase `FunkoHub` requiere autenticación mediante JWT.

### 9.1. Grupos de usuarios

Al establecer una conexión, el hub incorpora al usuario a un grupo individual:

```text
usuario-{usuarioId}
```

Si el usuario tiene el rol `Administrador`, también se incorpora al grupo:

```text
administradores
```

De esta forma, las notificaciones pueden dirigirse tanto a los administradores como al usuario propietario de la operación.

### 9.2. Eventos

| Evento | Descripción |
|---|---|
| `FunkoCreado` | Notifica la creación de un Funko |
| `FunkoActualizado` | Notifica la actualización de un Funko |
| `FunkoEliminado` | Notifica la eliminación de un Funko |

Las notificaciones se envían a los grupos correspondientes según la operación realizada.

Para conectar clientes mediante SignalR, la autenticación utiliza el JWT. La configuración de `JwtBearer` permite recibir el token mediante el parámetro `access_token` cuando la conexión se realiza al hub de Funkos.

Los errores durante el envío de notificaciones se registran, evitando que un problema de comunicación impida completar una operación de persistencia del catálogo.

---

## 10. Servicio de correo electrónico

El proyecto incorpora un sistema de envío de correos electrónicos mediante **MailKit**.

La implementación separa la lógica de envío de la gestión de los mensajes:

- `MailKitEmailService`: realiza el envío de correos mediante SMTP.
- `MemoryEmailService`: proporciona una implementación en memoria.
- `EmailBackgroundService`: procesa mensajes en segundo plano.
- `EmailMessage`: representa los mensajes que se deben enviar.
- `EmailTemplates`: centraliza las plantillas de correo.
- `EmailConfig`: contiene la configuración necesaria para el servicio SMTP.

La configuración del servidor de correo se realiza mediante variables de entorno o los archivos de configuración correspondientes. No es necesario introducir credenciales SMTP directamente en el código.

### 10.1. MailDev

Para las pruebas y el desarrollo se utiliza **MailDev**, que proporciona un servidor SMTP local y una interfaz web para consultar los correos enviados.

En Development:

```text
Interfaz web: http://localhost:1080
SMTP:         localhost:1025
```

En Production, dentro de la configuración local de demostración:

```text
Interfaz web: http://localhost:1081
```

MailDev permite comprobar el contenido de los mensajes sin utilizar un servidor de correo externo.

---

## 11. Autenticación y autorización

La API utiliza un sistema de autenticación basado en **JWT Bearer**.

El endpoint `POST /api/auth/login` comprueba las credenciales del administrador configuradas en la sección `Jwt` de la aplicación. Si son correctas, `AuthService` genera un token firmado mediante el algoritmo HMAC-SHA256 que incluye:

- El nombre del usuario.
- El rol `Administrador`.
- El emisor (`Issuer`) y la audiencia (`Audience`) configurados.
- Una fecha de expiración de dos horas.

Los controladores aplican el atributo `[Authorize(Roles = "Administrador")]` únicamente a las operaciones de creación, actualización y eliminación (`POST`, `PUT` y `DELETE`). Las operaciones de consulta (`GET`) permanecen públicas.

Para acceder a las operaciones protegidas, el cliente debe enviar el token en la cabecera HTTP:

```http
Authorization: Bearer JWT_GENERADO
```

### 11.1. Credenciales de desarrollo

Para facilitar las pruebas, el entorno de desarrollo dispone de unas credenciales de demostración:

```text
Usuario: admin
Contraseña: Admin123!
```

### 11.2. Configuración de seguridad

La configuración de autenticación se adapta al entorno de ejecución:

- **Development:** utiliza los valores de configuración de desarrollo.
- **Production:** recibe los parámetros JWT mediante variables de entorno definidas en `compose.prod.yaml`, como `JWT_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_ADMIN_USERNAME` y `JWT_ADMIN_PASSWORD`.

Las credenciales de demostración y las claves de desarrollo no deben reutilizarse en producción. Para un despliegue real, deben establecerse valores propios y seguros, evitando almacenar secretos directamente en el repositorio.

---

## 12. Caché Redis

`CatalogoService` utiliza la abstracción `IDistributedCache`, registrada mediante `AddStackExchangeRedisCache`.

Se almacenan en caché:

- Los listados paginados de Funkos, categorías y proveedores.
- Las consultas individuales por identificador.

Las entradas de datos tienen una expiración absoluta de **5 minutos**.

Para los listados se utiliza una clave de versión por entidad. Al crear, actualizar o eliminar un registro, el servicio cambia la versión correspondiente, de modo que las siguientes consultas de listado utilizan una nueva clave y no recuperan resultados anteriores.

Cuando se modifica o elimina un registro concreto, también se elimina su entrada individual de caché.

Ejemplos de familias de claves:

```text
funkos:version
funkos:{version}:{filtro}:{pagina}:{tamanoPagina}
funko:{id}

categorias:version
categorias:{version}:{filtro}:{pagina}:{tamanoPagina}
categoria:{id}

proveedores:version
proveedores:{version}:{filtro}:{pagina}:{tamanoPagina}
proveedor:{id}
```

Así se evita tener que modificar directamente todas las claves de los listados cuando cambia el catálogo.

---

## 13. Configuración y ejecución

### 13.1. Requisitos previos

- .NET 10 SDK.
- Docker Desktop o Docker Engine con Docker Compose.
- Rider, Visual Studio o un editor compatible con .NET.
- Bruno para ejecutar las colecciones de peticiones incluidas.

No es necesario instalar PostgreSQL, MongoDB ni Redis directamente en el equipo: Docker Compose proporciona los servicios de infraestructura.

### 13.2. Entorno de desarrollo

El entorno de desarrollo separa la infraestructura de la ejecución de la API. De esta forma, es posible levantar las bases de datos, Redis y MailDev mediante Docker y ejecutar la API directamente desde Rider.

Desde la carpeta raíz:

```bash
docker compose -f compose.local.yaml up -d
```

Esto levanta los siguientes servicios:

| Servicio | Imagen | Puerto del host |
|---|---|---|
| PostgreSQL | `postgres:18` | `5432` |
| MongoDB | `mongo:8` | `27017` |
| Redis | `redis:8` | `6379` |
| MailDev (web) | MailDev | `1080` |
| MailDev (SMTP) | MailDev | `1025` |

A continuación, se puede ejecutar el proyecto `ApiFunkosConjunta` desde Rider utilizando el perfil `http` o `https`.

Según `launchSettings.json`, las direcciones locales son:

```text
HTTP  → http://localhost:5087
HTTPS → https://localhost:7274
```

En este modo, la configuración de desarrollo utiliza `localhost` para conectarse a los puertos publicados por Docker.

Para detener la infraestructura:

```bash
docker compose -f compose.local.yaml down
```

### 13.3. Entorno de producción

El entorno de producción levanta la API y sus servicios de infraestructura dentro de un proyecto Docker Compose independiente.

Primero, se debe preparar el archivo `.env.prod` a partir de `.env.prod.example`, completando las variables de configuración necesarias.

Después, desde la raíz:

```bash
docker compose --env-file .env.prod -f compose.prod.yaml up -d --build
```

Los principales servicios son:

| Servicio | Imagen | Puerto del host |
|---|---|---|
| API | Construida desde el Dockerfile | `5000` |
| PostgreSQL | `postgres:18` | No publicado |
| MongoDB | `mongo:8` | No publicado |
| Redis | `redis:8` | No publicado |
| MailDev (web) | MailDev | `1081` |

La API queda disponible en:

```text
http://localhost:5000
```

MailDev queda disponible en:

```text
http://localhost:1081
```

Dentro de la red de Docker, la API se conecta a los servicios mediante sus nombres Compose:

```text
PostgreSQL → postgres:5432
MongoDB    → mongodb:27017
Redis      → redis:6379
```

Para detener el entorno:

```bash
docker compose --env-file .env.prod -f compose.prod.yaml down
```

### 13.4. Ejecución simultánea de ambos entornos

Los entornos Development y Production utilizan proyectos Compose, redes y volúmenes independientes. Por ello, pueden mantenerse levantados simultáneamente sin compartir sus datos.

Los puertos de MailDev también están separados:

- Development: `http://localhost:1080`.
- Production: `http://localhost:1081`.

### 13.5. Persistencia de Docker

Los archivos Compose definen volúmenes persistentes para PostgreSQL, MongoDB y Redis.

Los datos sobreviven al reinicio de los contenedores. Para detener los servicios sin eliminar los datos:

```bash
docker compose down
```

En cada entorno debe utilizarse el archivo Compose correspondiente.

No utilizar `docker compose down -v` salvo que se quiera eliminar expresamente también el contenido de los volúmenes.

### 13.6. Variables de entorno

El proyecto incluye archivos de ejemplo:

```text
.env.example
.env.prod.example
```

Estos archivos sirven como plantilla para configurar las conexiones y los servicios.

Los archivos reales `.env` y `.env.prod` contienen la configuración del entorno y no deben subirse al repositorio si incluyen contraseñas, claves JWT u otros secretos.

---

## 14. Inicialización de datos

Durante el arranque, la aplicación ejecuta los seeders para preparar los datos iniciales:

- `CategoriaSeeder`: crea las categorías iniciales si todavía no existen.
- `UsuarioSeeder`: crea el usuario administrador si todavía no existe.
- `ProveedorSeeder`: inserta o actualiza los proveedores iniciales identificados por sus ObjectId.
- `FunkoSeeder`: crea los Funkos iniciales si todavía no existen.

Las categorías iniciales son:

- Videojuegos
- Películas
- Series
- Anime
- Cómics
- Música
- Deportes
- Tecnología

Los proveedores iniciales son Grupo Cladellas, Extended Play y Kilumio.

Los Funkos de ejemplo incluyen Spider-Man, Batman, Naruto Uzumaki, Darth Vader, Geralt de Rivia y Eleven.

### 14.1. Comportamiento por entorno

En **Development**, al iniciar la API se reinicializan las bases de datos y Redis. Se eliminan los datos anteriores y se ejecutan de nuevo los seeders.

Esto permite comenzar las pruebas con un estado conocido cada vez que se arranca la aplicación desde Rider.

En **Production**, los datos existentes se conservan y los seeders se ejecutan sin eliminar previamente la información almacenada.

La inicialización de PostgreSQL utiliza `EnsureCreatedAsync()` para crear la base de datos y su esquema cuando corresponde. No se utiliza un flujo de migraciones de Entity Framework Core en esta versión del proyecto.

---

## 15. Validación y gestión de errores

Los DTO de entrada utilizan atributos de `System.ComponentModel.DataAnnotations`, como:

- `[Required]`
- `[StringLength]`
- `[Range]`
- `[RegularExpression]`

Se validan, entre otros aspectos, los nombres, los identificadores de categoría y proveedor, el stock, el precio y la ruta de imagen.

La lógica de servicios devuelve resultados mediante `CSharpFunctionalExtensions`:

```csharp
Result<T, DomainErrors>
```

Los errores se organizan en la carpeta `Errors`, con tipos específicos para las distintas capas y operaciones.

Entre los casos contemplados se encuentran:

- Registro no encontrado.
- Nombre duplicado.
- Identificador de proveedor no válido.
- Categoría con Funkos asociados.
- Proveedor con Funkos asociados.
- Errores de acceso a repositorios.

Los controladores traducen los resultados a códigos HTTP, por ejemplo `400 Bad Request`, `401 Unauthorized`, `404 Not Found`, `409 Conflict` y `500 Internal Server Error`, según el caso.

Las respuestas de error se construyen con `ProblemDetails` (RFC 9457) mediante las fábricas de `Common/Http/ApiProblem.cs`.

Ejemplo:

```json
{
  "type": "https://tools.ietf.org/html/rfc9457#section-10.5",
  "title": "No se ha encontrado el recurso solicitado.",
  "status": 404,
  "detail": "No se ha encontrado la categoría con el id: ID_CATEGORIA.",
  "instance": "/api/categorias/ID_CATEGORIA"
}
```

Los errores de validación del modelo devuelven un `ValidationProblemDetails`, que añade el diccionario `errors` con el detalle de cada campo.

---

## 16. Pruebas automatizadas

El proyecto de pruebas es `ApiFunkosConjunta.Tests` y utiliza NUnit, Moq y FluentAssertions.

La estructura cubre las siguientes áreas:

- **Controllers**: comprobación de las respuestas HTTP y del comportamiento de los controladores.
- **Services**: casos correctos e incorrectos de los servicios y del servicio coordinador `CatalogoService`.
- **Repositories**: operaciones de acceso a PostgreSQL y MongoDB.
- **Mappers**: conversiones entre modelos y DTO.
- **Errors**: comprobaciones de los errores de dominio.
- **Seed**: pruebas de inicialización de datos.
- **Hubs**: pruebas del comportamiento de SignalR.
- **GraphQL**: pruebas de las consultas y de la gestión de errores.

### 16.1. Pruebas unitarias

Los mocks de Moq permiten aislar las dependencias y comprobar el comportamiento de cada capa. FluentAssertions se utiliza para expresar las aserciones de forma legible.

También se comprueba la interacción con `IDistributedCache`, incluyendo la lectura de datos y la invalidación de las entradas al modificar el catálogo.

Las pruebas cubren, entre otros componentes:

- Servicios de autenticación y catálogo.
- Controladores.
- Seeders.
- Mappers.
- Hub de SignalR.
- Servicio de correo electrónico.
- Consultas GraphQL.

### 16.2. Testcontainers

Las pruebas de repositorio utilizan contenedores reales:

- `Testcontainers.PostgreSql` con `postgres:18` para los repositorios de categorías y Funkos.
- `Testcontainers.MongoDb` con `mongo:8` para el repositorio de proveedores.

Cada conjunto de pruebas prepara sus datos y utiliza los contenedores para comprobar el acceso real a los respectivos motores de persistencia.

Docker debe estar iniciado para ejecutar estas pruebas.

### 16.3. Pruebas del servicio de correo

Las pruebas de envío SMTP necesitan que MailDev esté levantado y escuchando en el puerto configurado.

En Development, el servidor SMTP está disponible en:

```text
localhost:1025
```

Si MailDev no está iniciado, las pruebas que intentan realizar una conexión SMTP pueden fallar por conexión rechazada. Esto no implica necesariamente un error en la lógica del servicio de correo.

### 16.4. Ejecutar los tests

Desde la carpeta raíz de la solución:

```bash
dotnet test ApiFunkosConjunta.Tests/ApiFunkosConjunta.Tests.csproj
```

### 16.5. Medir la cobertura

Para ejecutar las pruebas y generar el informe de cobertura:

```bash
dotnet test --settings coverage.runsettings --collect:"XPlat Code Coverage"
```

La cobertura permite consultar las líneas y ramas ejecutadas por las pruebas automatizadas.

---

## 17. Inyección de dependencias

La configuración de servicios se centraliza en `Infrastructure/DependencyInjection.cs`, que registra:

- Los repositorios y servicios de categorías.
- Los repositorios y servicios de proveedores.
- Los repositorios y servicios de Funkos.
- `CatalogoService`.
- `AuthService`.
- Los servicios de correo electrónico.
- La autenticación JWT y la autorización por roles.
- SignalR.
- GraphQL mediante HotChocolate.
- La documentación Swagger/OpenAPI.
- La política de CORS.

`Infrastructure/Persistence/PersistenceExtensions.cs` registra:

- `AppDbContext` con PostgreSQL.
- `IDistributedCache` con Redis.
- `IMongoClient`, `IMongoDatabase` e `IMongoCollection<Proveedor>` con MongoDB.

Las dependencias se inyectan mediante los constructores, evitando que los controladores tengan que crear manualmente sus servicios o repositorios.

---

## 18. Decisiones de diseño

### Separación por capas

Los controladores no acceden directamente a los repositorios ni a los contextos de datos. Las operaciones se delegan en servicios, manteniendo separadas las responsabilidades.

### Servicio coordinador del catálogo

`CatalogoService` actúa como punto de coordinación para los controladores. Además de delegar las operaciones CRUD, aplica las reglas que afectan a varias entidades, como comprobar la existencia de categorías y proveedores antes de guardar un Funko.

### Persistencia según la entidad

PostgreSQL almacena las entidades relacionadas mediante claves relacionales cuando es posible. MongoDB almacena los proveedores con su identificador nativo `ObjectId`. La relación entre ambos motores se resuelve desde la lógica de aplicación.

### Caché distribuida

El uso de `IDistributedCache` desacopla el servicio de la implementación concreta de Redis y permite centralizar la lectura y la invalidación de caché.

### Resultados tipados

`CSharpFunctionalExtensions` permite devolver resultados correctos o errores de dominio de forma explícita, sin utilizar excepciones como mecanismo habitual de control del flujo.

### Seguridad por roles

JWT permite identificar al usuario y transportar su rol. Los atributos de autorización restringen únicamente las operaciones de escritura, manteniendo públicas las consultas REST.

### Comunicación en tiempo real

SignalR permite notificar a los clientes conectados sobre las operaciones realizadas con Funkos, separando la comunicación en tiempo real de la respuesta HTTP de la API.

### Consultas GraphQL

GraphQL proporciona una alternativa de consulta al catálogo, reutilizando los servicios existentes y evitando duplicar la lógica de negocio.

### Envío de correo desacoplado

El servicio de correo separa la generación y gestión de mensajes del envío SMTP, permitiendo procesar los envíos en segundo plano y utilizar MailDev durante el desarrollo.

### Entornos independientes

Docker Compose mantiene separadas las configuraciones de Development y Production, con redes y volúmenes propios para evitar que los datos de ambos entornos se mezclen.

---

## 19. Conclusión

**ApiFunkosConjunta** implementa una API para la gestión de Funkos, categorías y proveedores mediante una arquitectura por capas y dos sistemas de persistencia.

La solución combina:

```text
PostgreSQL  → Funkos y Categorías
MongoDB     → Proveedores
Redis       → Caché distribuida
JWT         → Autenticación y rol Administrador
Swagger     → Documentación y pruebas de la API
GraphQL     → Consultas del catálogo
SignalR     → Notificaciones en tiempo real
MailKit     → Envío de correos electrónicos
Docker      → Infraestructura reproducible
NUnit       → Pruebas automatizadas
Testcontainers → Pruebas de integración
Bruno       → Pruebas manuales de la API
```

La separación entre controladores, servicios y repositorios permite mantener la lógica de negocio organizada, mientras que `CatalogoService` centraliza las reglas que afectan a las relaciones entre entidades y la gestión de caché.

El proyecto incorpora autenticación y autorización, documentación interactiva, consultas GraphQL, comunicación en tiempo real y envío de correos electrónicos. Además, dispone de pruebas automatizadas y colecciones de Bruno para comprobar el comportamiento de los endpoints y las restricciones de acceso.

La separación de los entornos de desarrollo y producción mediante Docker Compose permite ejecutar y probar la solución de forma reproducible, manteniendo aisladas sus respectivas configuraciones y persistencias.