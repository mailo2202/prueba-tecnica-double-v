require 'rails_helper'

RSpec.describe Api::V1::ClientesController, type: :controller do
  let(:valid_attributes) do
    {
      nombre: 'Juan Perez',
      identificacion: '12345678-9',
      email: 'juan@example.com',
      direccion: 'Calle 123 #45-67, Bogotá',
      telefono: '3001234567'
    }
  end

  let(:invalid_attributes) do
    {
      nombre: nil,
      identificacion: nil,
      email: 'invalid-email',
      direccion: nil
    }
  end

  describe 'GET #index' do
    it 'returns a successful response' do
      get :index
      expect(response).to have_http_status(:ok)
    end

    it 'returns all active clients' do
      cliente1 = create(:cliente, activo: true)
      cliente2 = create(:cliente, activo: true)
      create(:cliente, activo: false)

      get :index
      expect(JSON.parse(response.body).length).to eq(2)
    end
  end

  describe 'GET #show' do
    let(:cliente) { create(:cliente) }

    it 'returns a successful response' do
      get :show, params: { id: cliente.id }
      expect(response).to have_http_status(:ok)
    end

    it 'returns the correct client' do
      get :show, params: { id: cliente.id }
      expect(JSON.parse(response.body)['id']).to eq(cliente.id)
    end

    it 'returns 404 for non-existent client' do
      get :show, params: { id: 99999 }
      expect(response).to have_http_status(:not_found)
    end
  end

  describe 'POST #create' do
    context 'with valid parameters' do
      it 'creates a new client' do
        expect {
          post :create, params: { cliente: valid_attributes }
        }.to change(Cliente, :count).by(1)
      end

      it 'returns a created response' do
        post :create, params: { cliente: valid_attributes }
        expect(response).to have_http_status(:created)
      end
    end

    context 'with invalid parameters' do
      it 'does not create a new client' do
        expect {
          post :create, params: { cliente: invalid_attributes }
        }.not_to change(Cliente, :count)
      end

      it 'returns an unprocessable entity response' do
        post :create, params: { cliente: invalid_attributes }
        expect(response).to have_http_status(:unprocessable_entity)
      end
    end
  end

  describe 'PATCH #update' do
    let(:cliente) { create(:cliente) }

    context 'with valid parameters' do
      let(:new_attributes) do
        {
          nombre: 'Juan Carlos Perez',
          email: 'juancarlos@example.com'
        }
      end

      it 'updates the requested client' do
        patch :update, params: { id: cliente.id, cliente: new_attributes }
        cliente.reload
        expect(cliente.nombre).to eq('Juan Carlos Perez')
        expect(cliente.email).to eq('juancarlos@example.com')
      end

      it 'returns a successful response' do
        patch :update, params: { id: cliente.id, cliente: new_attributes }
        expect(response).to have_http_status(:ok)
      end
    end

    context 'with invalid parameters' do
      it 'returns an unprocessable entity response' do
        patch :update, params: { id: cliente.id, cliente: invalid_attributes }
        expect(response).to have_http_status(:unprocessable_entity)
      end
    end
  end

  describe 'DELETE #destroy' do
    let!(:cliente) { create(:cliente) }

    it 'destroys the requested client' do
      expect {
        delete :destroy, params: { id: cliente.id }
      }.to change(Cliente, :count).by(-1)
    end

    it 'returns no content response' do
      delete :destroy, params: { id: cliente.id }
      expect(response).to have_http_status(:no_content)
    end
  end
end
