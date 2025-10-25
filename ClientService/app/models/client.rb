class Client < ApplicationRecord
  # Validaciones
  validates :nombre, presence: true, length: { maximum: 255 }
  validates :identificacion, presence: true, uniqueness: true, length: { maximum: 50 }
  validates :email, presence: true, format: { with: URI::MailTo::EMAIL_REGEXP }, length: { maximum: 255 }
  validates :direccion, presence: true, length: { maximum: 500 }
  validates :telefono, length: { maximum: 20 }, allow_blank: true

  # Callbacks
  before_save :normalize_data
  after_create :register_audit_event
  after_update :register_audit_event
  after_destroy :register_audit_event

  # Scopes
  scope :active, -> { where(activo: true) }
  scope :by_identification, ->(identification) { where(identificacion: identification) }
  scope :by_email, ->(email) { where(email: email) }

  # Instance methods
  def full_name
    "#{nombre} (#{identificacion})"
  end

  def deactivate!
    update!(activo: false)
  end

  def activate!
    update!(activo: true)
  end

  private

  def normalize_data
    self.nombre = nombre.strip.titleize if nombre.present?
    self.email = email.strip.downcase if email.present?
    self.identificacion = identificacion.strip.upcase if identificacion.present?
  end

  def register_audit_event
    AuditService.new.register_event(
      event_type: saved_change_to_id? ? 'CREATE' : (destroyed? ? 'DELETE' : 'UPDATE'),
      entity: 'Client',
      entity_id: id || id_was,
      details: change_details
    )
  rescue => e
    Rails.logger.error "Error registering audit: #{e.message}"
  end

  def change_details
    if saved_change_to_id?
      "Client created: #{full_name}"
    elsif destroyed?
      "Client deleted: #{full_name}"
    else
      changes = saved_changes.except('updated_at')
      "Client updated: #{changes.keys.join(', ')}"
    end
  end
end
