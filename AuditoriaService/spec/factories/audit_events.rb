FactoryBot.define do
  factory :audit_event do
    event_type { %w[CREATE UPDATE DELETE READ ERROR].sample }
    entity { %w[Invoice Client User].sample }
    entity_id { Faker::Number.between(from: 1, to: 1000) }
    details { Faker::Lorem.sentence }
    service { %w[InvoicesService ClientsService AuditService].sample }
    timestamp { Faker::Time.between(from: 1.month.ago, to: Time.current) }
    user_id { Faker::Number.between(from: 1, to: 100) }
    ip_address { Faker::Internet.ip_v4_address }
    user_agent { Faker::Internet.user_agent }
    metadata { { 'version' => '1.0', 'source' => 'api' } }

    trait :create do
      event_type { 'CREATE' }
    end

    trait :update do
      event_type { 'UPDATE' }
    end

    trait :delete do
      event_type { 'DELETE' }
    end

    trait :read do
      event_type { 'READ' }
    end

    trait :error do
      event_type { 'ERROR' }
    end

    trait :invoice do
      entity { 'Invoice' }
    end

    trait :client do
      entity { 'Client' }
    end

    trait :invoices_service do
      service { 'InvoicesService' }
    end

    trait :clients_service do
      service { 'ClientsService' }
    end

    trait :audit_service do
      service { 'AuditService' }
    end
  end
end
