class AuditEvent
  include Mongoid::Document
  include Mongoid::Timestamps

  field :event_type, type: String
  field :entity, type: String
  field :entity_id, type: Integer
  field :details, type: String
  field :timestamp, type: Time
  field :service, type: String
  field :user_id, type: Integer
  field :ip_address, type: String
  field :user_agent, type: String
  field :metadata, type: Hash

  index({ entity: 1, entity_id: 1 })
  index({ service: 1 })
  index({ timestamp: 1 })
  index({ event_type: 1 })
  index({ user_id: 1 })

  index({ service: 1, timestamp: -1 })

  validates :event_type, presence: true, inclusion: { in: %w[CREATE UPDATE DELETE READ ERROR] }
  validates :entity, presence: true
  validates :entity_id, presence: true, numericality: { greater_than: 0 }
  validates :service, presence: true
  validates :timestamp, presence: true

  scope :by_entity, ->(entity, entity_id) { where(entity: entity, entity_id: entity_id) }
  scope :by_service, ->(service) { where(service: service) }
  scope :by_date_range, ->(start_date, end_date) { where(timestamp: start_date..end_date) }
  scope :by_event_type, ->(event_type) { where(event_type: event_type) }
  scope :recent, -> { order(timestamp: :desc) }

  before_save :set_default_timestamp

  def self.register_event(event_type:, entity:, entity_id:, details:, service:, **options)
    create!(
      event_type: event_type,
      entity: entity,
      entity_id: entity_id,
      details: details,
      service: service,
      timestamp: Time.current,
      user_id: options[:user_id],
      ip_address: options[:ip_address],
      user_agent: options[:user_agent],
      metadata: options[:metadata]
    )
  rescue => e
    Rails.logger.error "Error registering audit event: #{e.message}"
    nil
  end

  # Instance methods
  def full_description
    "#{event_type} #{entity} ##{entity_id}: #{details}"
  end

  def is_error?
    event_type == 'ERROR'
  end

  def is_critical_operation?
    %w[CREATE DELETE ERROR].include?(event_type)
  end

  private

  def set_default_timestamp
    self.timestamp ||= Time.current
  end
end
