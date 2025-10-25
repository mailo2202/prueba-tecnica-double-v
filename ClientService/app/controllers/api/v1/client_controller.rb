class Api::V1::ClientController < ApplicationController
  before_action :set_cliente, only: [:show, :update, :destroy]

  # GET /api/v1/client
  def index
    @client = Client.activos
    render json: @client, status: :ok
  end

  # GET /api/v1/client/:id
  def show
    render json: @cliente, status: :ok
  end

  # POST /api/v1/client
  def create
    @cliente = Client.new(cliente_params)

    if @cliente.save
      render json: @cliente, status: :created
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

  # PATCH/PUT /api/v1/client/:id
  def update
    if @cliente.update(cliente_params)
      render json: @cliente, status: :ok
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

  # DELETE /api/v1/client/:id
  def destroy
    @cliente.destroy
    head :no_content
  end

  private

  def set_cliente
    @cliente = Client.find(params[:id])
  rescue ActiveRecord::RecordNotFound
    render json: { error: 'Client no encontrado' }, status: :not_found
  end

  def cliente_params
    params.require(:cliente).permit(:nombre, :identificacion, :email, :direccion, :telefono)
  end
end
