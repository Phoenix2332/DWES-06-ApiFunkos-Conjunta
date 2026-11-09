# ApiFunkosConjunta — API de gestión de Funkos

## 1. Descripción del proyecto

**ApiFunkosConjunta** es una API REST desarrollada con **ASP.NET Core y .NET 10** para gestionar un catálogo de Funkos, sus categorías y sus proveedores.

El proyecto utiliza una arquitectura por capas que separa los controladores, los servicios de aplicación, los repositorios y la persistencia. Además, combina dos sistemas de almacenamiento según el tipo de entidad:

- **PostgreSQL** para Funkos y categorías.
- **MongoDB** para proveedores.
- **Redis** como caché distribuida para las consultas del catálogo.

La API permite realizar operaciones CRUD independientes para Funkos, categorías y proveedores. Las consultas son públicas, mientras que las operaciones de creación, actualización y eliminación requieren autenticación mediante JWT y el rol `Administrador`.

El repositorio también incluye pruebas unitarias, pruebas de repositorio con Testcontainers y una colección de peticiones de Bruno para comprobar manualmente los endpoints.

---

## 2. Objetivos

Los objetivos principales del proyecto son:

- Desarrollar una API REST con ASP.NET Core y .NET 10.
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
- Disponer de pruebas unitarias y pruebas de integración de repositorios con Testcontainers.
- Comprobar los endpoints mediante una colección de Bruno.
- Utilizar Docker Compose para levantar la API y los servicios de infraestructura.

---

## 3. Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| .NET 10 / ASP.NET Core | Plataforma y framework de la API REST |
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
                         │      Controllers     │
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

                         ┌──────────────────────┐
                         │    IDistributedCache │
                         │        Redis         │
                         └──────────────────────┘
```

La responsabilidad de cada capa es:

- **Controllers**: exponen los endpoints HTTP, reciben DTO y traducen los resultados a respuestas HTTP.
- **CatalogoService**: coordina las operaciones de las tres entidades, comprueba las relaciones entre ellas y gestiona la caché.
- **Services**: contienen la lógica específica de cada entidad.
- **Repositories**: encapsulan el acceso a PostgreSQL o MongoDB.
- **Models**: representan las entidades del dominio.
- **Dto**: contienen los datos de entrada y salida de la API.
- **Mappers**: convierten entre modelos y DTO.
- **Infrastructure**: configura la inyección de dependencias y la persistencia.
- **Data**: contiene el contexto de Entity Framework Core, las configuraciones y los seeders.
- **Errors**: agrupa los errores de dominio y de las distintas capas.

Los controladores utilizan `ICatalogoService`; los servicios individuales quedan como dependencias internas de la coordinación del catálogo.

---

## 5. Estructura del proyecto

```text
ApiFunkosConjunta/
├── ApiFunkosConjunta/
│   ├── Common/
│   │   ├── Json/
│   │   └── Pagination/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CategoriasController.cs
│   │   ├── FunkosController.cs
│   │   └── ProveedoresController.cs
│   ├── Data/
│   │   ├── Configurations/
│   │   └── Seed/
│   ├── Dto/
│   │   ├── Auth/
│   │   ├── Categorias/
│   │   ├── Funkos/
│   │   └── Proveedores/
│   ├── Errors/
│   ├── IdGenerators/
│   ├── Infrastructure/
│   │   └── Persistence/
│   ├── Mappers/
│   ├── Models/
│   ├── Repositories/
│   │   ├── Implementations/
│   │   └── Interfaces/
│   ├── Services/
│   │   ├── Implementations/
│   │   └── Interfaces/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Dockerfile
├── ApiFunkosConjunta.Tests/
│   ├── Controllers/
│   ├── Errors/
│   ├── Mappers/
│   ├── Repositories/
│   └── Services/
├── ApiFunkosConjunta 1/
│   ├── Funko/
│   ├── Categoria/
│   ├── Proveedor/
│   └── Obtención de Token.yml
├── compose.yaml
└── ApiFunkosConjunta.slnx
```

La carpeta `ApiFunkosConjunta 1` contiene la colección de Bruno exportada, organizada por entidad y con peticiones para obtener el token y comprobar las operaciones CRUD.

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

La API publica su documentación con **Swashbuckle**. En desarrollo está disponible la interfaz web de Swagger UI:

```text
http://localhost:5087/swagger
```

Y el documento OpenAPI en formato JSON:

```text
http://localhost:5087/swagger/v1/swagger.json
```

La configuración vive en `Infrastructure/SwaggerConfig.cs` y incluye:

| Elemento | Para qué sirve |
|---|---|
| `SwaggerDoc("v1", ...)` | Título, versión, descripción y contacto de la API |
| `AddSecurityDefinition("Bearer", ...)` | Esquema de seguridad JWT: habilita el botón **Authorize** de Swagger UI |
| `IncludeXmlComments(...)` | Comentarios XML de los controladores y DTO como descripciones y ejemplos |
| `EnableAnnotations()` | Atributos `[SwaggerOperation]`, `[SwaggerResponse]`, `[SwaggerParameter]`, `[SwaggerRequestBody]` y `[SwaggerSchema]` |
| `ExampleFilters()` | Ejemplos de petición mediante `IExamplesProvider<T>` (`Common/Examples`) |
| `OperationFilter<SwaggerExamplesFilter>()` | Filtro propio que añade ejemplos `ProblemDetails` a las respuestas de error |

Para probar los endpoints protegidos desde Swagger UI:

1. Abre <http://localhost:5087/swagger>.
2. Pulsa **Authorize**, pega el token (sin la palabra `Bearer`) y confirma.
3. Ejecuta `POST /api/Auth/login` para obtener el token si aún no lo tienes.

Las respuestas de error se devuelven como `ProblemDetails` (RFC 9457), de modo que la documentación y la respuesta real usan el mismo formato.

### 7.6. CORS

La política de CORS se registra en `Infrastructure/CorsConfig.cs`:

| Entorno | Política | Comportamiento |
|---|---|---|
| Desarrollo | `AllowAll` | Permite cualquier origen, método y cabecera |
| Producción | `ProductionPolicy` | Solo los orígenes de la sección `Cors:AllowedOrigins` |

En producción los orígenes se configuran en `appsettings.Production.json` o mediante las variables de entorno `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1`, ...

---

## 8. Ejemplos de peticiones

### 8.1. Iniciar sesión

Petición:

```http
POST /api/auth/login
Content-Type: application/json
```

Cuerpo:

```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

