# Estructura del Repositorio - FactuMarket S.A.

## Organización del Proyecto

Este repositorio contiene un sistema de microservicios implementado como monorepo para facilitar la evaluación y demostración de la arquitectura propuesta.

### Estructura de Carpetas

```
facturacion-electronica/
├── InvoicesService/              # Microservicio .NET Core
│   ├── src/                      # Código fuente
│   ├── tests/                    # Pruebas unitarias
│   ├── Dockerfile               # Imagen Docker
│   └── README.md                # Documentación específica
├── ClientService/              # Microservicio Ruby on Rails
│   ├── app/                     # Aplicación Rails
│   ├── spec/                    # Pruebas RSpec
│   ├── Dockerfile              # Imagen Docker
│   └── README.md               # Documentación específica
├── AuditService/            # Microservicio Ruby + MongoDB
│   ├── app/                     # Aplicación Rails
│   ├── spec/                    # Pruebas RSpec
│   ├── Dockerfile              # Imagen Docker
│   └── README.md               # Documentación específica
├── scripts/                     # Scripts de gestión
│   ├── oracle/                  # Scripts de Oracle
│   ├── mongodb/                 # Scripts de MongoDB
│   └── manage.sh               # Script principal
├── docs/                        # Documentación adicional
│   └── arquitectura.md         # Diagramas de arquitectura
├── nginx/                       # Configuración de proxy
│   └── nginx.conf              # Configuración Nginx
├── docker-compose.yml          # Orquestación de servicios
├── .gitignore                  # Archivos ignorados por Git
├── .env.example                # Variables de entorno ejemplo
└── README.md                   # Documentación principal
```

## Ventajas del Monorepo para esta Prueba

### 1. **Facilidad de Evaluación**
- Un solo `git clone` para obtener todo el sistema
- Configuración centralizada en `docker-compose.yml`
- Documentación unificada

### 2. **Demostración de Arquitectura**
- Muestra claramente la separación de responsabilidades
- Facilita la comparación entre tecnologías
- Demuestra principios de microservicios

### 3. **Simplicidad de Despliegue**
- Un solo comando para levantar todo el sistema
- Scripts compartidos para gestión
- Configuración de red unificada

## Consideraciones para Producción

En un entorno de producción real, se podría considerar:

### Repositorios Separados si:
- Equipos diferentes manejan cada servicio
- Ciclos de desarrollo independientes
- Tecnologías muy diferentes
- Escalabilidad de equipos

### Monorepo si:
- Equipo pequeño y cohesivo
- Servicios estrechamente relacionados
- Configuración compartida importante
- Facilidad de mantenimiento

## Comandos de Gestión

```bash
# Clonar el repositorio
git clone <repository-url>
cd facturacion-electronica

# Levantar todo el sistema
docker-compose up -d

# Ejecutar pruebas de todos los servicios
./scripts/manage.sh test

# Ver estado de servicios
docker-compose ps
```

## Contribución al Repositorio

### Estructura de Commits
```
feat(invoices): agregar validación de cliente
fix(client): corregir validación de email
docs: actualizar documentación de API
test(audit): agregar pruebas de integración
```

### Ramas por Funcionalidad
```
feature/nueva-validacion-invoices
bugfix/correccion-client-duplicado
docs/actualizacion-readme
```

## Monitoreo del Repositorio

### Métricas Importantes
- Cobertura de pruebas por servicio
- Tiempo de build de cada servicio
- Calidad del código (linting)
- Documentación actualizada

### Herramientas Recomendadas
- GitHub Actions para CI/CD
- SonarQube para calidad de código
- Docker Hub para imágenes
- Slack/Teams para notificaciones
