FactoryBot.define do
  factory :cliente do
    nombre { Faker::Name.name }
    identificacion { Faker::Number.unique.number(digits: 8).to_s + '-' + Faker::Number.number(digits: 1).to_s }
    email { Faker::Internet.email }
    direccion { Faker::Address.full_address }
    telefono { Faker::PhoneNumber.phone_number }
    activo { true }

    trait :inactivo do
      activo { false }
    end

    trait :con_telefono do
      telefono { Faker::PhoneNumber.phone_number }
    end

    trait :sin_telefono do
      telefono { nil }
    end
  end
end