Respuesta correcta: `200 OK`.

```json
{
  "token": "JWT_GENERADO"
}
```

Si las credenciales son incorrectas, la API devuelve `401 Unauthorized`.

### 8.2. Crear un Funko

Petición:

```http
POST /api/funkos
Authorization: Bearer JWT_GENERADO
Content-Type: application/json
```

Cuerpo de ejemplo:

```json
{
  "nombre": "Iron Man",
  "categoriaId": "ID_CATEGORIA",
  "proveedorId": "64b000000000000000000001",
  "stock": 15,
  "precio": 29.99,
  "imagen": "imagenes/funkos/iron-man.png"
}
```

`ID_CATEGORIA` debe sustituirse por el identificador real de una categoría existente. El `proveedorId` debe corresponder a un proveedor existente en MongoDB.

### 8.3. Crear una categoría

```http
POST /api/categorias
Authorization: Bearer JWT_GENERADO
Content-Type: application/json
```

```json
{
  "nombre": "Videojuegos Retro"
}
```

El identificador se genera automáticamente.

### 8.4. Crear un proveedor

```http
POST /api/proveedores
Authorization: Bearer JWT_GENERADO
Content-Type: application/json
```

```json
{
  "nombre": "Distribuciones Retro",
  "contacto": "contacto@distribucionesretro.com",
  "pais": "España"
}
```

El identificador `ObjectId` se genera al insertar el documento en MongoDB.

---

## 9. Autenticación y autorización

La API utiliza autenticación **JWT Bearer**.

El endpoint `POST /api/auth/login` comprueba las credenciales del administrador configuradas bajo la sección `Jwt` de `appsettings.json`. Si son correctas, `AuthService` genera un token firmado con HMAC-SHA256 que incluye:

- El nombre del usuario.
- El rol `Administrador`.
- El emisor y la audiencia configurados.
- Una expiración de dos horas.

Los controladores aplican `[Authorize(Roles = "Administrador")]` únicamente a los métodos `POST`, `PUT` y `DELETE`. Los métodos `GET` permanecen públicos.

Para probar las operaciones protegidas, el token se envía en la cabecera:

```http
Authorization: Bearer JWT_GENERADO
```

En la configuración de desarrollo incluida en el proyecto, las credenciales de demostración son:

```text
Usuario: admin
Contraseña: Admin123!
```

**Seguridad:** estas credenciales y la clave JWT del `appsettings.json` son valores de desarrollo. Antes de desplegar la aplicación deben sustituirse y trasladarse a variables de entorno o a un sistema seguro de gestión de secretos. No se deben reutilizar en producción.

