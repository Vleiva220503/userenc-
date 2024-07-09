using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace LoginPermisos
{

    public class Conexion
    {
        private readonly string connectionString;

        public Conexion()
        {
            connectionString = "Data Source=DESKTOP-N7S798M\\MSSQLSERVER01;Initial Catalog=prueba;Integrated Security=True;Encrypt=False";
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        private void OpenConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        private void CloseConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public void AgregarUsuario(string usuario, string pnombre, string snombre, string apellido, string sapellido, string correo, byte[] password, string privilegios, byte[] foto, string numeroTelefono, string telefonoEmergencia)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("agregar_usuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@p_usuario", usuario);
                        command.Parameters.AddWithValue("@p_pnombre", pnombre);
                        command.Parameters.AddWithValue("@p_snombre", (object)snombre ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_apellido", apellido);
                        command.Parameters.AddWithValue("@p_sapellido", (object)sapellido ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_correo", correo);
                        command.Parameters.AddWithValue("@p_password", password);
                        command.Parameters.AddWithValue("@p_privilegios", privilegios);
                        command.Parameters.AddWithValue("@p_foto", foto);
                        command.Parameters.AddWithValue("@p_numero_telefono", numeroTelefono);
                        command.Parameters.AddWithValue("@p_telefono_emergencia", telefonoEmergencia);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al agregar el usuario: " + ex.Message);
                }
            }
        }

        public DataTable MostrarTodosLosUsuarios()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("mostrar_usuarios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al mostrar los usuarios: " + ex.Message);
                }
            }

            return dataTable;
        }

        public void EditarUsuario(int id, string usuario, string pnombre, string snombre, string apellido, string sapellido, string correo, byte[] password, string privilegios, byte[] foto, string numeroTelefono, string telefonoEmergencia)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("editar_usuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@p_id", id);
                        command.Parameters.AddWithValue("@p_usuario", usuario);
                        command.Parameters.AddWithValue("@p_pnombre", pnombre);
                        command.Parameters.AddWithValue("@p_snombre", (object)snombre ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_apellido", apellido);
                        command.Parameters.AddWithValue("@p_sapellido", (object)sapellido ?? DBNull.Value);
                        command.Parameters.AddWithValue("@p_correo", correo);
                        command.Parameters.AddWithValue("@p_password", (object)password ?? DBNull.Value); // Opcional
                        command.Parameters.AddWithValue("@p_privilegios", privilegios);
                        command.Parameters.AddWithValue("@p_foto", (object)foto ?? DBNull.Value); // Opcional
                        command.Parameters.AddWithValue("@p_numero_telefono", numeroTelefono);
                        command.Parameters.AddWithValue("@p_telefono_emergencia", telefonoEmergencia);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al editar el usuario: " + ex.Message);
                }
            }
        }

    }
}
