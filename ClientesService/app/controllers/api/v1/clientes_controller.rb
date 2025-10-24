class Api::V1::ClientesController < ApplicationController
  before_action :set_cliente, only: [:show, :update, :destroy]

  # GET /api/v1/clientes
  def index
    @clientes = Cliente.activos
    render json: @clientes, status: :ok
  end

  # GET /api/v1/clientes/:id
  def show
    render json: @cliente, status: :ok
  end

  # POST /api/v1/clientes
  def create
    @cliente = Cliente.new(cliente_params)

    if @cliente.save
      render json: @cliente, status: :created
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

  # PATCH/PUT /api/v1/clientes/:id
  def update
    if @cliente.update(cliente_params)
      render json: @cliente, status: :ok
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

  # DELETE /api/v1/clientes/:id
  def destroy
    @cliente.destroy
    head :no_content
  end

  private

  def set_cliente
    @cliente = Cliente.find(params[:id])
  rescue ActiveRecord::RecordNotFound
    render json: { error: 'Cliente no encontrado' }, status: :not_found
  end

  def cliente_params
    params.require(:cliente).permit(:nombre, :identificacion, :email, :direccion, :telefono)
  end
end