---

## 10. Caché Redis

`CatalogoService` utiliza la abstracción `IDistributedCache`, registrada mediante `AddStackExchangeRedisCache`.

Se almacenan en caché:

- Los listados paginados de Funkos, categorías y proveedores.
- Las consultas individuales por identificador.

Las entradas de datos tienen una expiración absoluta de **5 minutos**.

Para los listados se utiliza una clave de versión por entidad. Al crear, actualizar o eliminar un registro, el servicio cambia la versión correspondiente, de modo que las siguientes consultas de listado utilizan una nueva clave y no recuperan resultados anteriores. Cuando se modifica o elimina un registro concreto, también se elimina su entrada individual de caché.

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

## 11. Configuración y ejecución

### 11.1. Requisitos previos

- .NET 10 SDK.
- Docker Desktop o Docker Engine con Docker Compose.
- Rider, Visual Studio o un editor compatible con .NET.
- Bruno para ejecutar la colección de peticiones incluida.

No es necesario instalar PostgreSQL, MongoDB ni Redis directamente en el equipo: Docker Compose proporciona los tres servicios.

### 11.2. Ejecutar toda la aplicación con Docker Compose

Desde la carpeta raíz, donde se encuentra `compose.yaml`, ejecutar:

```bash
docker compose up -d --build
```

Esto levanta los siguientes servicios:

| Servicio Compose | Imagen | Puerto del host |
|---|---|---|
| `apifunkosconjunta` | Construida desde el Dockerfile | `5000` |
| `postgres` | `postgres:18` | `5432` |
| `mongodb` | `mongo:8` | `27017` |
| `redis` | `redis:8` | `6379` |

La API queda disponible en:

```text
http://localhost:5000
```

Dentro de la red de Docker, la API se conecta a los servicios mediante sus nombres Compose:

```text
PostgreSQL → postgres:5432
MongoDB    → mongodb:27017
Redis      → redis:6379
```

### 11.3. Ejecutar la API desde Rider

También es posible ejecutar la API desde Rider y mantener las bases de datos y Redis en Docker.

Primero, desde la raíz del proyecto:

```bash
docker compose up -d postgres mongodb redis
```

Después, ejecutar el proyecto `ApiFunkosConjunta` desde Rider utilizando el perfil `http` o `https`.

Según `launchSettings.json`, las direcciones locales son:

```text
HTTP  → http://localhost:5087
HTTPS → https://localhost:7274
```

En este modo, `appsettings.json` utiliza `localhost` para conectarse a los puertos publicados por Docker.

### 11.4. Persistencia de Docker

Compose define volúmenes persistentes:

```text
postgres_data
mongodb_data
redis_data
```

Los datos sobreviven al reinicio de los contenedores. Para detener los servicios sin eliminar los datos:

```bash
docker compose down
```

No utilizar `docker compose down -v` salvo que se quiera eliminar expresamente también el contenido de los volúmenes.

---

## 12. Inicialización de datos

En el entorno `Development`, `Program.cs` ejecuta los seeders:

- `CategoriaSeeder`: crea las categorías iniciales si todavía no existen.
- `ProveedorSeeder`: inserta o reemplaza los proveedores iniciales identificados por sus ObjectId.
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

La inicialización de PostgreSQL utiliza `EnsureCreatedAsync()` para crear la base de datos y su esquema cuando corresponde. No se utiliza un flujo de migraciones de Entity Framework Core en esta versión del proyecto.

---

## 13. Validación y gestión de errores

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

