class Api::V1::AuditController < ApplicationController
  before_action :set_audit_event, only: [:show]

  def index
    @audit_events = AuditEvent.recent.limit(100)
    
    if params[:service].present?
      @audit_events = @audit_events.by_service(params[:service])
    end
    
    if params[:event_type].present?
      @audit_events = @audit_events.by_event_type(params[:event_type])
    end
    
    if params[:start_date].present? && params[:end_date].present?
      start_date = Time.parse(params[:start_date])
      end_date = Time.parse(params[:end_date])
      @audit_events = @audit_events.by_date_range(start_date, end_date)
    end

    render json: @audit_events, status: :ok
  end

  def show
    render json: @audit_event, status: :ok
  end

  def create
    @audit_event = AuditEvent.new(audit_event_params)
    
    if @audit_event.save
      render json: @audit_event, status: :created
    else
      render json: { errors: @audit_event.errors.full_messages }, status: :unprocessable_entity
    end
  end

  def by_entity
    entity = params[:entity]
    entity_id = params[:entity_id].to_i
    
    @audit_events = AuditEvent.by_entity(entity, entity_id).recent.limit(50)
    
    render json: @audit_events, status: :ok
  end

  def by_service
    service = params[:service]
    @audit_events = AuditEvent.by_service(service).recent.limit(100)
    
    render json: @audit_events, status: :ok
  end

  def by_date_range
    start_date = Time.parse(params[:start_date])
    end_date = Time.parse(params[:end_date])
    
    @audit_events = AuditEvent.by_date_range(start_date, end_date).recent.limit(200)
    
    render json: @audit_events, status: :ok
  rescue ArgumentError
    render json: { error: 'Invalid date format' }, status: :bad_request
  end

  private

  def set_audit_event
    @audit_event = AuditEvent.find(params[:id])
  rescue Mongoid::Errors::DocumentNotFound
    render json: { error: 'Audit event not found' }, status: :not_found
  end

  def audit_event_params
    params.require(:audit_event).permit(
      :event_type, :entity, :entity_id, :details, :service, 
      :user_id, :ip_address, :user_agent, metadata: {}
    )
  end
end
