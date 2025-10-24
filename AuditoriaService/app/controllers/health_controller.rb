class HealthController < ApplicationController
  def show
    render json: {
      status: 'healthy',
      service: 'AuditService',
      timestamp: Time.current,
      version: '1.0.0',
      database: database_status,
      total_events: total_events_count
    }, status: :ok
  end

  private

  def database_status
    AuditEvent.count
    'connected'
  rescue => e
    Rails.logger.error "Database health check failed: #{e.message}"
    'disconnected'
  end

  def total_events_count
    AuditEvent.count
  rescue
    0
  end
end
