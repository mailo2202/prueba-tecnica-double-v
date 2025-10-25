class CreateClient < ActiveRecord::Migration[7.0]
  def change
    create_table :client, id: :integer do |t|
      t.string :nombre, limit: 255, null: false
      t.string :identificacion, limit: 50, null: false
      t.string :email, limit: 255, null: false
      t.string :direccion, limit: 500, null: false
      t.string :telefono, limit: 20
      t.boolean :activo, default: true, null: false
      t.timestamps
    end

    add_index :client, :identificacion, unique: true, name: 'idx_client_identificacion'
    add_index :client, :email, name: 'idx_client_email'
    add_index :client, :activo, name: 'idx_client_activo'
  end
end
