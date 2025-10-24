class AuditService
  include HTTParty
  base_uri ENV.fetch('AUDIT_SERVICE_URL', 'http://localhost:3002')

  def initialize
    @options = {
      headers: {
        'Content-Type' => 'application/json',
        'Accept' => 'application/json'
      },
      timeout: 30
    }
  end

  def register_event(event_type:, entity:, entity_id:, details:)
    event = {
      event_type: event_type,
      entity: entity,
      entity_id: entity_id,
      details: details,
      timestamp: Time.current.iso8601,
      service: 'ClientsService'
    }

    response = self.class.post('/api/v1/audit', @options.merge(body: event.to_json))
    
    unless response.success?
      Rails.logger.error "Error registering audit event: #{response.code} - #{response.body}"
    end

    response.success?
  rescue => e
    Rails.logger.error "Error connecting to audit service: #{e.message}"
    false
  end
end
