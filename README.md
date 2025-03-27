# Sistema de Gestión de Inventarios - Backend .NET Core

Este proyecto implementa un sistema de gestión de inventarios con arquitectura de microservicios utilizando .NET Core y SQL Server, enfocado en el desarrollo backend.

## Enfoque del Proyecto

Este proyecto se enfoca exclusivamente en el desarrollo backend con microservicios, considerando que se trata de una evaluación técnica para un puesto de Backend Developer en .NET.

## Requisitos

Para ejecutar este proyecto necesitas:
- .NET 6.0 SDK o superior
- SQL Server (Express o edición completa)
- Visual Studio 2022 o VS Code
- Docker (opcional, para ejecución containerizada)

## Estructura del Proyecto

```
InventoryManagementSystem/
│
├── src/
│   └── Services/
│       ├── ProductService/      # Microservicio de productos
│       └── TransactionService/  # Microservicio de transacciones
│
├── scripts/
│   └── Database.sql             # Script para crear la base de datos
│
├── docker/
│   ├── docker-compose.yml       # Configuración para despliegue con Docker Compose
│   ├── Dockerfile.product       # Dockerfile para el servicio de productos
│   └── Dockerfile.transaction   # Dockerfile para el servicio de transacciones
│
└── README.md
```

## Ejecución del backend

### Opción 1: Ejecución con Docker

1. Asegúrate de tener Docker y Docker Compose instalados en tu sistema.

2. Desde la carpeta raíz del proyecto, ejecuta:
   ```
   docker-compose -f docker/docker-compose.yml up -d
   ```

3. Este comando descargará las imágenes necesarias, creará los contenedores para los servicios y la base de datos SQL Server, y ejecutará las migraciones iniciales.

4. Accede a las APIs a través de:
   - API de Productos: `http://localhost:5001/swagger` (para documentación)
   - API de Transacciones: `http://localhost:5003/swagger` (para documentación)

5. Para detener los servicios:
   ```
   docker-compose -f docker/docker-compose.yml down
   ```

### Opción 2: Configuración Manual

#### Configuración de la Base de Datos
1. Abre SQL Server Management Studio o cualquier cliente de SQL
2. Ejecuta el script `scripts/Database.sql` para crear la base de datos y las tablas necesarias
3. Verifica que la base de datos `InventoryDB` se ha creado correctamente

#### Configuración de los Microservicios
1. Abre cada proyecto en Visual Studio o VS Code
2. Actualiza las cadenas de conexión en los archivos `appsettings.json` si es necesario:
   - `ProductService.API/appsettings.json`
   - `TransactionService.API/appsettings.json`

#### Ejecución de los Microservicios
##### Usando Visual Studio:
1. Configura la solución para iniciar múltiples proyectos:
   - `ProductService.API`
   - `TransactionService.API`
2. Presiona F5 o haz clic en el botón "Iniciar"

##### Usando línea de comandos:
1. Abre dos terminales diferentes
2. En la primera terminal, navega al directorio del microservicio de productos:
   ```
   cd src/Services/ProductService/ProductService.API
   dotnet run
   ```
3. En la segunda terminal, navega al directorio del microservicio de transacciones:
   ```
   cd src/Services/TransactionService/TransactionService.API
   dotnet run
   ```
4. Abre un navegador y accede a:
   - API de Productos: `https://localhost:5001/swagger` (para documentación)
   - API de Transacciones: `https://localhost:5003/swagger` (para documentación)

## Detalles de la Implementación con Docker

### Estructura de Docker
- `docker-compose.yml`: Orquesta la ejecución de todos los servicios:
  - SQL Server en un contenedor
  - ProductService API
  - TransactionService API

### Variables de Entorno
Los contenedores utilizan variables de entorno para la configuración:
- `ASPNETCORE_ENVIRONMENT`: Establece el entorno de ejecución (Development, Production)
- `ConnectionStrings__DefaultConnection`: Cadena de conexión a la base de datos
- `ASPNETCORE_URLS`: URLs donde se exponen los servicios

### Volúmenes
- Se utiliza un volumen para persistir los datos de SQL Server entre reinicios de los contenedores

### Red
- Los servicios se comunican a través de una red Docker interna
- Los puertos de los servicios se mapean a puertos del host para acceso externo

## APIs y Endpoints

### ProductService API
- `GET /api/products` - Obtener todos los productos
- `GET /api/products/{id}` - Obtener un producto por ID
- `GET /api/products/filter` - Filtrar productos
- `POST /api/products` - Crear un nuevo producto
- `PUT /api/products/{id}` - Actualizar un producto
- `DELETE /api/products/{id}` - Eliminar un producto
- `PATCH /api/products/{id}/stock` - Actualizar stock de un producto

### TransactionService API
- `GET /api/transactions` - Obtener todas las transacciones
- `GET /api/transactions/{id}` - Obtener una transacción por ID
- `GET /api/transactions/filter` - Filtrar transacciones
- `GET /api/transactions/product/{productId}` - Obtener transacciones por producto
- `POST /api/transactions` - Crear una nueva transacción
- `PUT /api/transactions/{id}` - Actualizar una transacción
- `DELETE /api/transactions/{id}` - Eliminar una transacción

## Funcionalidades Principales
- Gestión completa de productos (CRUD)
- Registro de transacciones (compras y ventas)
- Validación de stock en ventas
- Actualización automática de inventario
- Filtros dinámicos para productos y transacciones
- Comunicación entre microservicios
- Manejo de excepciones para integridad referencial (prevención de eliminación de productos con transacciones)

## Tecnologías Utilizadas
- .NET 6.0
- ASP.NET Core Web API
- SQL Server
- Dapper
- Swagger/OpenAPI
- Docker
- Docker Compose
