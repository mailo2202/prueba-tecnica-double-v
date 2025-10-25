require 'rails_helper'

RSpec.describe Client, type: :model do
  describe 'validations' do
    it 'is valid with valid attributes' do
      cliente = build(:client)
      expect(cliente).to be_valid
    end

    it 'is not valid without a nombre' do
      cliente = build(:cliente, nombre: nil)
      expect(cliente).not_to be_valid
      expect(cliente.errors[:nombre]).to include("can't be blank")
    end

    it 'is not valid without an identificacion' do
      cliente = build(:cliente, identificacion: nil)
      expect(cliente).not_to be_valid
      expect(cliente.errors[:identificacion]).to include("can't be blank")
    end

    it 'is not valid without an email' do
      cliente = build(:cliente, email: nil)
      expect(cliente).not_to be_valid
      expect(cliente.errors[:email]).to include("can't be blank")
    end

    it 'is not valid with invalid email format' do
      cliente = build(:cliente, email: 'invalid-email')
      expect(cliente).not_to be_valid
      expect(cliente.errors[:email]).to include('is invalid')
    end

    it 'is not valid without a direccion' do
      cliente = build(:cliente, direccion: nil)
      expect(cliente).not_to be_valid
      expect(cliente.errors[:direccion]).to include("can't be blank")
    end

    it 'is not valid with duplicate identificacion' do
      create(:client, identificacion: '12345678-9')
      cliente = build(:cliente, identificacion: '12345678-9')
      expect(cliente).not_to be_valid
      expect(cliente.errors[:identificacion]).to include('has already been taken')
    end
  end

  describe 'callbacks' do
    it 'normalizes data before save' do
      cliente = create(:client, 
        nombre: '  juan perez  ',
        email: '  JUAN@EXAMPLE.COM  ',
        identificacion: '  12345678-9  '
      )
      
      expect(cliente.nombre).to eq('Juan Perez')
      expect(cliente.email).to eq('juan@example.com')
      expect(cliente.identificacion).to eq('12345678-9')
    end
  end

  describe 'scopes' do
    let!(:cliente_activo) { create(:client, activo: true) }
    let!(:cliente_inactivo) { create(:client, activo: false) }

    it 'returns only active clients' do
      expect(Client.activos).to include(cliente_activo)
      expect(Client.activos).not_to include(cliente_inactivo)
    end
  end

  describe 'methods' do
    let(:cliente) { create(:client, nombre: 'Juan Perez', identificacion: '12345678-9') }

    it 'returns full name with identification' do
      expect(cliente.nombre_completo).to eq('Juan Perez (12345678-9)')
    end

    it 'deactivates client' do
      cliente.desactivar!
      expect(cliente.reload.activo).to be_falsey
    end

    it 'activates client' do
      cliente.update!(activo: false)
      cliente.activar!
      expect(cliente.reload.activo).to be_truthy
    end
  end
end
