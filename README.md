# Sistema de Facturación Electrónica - FactuMarket S.A.

## Descripción del Proyecto

Este proyecto implementa un sistema de microservicios para la facturación electrónica de FactuMarket S.A., siguiendo los principios de Clean Architecture y el patrón MVC. El sistema está diseñado para resolver los problemas de demoras en emisión de facturas, duplicación de información, falta de trazabilidad y escasa flexibilidad tecnológica.

## Arquitectura del Sistema

### Microservicios Implementados

1. **Servicio de Facturas** (.NET Core)
   - **Responsabilidad**: Creación y gestión de facturas electrónicas
   - **Tecnología**: ASP.NET Core con Clean Architecture
   - **Base de datos**: Oracle (transaccional)
   - **Puerto**: 5000
   - **Endpoints principales**:
     - `POST /api/facturas` - Crear factura
     - `GET /api/facturas/{id}` - Consultar factura por ID
     - `GET /api/facturas?fechaInicio=xx&fechaFin=yy` - Listar facturas por rango
     - `GET /api/health` - Estado del servicio

2. **Servicio de Client** (Ruby on Rails)
   - **Responsabilidad**: Gestión de información de client
   - **Tecnología**: Ruby on Rails con MVC
   - **Base de datos**: Oracle (transaccional)
   - **Puerto**: 3001
   - **Endpoints principales**:
     - `POST /api/v1/client` - Registrar client
     - `GET /api/v1/client/{id}` - Consultar client por ID
     - `GET /api/v1/client` - Listar client
     - `PUT /api/v1/client/{id}` - Actualizar client
     - `DELETE /api/v1/client/{id}` - Eliminar client
     - `GET /health` - Estado del servicio

3. **Servicio de Auditoría** (Ruby on Rails + MongoDB)
   - **Responsabilidad**: Registro de eventos del sistema
   - **Tecnología**: Ruby on Rails con Mongoid
   - **Base de datos**: MongoDB (NoSQL)
   - **Puerto**: 3002
   - **Endpoints principales**:
     - `POST /api/v1/audit` - Registrar evento
     - `GET /api/v1/audit/{id}` - Consultar evento por ID
     - `GET /api/v1/audit` - Listar eventos con filtros
     - `GET /api/v1/audit/entity/{entity}/{entity_id}` - Eventos por entidad
     - `GET /api/v1/audit/service/{service}` - Eventos por servicio
     - `GET /health` - Estado del servicio

## Principios Aplicados

### Microservicios
- **Independencia**: Cada servicio tiene su propia base de datos y despliegue
- **Escalabilidad**: Servicios pueden escalarse independientemente
- **Despliegue autónomo**: Cada servicio puede desplegarse sin afectar otros
- **Comunicación**: REST APIs para comunicación síncrona, eventos para auditoría

### Clean Architecture
- **Separación de capas**: Dominio, Aplicación, Infraestructura, Presentación
- **Inversión de dependencias**: Las capas internas no dependen de las externas
- **Testabilidad**: Lógica de dominio aislada y testeable
- **Independencia de frameworks**: La lógica de negocio no depende de frameworks externos

### MVC (Model-View-Controller)
- **Controllers**: Manejan las peticiones HTTP y coordinan la lógica
- **Models**: Representan la lógica de negocio y validaciones
- **Views**: Serialización de respuestas JSON (APIs REST)

## Requisitos del Sistema

## Instalación y Ejecución

### Opción 1: Docker Compose (Recomendado)

```bash
# Clonar el repositorio
git clone git@github.com:mailo2202/prueba-tecnica-double-v.git
cd prueba-tecnica-double-v

# Levantar todos los servicios
docker-compose up -d

# Verificar que todos los servicios estén corriendo
docker-compose ps

# Ver logs en tiempo real
docker-compose logs -f
```

### Opción 2: Ejecución Manual

#### Servicio de Facturas (.NET)
```bash
cd FacturasService
dotnet restore
dotnet run --project src/FacturasService.WebAPI
```

#### Servicio de Client (Ruby)
```bash
cd ClientService
bundle install
rails server -p 3001
```

#### Servicio de Auditoría (Ruby)
```bash
cd AuditoriaService
bundle install
rails server -p 3002
```

## Endpoints de Prueba

### Servicio de Client
```bash
# Crear cliente
curl -X POST http://localhost:3001/api/v1/client \
  -H "Content-Type: application/json" \
  -d '{
    "cliente": {
      "nombre": "Empresa ABC S.A.S",
      "identificacion": "900123456-1",
      "email": "contacto@empresaabc.com",
      "direccion": "Calle 123 #45-67, Bogotá",
      "telefono": "3001234567"
    }
  }'

# Consultar cliente
curl http://localhost:3001/api/v1/client/1

# Listar clientes
curl http://localhost:3001/api/v1/client

# Actualizar cliente
curl -X PUT http://localhost:3001/api/v1/client/1 \
  -H "Content-Type: application/json" \
  -d '{
    "cliente": {
      "nombre": "Empresa ABC S.A.S Actualizada",
      "email": "nuevo@empresaabc.com"
    }
  }'
```

### Servicio de Facturas
```bash
# Crear factura
curl -X POST http://localhost:5000/api/facturas \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": 1,
    "monto": 150000.00,
    "fechaEmision": "2024-01-15",
    "descripcion": "Servicios de consultoría"
  }'

# Consultar factura
curl http://localhost:5000/api/facturas/1

# Listar facturas por rango
curl "http://localhost:5000/api/facturas?fechaInicio=2024-01-01&fechaFin=2024-01-31"
```

