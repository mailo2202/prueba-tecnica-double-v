# Diagrama de Arquitectura - Sistema de Facturación Electrónica

## Diagrama de Alto Nivel

```mermaid
graph TB
    subgraph "Cliente/Usuario"
        U[Usuario Final]
        API[Cliente API]
    end
    
    subgraph "Capa de Presentación"
        NGINX[Nginx Reverse Proxy]
    end
    
    subgraph "Microservicios"
        FS[FacturasService<br/>.NET Core<br/>Puerto 5000]
        CS[ClientesService<br/>Ruby on Rails<br/>Puerto 3001]
        AS[AuditoriaService<br/>Ruby on Rails<br/>Puerto 3002]
    end
    
    subgraph "Bases de Datos"
        ORACLE[(Oracle Database<br/>Transaccional)]
        MONGO[(MongoDB<br/>Auditoría/Logs)]
    end
    
    subgraph "Infraestructura"
        DOCKER[Docker Containers]
        COMPOSE[Docker Compose]
    end
    
    U --> API
    API --> NGINX
    NGINX --> FS
    NGINX --> CS
    NGINX --> AS
    
    FS --> ORACLE
    CS --> ORACLE
    AS --> MONGO
    
    FS -.->|Eventos| AS
    CS -.->|Eventos| AS
    
    DOCKER --> FS
    DOCKER --> CS
    DOCKER --> AS
    DOCKER --> ORACLE
    DOCKER --> MONGO
    
    COMPOSE --> DOCKER
```

## Flujo de Comunicación Entre Servicios

```mermaid
sequenceDiagram
    participant C as Cliente
    participant F as FacturasService
    participant CS as ClientesService
    participant AS as AuditoriaService
    participant O as Oracle
    participant M as MongoDB
    
    C->>F: POST /api/facturas
    F->>CS: GET /api/v1/clientes/{id}
    CS->>O: SELECT cliente
    O-->>CS: Cliente encontrado
    CS-->>F: Cliente válido
    F->>O: INSERT factura
    O-->>F: Factura creada
    F->>AS: POST /api/v1/auditoria
    AS->>M: INSERT evento
    M-->>AS: Evento registrado
    AS-->>F: Auditoría OK
    F-->>C: Factura creada (201)
    
    Note over F,AS: Comunicación asíncrona<br/>para auditoría
```

## Arquitectura Clean Architecture - FacturasService

```mermaid
graph TB
    subgraph "Capa de Presentación"
        API[WebAPI Controllers]
        SWAGGER[Swagger UI]
    end
    
    subgraph "Capa de Aplicación"
        CMD[Commands]
        QRY[Queries]
        VAL[Validators]
        MEDIATR[MediatR]
    end
    
    subgraph "Capa de Dominio"
        ENT[Entities]
        REPO_INT[Repository Interfaces]
        DOM_SVC[Domain Services]
        RULES[Business Rules]
    end
    
    subgraph "Capa de Infraestructura"
        EF[Entity Framework]
        REPO_IMPL[Repository Implementations]
        EXT_SVC[External Services]
        HTTP[HttpClient]
    end
    
    API --> CMD
    API --> QRY
    CMD --> MEDIATR
    QRY --> MEDIATR
    MEDIATR --> REPO_INT
    MEDIATR --> DOM_SVC
    REPO_INT --> REPO_IMPL
    REPO_IMPL --> EF
    DOM_SVC --> EXT_SVC
    EXT_SVC --> HTTP
    
    VAL --> CMD
    RULES --> ENT
```

## Patrón MVC - ClientesService

```mermaid
graph TB
    subgraph "Model"
        CLIENTE[Cliente Model]
        VALIDATIONS[Validations]
        CALLBACKS[Callbacks]
    end
    
    subgraph "View"
        JSON[JSON Serialization]
        RESPONSE[HTTP Response]
    end
    
    subgraph "Controller"
        CTRL[ClientesController]
        ACTIONS[Actions]
        FILTERS[Before Actions]
    end
    
    CTRL --> ACTIONS
    ACTIONS --> CLIENTE
    CLIENTE --> VALIDATIONS
    CLIENTE --> CALLBACKS
    ACTIONS --> JSON
    JSON --> RESPONSE
    FILTERS --> ACTIONS
```

## Estrategia de Persistencia

```mermaid
graph LR
    subgraph "Datos Transaccionales"
        CLIENTES[Clientes]
        FACTURAS[Facturas]
        ORACLE[(Oracle Database)]
    end
    
    subgraph "Datos de Auditoría"
        EVENTOS[Eventos de Auditoría]
        LOGS[Logs del Sistema]
        MONGO[(MongoDB)]
    end
    
    CLIENTES --> ORACLE
    FACTURAS --> ORACLE
    EVENTOS --> MONGO
    LOGS --> MONGO
    
    ORACLE -.->|ACID| TRANSACCIONES[Transacciones<br/>Consistentes]
    MONGO -.->|Flexible| DOCUMENTOS[Documentos<br/>Flexibles]
```

## Principios de Microservicios Aplicados

### 1. Independencia
- Cada servicio tiene su propia base de datos
- Despliegue independiente
- Tecnologías diferentes por servicio

### 2. Escalabilidad
- Servicios pueden escalarse horizontalmente
- Load balancing automático
- Recursos independientes

### 3. Resiliencia
- Circuit breakers para servicios externos
- Retry policies
- Fallback mechanisms

### 4. Observabilidad
- Logs centralizados
- Métricas de auditoría
- Health checks

## Comunicación Entre Servicios

### Síncrona (REST)
- Validación de cliente antes de crear factura
- Consultas inmediatas que requieren respuesta

### Asíncrona (Eventos)
- Registro de auditoría
- Notificaciones
- Procesamiento en background

## Consideraciones de Seguridad

### Autenticación
- Tokens JWT (implementación futura)
- Rate limiting
- Validación de permisos

### Protección de Datos
- Encriptación en tránsito
- Sanitización de inputs
- Validación estricta

## Monitoreo y Logs

### Health Checks
- Endpoint `/health` en cada servicio
- Verificación de conectividad a BD
- Estado general del servicio

### Auditoría
- Todos los eventos registrados
- Trazabilidad completa
- Análisis de patrones

## Escalabilidad Futura

### Integración con DIAN
- Servicio adicional para integración tributaria
- API externa de la DIAN
- Validación de documentos electrónicos

### Nuevas Funcionalidades
- Servicio de Notificaciones
- Servicio de Reportes
- Servicio de Configuración