Las respuestas de error se construyen con `ProblemDetails` (RFC 9457) mediante las fábricas de `Common/Http/ApiProblem.cs`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9457#section-10.5",
  "title": "No se ha encontrado el recurso solicitado.",
  "status": 404,
  "detail": "No se ha encontrado la categoría con el id: D2M_Lk6kKe6.",
  "instance": "/api/categorias/D2M_Lk6kKe6"
}
```

Los errores de validación del modelo devuelven un `ValidationProblemDetails`, que añade el diccionario `errors` con el detalle de cada campo.

---

## 14. Pruebas automatizadas

El proyecto de pruebas es `ApiFunkosConjunta.Tests` y utiliza NUnit, Moq y FluentAssertions.

La estructura cubre las siguientes áreas:

- **Controllers**: comprobación de las respuestas HTTP y del comportamiento de los controladores.
- **Services**: casos correctos e incorrectos de los servicios y del servicio coordinador `CatalogoService`.
- **Repositories**: operaciones de acceso a PostgreSQL y MongoDB.
- **Mappers**: conversiones entre modelos y DTO.
- **Errors**: comprobaciones de los errores de dominio.

### 14.1. Pruebas unitarias

Los mocks de Moq permiten aislar las dependencias y comprobar el comportamiento de cada capa. FluentAssertions se utiliza para expresar las aserciones de forma legible.

También se comprueba la interacción con `IDistributedCache`, incluyendo la lectura de datos y la invalidación de las entradas al modificar el catálogo.

### 14.2. Testcontainers

Las pruebas de repositorio utilizan contenedores reales:

- `Testcontainers.PostgreSql` con `postgres:18` para los repositorios de categorías y Funkos.
- `Testcontainers.MongoDb` con `mongo:8` para el repositorio de proveedores.

Cada conjunto de pruebas prepara sus datos y utiliza los contenedores para comprobar el acceso real a los respectivos motores de persistencia.

Docker debe estar iniciado para ejecutar estas pruebas.

### 14.3. Ejecutar los tests

Desde la carpeta raíz de la solución:

```bash
dotnet test ApiFunkosConjunta.Tests/ApiFunkosConjunta.Tests.csproj
```

---

## 15. Pruebas manuales con Bruno

El proyecto incluye una colección exportada de Bruno en:

```text
ApiFunkosConjunta 1/
```

La colección está organizada por entidades:

```text
ApiFunkosConjunta 1/
├── Funko/
├── Categoria/
├── Proveedor/
├── Obtención de Token.yml
└── opencollection.yml
```

Las peticiones permiten probar:

- Obtención del token de administrador.
- Listado de Funkos, categorías y proveedores.
- Creación con token y sin token.
- Actualización con token y sin token.
- Eliminación con token y sin token.

Para las operaciones protegidas, se debe seleccionar **Auth → Bearer Token** en Bruno y utilizar el JWT obtenido mediante `POST /api/auth/login`.

Los resultados esperados para las operaciones de escritura son:

| Situación | Resultado esperado |
|---|---|
| POST, PUT o DELETE sin token | `401 Unauthorized` |
| POST, PUT o DELETE con token de administrador válido | La operación se ejecuta si los datos y las relaciones son correctos |
| Login con credenciales incorrectas | `401 Unauthorized` |

Las operaciones de eliminación de categorías y proveedores deben probarse con registros que no tengan Funkos asociados, ya que el servicio impide eliminar entidades que estén siendo utilizadas.

---

## 16. Inyección de dependencias

La configuración de servicios se centraliza en `Infrastructure/DependencyInjection.cs`, que registra:

- Los repositorios y servicios de categorías.
- Los repositorios y servicios de proveedores.
- Los repositorios y servicios de Funkos.
- `CatalogoService`.
- `AuthService`.
- La autenticación JWT y la autorización por roles.
- La documentación Swagger/OpenAPI (`Infrastructure/SwaggerConfig.cs`).
- La política de CORS (`Infrastructure/CorsConfig.cs`).

`Infrastructure/Persistence/PersistenceExtensions.cs` registra:

- `AppDbContext` con PostgreSQL.
- `IDistributedCache` con Redis.
- `IMongoClient`, `IMongoDatabase` e `IMongoCollection<Proveedor>` con MongoDB.

Las dependencias se inyectan mediante los constructores, evitando que los controladores tengan que crear manualmente sus servicios o repositorios.

---

## 17. Decisiones de diseño

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

JWT permite identificar al usuario y transportar su rol. Los atributos de autorización restringen únicamente las operaciones de escritura, manteniendo públicas las consultas.

---

## 18. Conclusión

**ApiFunkosConjunta** implementa una API REST para la gestión de Funkos, categorías y proveedores mediante una arquitectura por capas y dos sistemas de persistencia.

La solución combina:

```text
PostgreSQL → Funkos y Categorías
MongoDB    → Proveedores
Redis      → Caché distribuida
JWT        → Autenticación y rol Administrador
Docker     → Infraestructura reproducible
NUnit      → Pruebas automatizadas
Bruno      → Pruebas manuales de la API
```

La separación entre controladores, servicios y repositorios permite mantener la lógica de negocio organizada, mientras que `CatalogoService` centraliza las reglas que afectan a las relaciones entre entidades y la gestión de caché.

El proyecto incluye tanto pruebas automatizadas como una colección de Bruno para comprobar el comportamiento de los endpoints y las restricciones de acceso.