### Servicio de Auditoría
```bash
# Registrar evento manualmente
curl -X POST http://localhost:3002/api/v1/audit \
  -H "Content-Type: application/json" \
  -d '{
    "audit_event": {
      "event_type": "READ",
      "entity": "Invoice",
      "entity_id": 1,
      "details": "Manual invoice query",
      "service": "FacturasService"
    }
  }'

# Consultar eventos de una factura
curl http://localhost:3002/api/v1/audit/entity/Invoice/1

# Consultar eventos por servicio
curl http://localhost:3002/api/v1/audit/service/InvoicesService

# Listar eventos recientes
curl http://localhost:3002/api/v1/audit

## Validaciones Implementadas

### Facturas
- Cliente debe existir en el sistema (validación síncrona)
- Monto debe ser mayor a 0
- Fecha de emisión debe ser válida y no futura
- Descripción es requerida (máximo 500 caracteres)
- Generación automática de número de factura único

### Clientes
- Identificación única en el sistema
- Email válido según RFC
- Todos los campos requeridos deben estar presentes
- Normalización automática de datos (nombres, emails)

### Auditoría
- Evento debe ser uno de los tipos válidos
- Entidad e ID de entidad son requeridos
- Servicio de origen es requerido
- Timestamp automático si no se proporciona

## Base de Datos

### Oracle (Transaccional)
- **Tabla CLIENTES**: Información de clientes
- **Tabla FACTURAS**: Información de facturas
- **Índices**: Optimización de consultas frecuentes
- **Triggers**: Actualización automática de timestamps
- **Procedimientos**: Operaciones complejas

### MongoDB (Auditoría)
- **Colección evento_auditorias**: Eventos del sistema
- **Colección auditoria_stats**: Estadísticas agregadas
- **Colección system_config**: Configuración del sistema
- **Índices**: Optimización de consultas por entidad, servicio, fecha
- **Vistas**: Consultas predefinidas comunes
```
## Pruebas Unitarias

Las pruebas unitarias están implementadas en cada servicio:

### Servicio de Facturas (.NET)
```bash
cd InvoicesService
dotnet test
```

### Servicio de Clientes (Ruby)
```bash
cd ClientService
bundle exec rspec
```

### Servicio de Auditoría (Ruby)
```bash
cd AuditService
bundle exec rspec
```

## Comunicación Entre Servicios

### Síncrona (REST)
- **Validación de cliente**: FacturasService → ClientService
- **Consultas inmediatas**: Requieren respuesta inmediata

### Asíncrona (Eventos)
- **Auditoría**: Todos los servicios → AuditoriaService
- **No bloquea operaciones**: Mejora rendimiento

## Monitoreo y Logs

### Health Checks
- Cada servicio expone endpoint `/health`
- Verificación de conectividad a base de datos
- Estado general del servicio

### Logs Centralizados
- Logs estructurados en formato JSON
- Rotación automática de logs
- Niveles configurables por servicio

### Métricas de Auditoría
- Conteo de eventos por servicio
- Tasa de errores
- Tiempo de respuesta promedio
- Eventos críticos en tiempo real

## Escalabilidad y Rendimiento

### Escalabilidad Horizontal
- Cada microservicio puede escalarse independientemente
- Load balancing con Nginx
- Base de datos Oracle con clustering
- MongoDB con réplicas

### Optimizaciones
- Índices optimizados en ambas bases de datos
- Caché de consultas frecuentes
- Conexiones pool configuradas
- Compresión de respuestas HTTP

## Seguridad

### Autenticación y Autorización
- Tokens JWT para autenticación (implementación futura)
- Validación de permisos por endpoint
- Rate limiting por IP

### Protección de Datos
- Encriptación en tránsito (HTTPS)
- Encriptación en reposo (base de datos)
- Sanitización de inputs
- Validación estricta de datos

## Despliegue en Producción

### Variables de Entorno
```bash
# Oracle Database
ORACLE_HOST=oracle-server
ORACLE_PORT=1521
ORACLE_DATABASE=XE
ORACLE_USERNAME=facturas_user
ORACLE_PASSWORD=secure_password

# MongoDB
MONGODB_HOST=mongodb-server
MONGODB_DATABASE=auditoria_db

# Servicios
CLIENTES_SERVICE_URL=http://client-service:3001
AUDITORIA_SERVICE_URL=http://auditoria-service:3002
```

### Configuración de Producción
- SSL/TLS habilitado
- Logs centralizados
- Monitoreo con Prometheus/Grafana
- Backup automático de bases de datos
- Health checks con alertas

## Contribución

### Convenciones de Código
- **.NET**: C# coding conventions de Microsoft
- **Ruby**: Ruby style guide oficial
- **Git**: Conventional commits
- **Documentación**: Comentarios en español

### Proceso de Desarrollo
1. Fork del repositorio
2. Crear rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -m 'feat: agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

### Pruebas Requeridas
- Cobertura mínima del 80% en lógica de dominio
- Pruebas de integración para APIs
- Pruebas de carga para endpoints críticos
- Validación de seguridad

## Troubleshooting

### Problemas Comunes

#### Servicio no responde
```bash
# Verificar estado
docker-compose ps

# Ver logs
docker-compose logs servicio-name

# Reiniciar servicio
docker-compose restart servicio-name
```

#### Error de conexión a base de datos
```bash
# Verificar conectividad
docker-compose exec oracle sqlplus sys/OraclePass123@//localhost:1521/XE as sysdba

# Verificar configuración
docker-compose exec servicio-name env | grep -i oracle
```

#### Problemas de memoria
```bash
# Limpiar contenedores
docker system prune -f

# Verificar uso de recursos
docker stats
```

## Contacto y Soporte

- **Desarrollador**: Camilo Muñoz Giraldo
- **Empresa**: FactuMarket S.A.
- **Fecha**: octubre 2025
- **Versión**: 1.0.0