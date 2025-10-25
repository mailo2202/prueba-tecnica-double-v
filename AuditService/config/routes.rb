Rails.application.routes.draw do
  # Reveal health status on /up that returns 200 if the app boots with no exceptions, otherwise 500.
  # Can be used by uptime monitors and load balancers.
  get "up" => "rails/health#show", as: :rails_health_check

  # API routes
  namespace :api do
    namespace :v1 do
      resources :audit, only: [:index, :show, :create]
      get 'audit/entity/:entity/:entity_id', to: 'audit#by_entity', as: 'audit_by_entity'
      get 'audit/service/:service', to: 'audit#by_service', as: 'audit_by_service'
      get 'audit/date/:start_date/:end_date', to: 'audit#by_date_range', as: 'audit_by_date_range'
    end
  end

  # Health check endpoint
  get '/health', to: 'health#show'
  
  # Root endpoint
  root 'health#show'
end
