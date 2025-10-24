class HealthController < ApplicationController
  def show
    render json: {
      status: 'healthy',
      service: 'ClientesService',
      timestamp: Time.current,
      version: '1.0.0',
      database: database_status
    }, status: :ok
  end

  private

  def database_status
    Cliente.count
    'connected'
  rescue => e
    Rails.logger.error "Database health check failed: #{e.message}"
    'disconnected'
  end
end
