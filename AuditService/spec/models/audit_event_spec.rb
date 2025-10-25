require 'rails_helper'

RSpec.describe AuditEvent, type: :model do
  describe 'validations' do
    it 'is valid with valid attributes' do
      audit_event = build(:audit_event)
      expect(audit_event).to be_valid
    end

    it 'is not valid without event_type' do
      audit_event = build(:audit_event, event_type: nil)
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:event_type]).to include("can't be blank")
    end

    it 'is not valid with invalid event_type' do
      audit_event = build(:audit_event, event_type: 'INVALID')
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:event_type]).to include('is not included in the list')
    end

    it 'is not valid without entity' do
      audit_event = build(:audit_event, entity: nil)
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:entity]).to include("can't be blank")
    end

    it 'is not valid without entity_id' do
      audit_event = build(:audit_event, entity_id: nil)
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:entity_id]).to include("can't be blank")
    end

    it 'is not valid with negative entity_id' do
      audit_event = build(:audit_event, entity_id: -1)
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:entity_id]).to include('must be greater than 0')
    end

    it 'is not valid without service' do
      audit_event = build(:audit_event, service: nil)
      expect(audit_event).not_to be_valid
      expect(audit_event.errors[:service]).to include("can't be blank")
    end
  end

  describe 'callbacks' do
    it 'sets timestamp to current time if not provided' do
      audit_event = build(:audit_event, timestamp: nil)
      audit_event.save!
      expect(audit_event.timestamp).to be_present
      expect(audit_event.timestamp).to be_within(1.second).of(Time.current)
    end
  end

  describe 'scopes' do
    let!(:invoice_event) { create(:audit_event, entity: 'Invoice', entity_id: 1) }
    let!(:client_event) { create(:audit_event, entity: 'Client', entity_id: 2) }
    let!(:invoices_service_event) { create(:audit_event, service: 'InvoicesService') }
    let!(:clients_service_event) { create(:audit_event, service: 'ClientsService') }

    it 'filters by entity' do
      events = AuditEvent.by_entity('Invoice', 1)
      expect(events).to include(invoice_event)
      expect(events).not_to include(client_event)
    end

    it 'filters by service' do
      events = AuditEvent.by_service('InvoicesService')
      expect(events).to include(invoices_service_event)
      expect(events).not_to include(clients_service_event)
    end

    it 'orders by timestamp desc' do
      events = AuditEvent.recent
      expect(events.first.timestamp).to be >= events.last.timestamp
    end
  end

  describe 'class methods' do
    it 'registers an event successfully' do
      expect {
        AuditEvent.register_event(
          event_type: 'CREATE',
          entity: 'Invoice',
          entity_id: 1,
          details: 'Invoice created',
          service: 'InvoicesService'
        )
      }.to change(AuditEvent, :count).by(1)
    end

    it 'handles registration errors gracefully' do
      allow(AuditEvent).to receive(:create!).and_raise(StandardError.new('Test error'))
      
      result = AuditEvent.register_event(
        event_type: 'CREATE',
        entity: 'Invoice',
        entity_id: 1,
        details: 'Invoice created',
        service: 'InvoicesService'
      )
      
      expect(result).to be_nil
    end
  end

  describe 'instance methods' do
    let(:audit_event) { create(:audit_event, event_type: 'CREATE', entity: 'Invoice', entity_id: 1, details: 'Invoice created') }

    it 'returns complete description' do
      expect(audit_event.full_description).to eq('CREATE Invoice #1: Invoice created')
    end

    it 'identifies error events' do
      error_event = create(:audit_event, event_type: 'ERROR')
      expect(error_event.is_error?).to be_truthy
      expect(audit_event.is_error?).to be_falsey
    end

    it 'identifies critical operations' do
      critical_event = create(:audit_event, event_type: 'CREATE')
      non_critical_event = create(:audit_event, event_type: 'READ')
      
      expect(critical_event.is_critical_operation?).to be_truthy
      expect(non_critical_event.is_critical_operation?).to be_falsey
    end
  end
end
