# Sistema de Gestión de Inventarios - Backend .NET Core

Este proyecto implementa un sistema de gestión de inventarios con arquitectura de microservicios utilizando .NET Core y SQL Server, enfocado en el desarrollo backend.

## Enfoque del Proyecto

Este proyecto se enfoca exclusivamente en el desarrollo backend con microservicios, considerando que se trata de una evaluación técnica para un puesto de Backend Developer en .NET.

## Requisitos

Para ejecutar este proyecto necesitas:
- .NET 6.0 SDK o superior
- SQL Server (Express o edición completa)
- Visual Studio 2022 o VS Code

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
└── README.md
```

## Ejecución del backend

### Configuración de la Base de Datos

1. Abre SQL Server Management Studio o cualquier cliente de SQL
2. Ejecuta el script `scripts/Database.sql` para crear la base de datos y las tablas necesarias
3. Verifica que la base de datos `InventoryDB` se ha creado correctamente

### Configuración de los Microservicios

1. Abre cada proyecto en Visual Studio o VS Code
2. Actualiza las cadenas de conexión en los archivos `appsettings.json` si es necesario:
   - `ProductService.API/appsettings.json`
   - `TransactionService.API/appsettings.json`

### Ejecución de los Microservicios

#### Usando Visual Studio:

1. Configura la solución para iniciar múltiples proyectos:
   - `ProductService.API`
   - `TransactionService.API`
2. Presiona F5 o haz clic en el botón "Iniciar"

#### Usando línea de comandos:

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

## Tecnologías Utilizadas

- .NET 6.0
- ASP.NET Core Web API
- SQL Server
- Dapper
- Swagger/OpenAPI