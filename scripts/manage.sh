#!/bin/bash

# Script de inicialización y configuración del sistema de facturación electrónica
# FactuMarket S.A.

echo "=========================================="
echo "Sistema de Facturación Electrónica"
echo "FactuMarket S.A."
echo "=========================================="

# Función para mostrar el estado de los servicios
show_status() {
    echo "Estado de los servicios:"
    docker-compose ps
}

# Función para inicializar el sistema
init_system() {
    echo "Inicializando sistema..."
    
    # Crear directorios necesarios
    mkdir -p logs
    mkdir -p data/oracle
    mkdir -p data/mongodb
    
    # Construir imágenes Docker
    echo "Construyendo imágenes Docker..."
    docker-compose build
    
    # Levantar servicios
    echo "Levantando servicios..."
    docker-compose up -d
    
    # Esperar a que los servicios estén listos
    echo "Esperando a que los servicios estén listos..."
    sleep 30
    
    # Verificar estado
    show_status
}

# Función para detener el sistema
stop_system() {
    echo "Deteniendo sistema..."
    docker-compose down
}

# Función para reiniciar el sistema
restart_system() {
    echo "Reiniciando sistema..."
    stop_system
    sleep 5
    init_system
}

# Función para mostrar logs
show_logs() {
    if [ -z "$1" ]; then
        echo "Mostrando logs de todos los servicios..."
        docker-compose logs -f
    else
        echo "Mostrando logs del servicio: $1"
        docker-compose logs -f "$1"
    fi
}

# Función para ejecutar pruebas
run_tests() {
    echo "Ejecutando pruebas..."
    
    # Pruebas del servicio de Facturas (.NET)
    echo "Ejecutando pruebas del servicio de Facturas..."
    docker-compose exec invoices-service dotnet test
    
    # Pruebas del servicio de Client (Ruby)
    echo "Ejecutando pruebas del servicio de Client..."
    docker-compose exec client-service bundle exec rspec
    
    # Pruebas del servicio de Auditoría (Ruby)
    echo "Ejecutando pruebas del servicio de Auditoría..."
    docker-compose exec audit-service bundle exec rspec
}

# Función para mostrar ayuda
show_help() {
    echo "Uso: $0 [comando]"
    echo ""
    echo "Comandos disponibles:"
    echo "  init      - Inicializar el sistema completo"
    echo "  start     - Iniciar servicios"
    echo "  stop      - Detener servicios"
    echo "  restart   - Reiniciar servicios"
    echo "  status    - Mostrar estado de servicios"
    echo "  logs      - Mostrar logs (opcional: nombre del servicio)"
    echo "  test      - Ejecutar todas las pruebas"
    echo "  clean     - Limpiar contenedores y volúmenes"
    echo "  help      - Mostrar esta ayuda"
    echo ""
    echo "Ejemplos:"
    echo "  $0 init"
    echo "  $0 logs invoices-service"
    echo "  $0 test"
}

# Función para limpiar el sistema
clean_system() {
    echo "Limpiando sistema..."
    docker-compose down -v
    docker system prune -f
    docker volume prune -f
}

# Función para iniciar servicios
start_services() {
    echo "Iniciando servicios..."
    docker-compose up -d
    show_status
}

# Procesar argumentos de línea de comandos
case "$1" in
    init)
        init_system
        ;;
    start)
        start_services
        ;;
    stop)
        stop_system
        ;;
    restart)
        restart_system
        ;;
    status)
        show_status
        ;;
    logs)
        show_logs "$2"
        ;;
    test)
        run_tests
        ;;
    clean)
        clean_system
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        echo "Comando no reconocido: $1"
        show_help
        exit 1
        ;;
esac
