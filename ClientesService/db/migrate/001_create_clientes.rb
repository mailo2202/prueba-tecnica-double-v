class CreateClientes < ActiveRecord::Migration[7.0]
  def change
    create_table :clientes, id: :integer do |t|
      t.string :nombre, limit: 255, null: false
      t.string :identificacion, limit: 50, null: false
      t.string :email, limit: 255, null: false
      t.string :direccion, limit: 500, null: false
      t.string :telefono, limit: 20
      t.boolean :activo, default: true, null: false
      t.timestamps
    end

    add_index :clientes, :identificacion, unique: true, name: 'idx_clientes_identificacion'
    add_index :clientes, :email, name: 'idx_clientes_email'
    add_index :clientes, :activo, name: 'idx_clientes_activo'
  end
end
