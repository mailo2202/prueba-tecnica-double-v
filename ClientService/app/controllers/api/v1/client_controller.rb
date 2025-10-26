class Api::V1::ClientController < ApplicationController
  before_action :set_cliente, only: [:show, :update, :destroy]

  def index
    @client = Client.activos
    render json: @client, status: :ok
  end

  def show
    render json: @cliente, status: :ok
  end

  def create
    @cliente = Client.new(cliente_params)

    if @cliente.save
      render json: @cliente, status: :created
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

  def update
    if @cliente.update(cliente_params)
      render json: @cliente, status: :ok
    else
      render json: { errors: @cliente.errors.full_messages }, status: :unprocessable_entity
    end
  end

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
