using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TODO_APP
{
    public class Conexion
    {
        
        private string connectionString = "Data Source=LAPTOP-FJ1855I0\\SQLEXPRESS; database=UDBTareas; Integrated Security=true";

        //Funcion que valida las credenciales del usuario
        public Tuple<bool, int, string> ValidateCredentials(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UsuarioID, NombreUsuario FROM Usuarios WHERE NombreUsuario = @Username AND Contraseña = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Agregar parámetros para prevenir inyección SQL
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string nombreUsuario = reader.GetString(1);

                            //Usuario encontrado en la base de datos
                            return new Tuple<bool, int, string>(true, userId, nombreUsuario);
                        }
                        else
                        {
                            //Usuario no encontrado en la base de datos
                            return new Tuple<bool, int, string>(false, -1, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al validar credenciales: " + ex.Message);
                        return new Tuple<bool, int, string>(false, -1, null);
                    }
                }
            }
        }


        //Función para crear nuevos usuarios.
        public Tuple<bool, int, string> CrearUsuario(string nombreUsuario, string contraseña)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Verificar si el usuario ya existe
                    string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @NombreUsuario";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        int userCount = (int)checkCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("El Usuario ya existe, puedes probar con otro nombre de usuario", "Error de registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return new Tuple<bool, int, string>(false, -1, null);
                        }
                    }

                    // Insertar nuevo usuario
                    string insertQuery = "INSERT INTO Usuarios (NombreUsuario, Contraseña) VALUES (@NombreUsuario, @Contraseña); SELECT SCOPE_IDENTITY()";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        insertCommand.Parameters.AddWithValue("@Contraseña", contraseña);

                        // Obtener el ID del usuario insertado
                        int userId = Convert.ToInt32(insertCommand.ExecuteScalar());

                        return new Tuple<bool, int, string>(true, userId, nombreUsuario);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al crear usuario: " + ex.Message);
                    return new Tuple<bool, int, string>(false, -1, null);
                }
            }
        }


        // Método para crear una nueva tarea
        public bool CrearTarea(int usuarioID, string nombreTarea, string descripcion, DateTime fechaInicio, DateTime fechaFin, int categoriaID, int estadoID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tareas (UsuarioID, NombreTarea, Descripcion, FechaInicio, FechaFin, CategoriaID, EstadoID) " +
                               "VALUES (@UsuarioID, @NombreTarea, @Descripcion, @FechaInicio, @FechaFin, @CategoriaID, @EstadoID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    command.Parameters.AddWithValue("@NombreTarea", nombreTarea);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.Parameters.AddWithValue("@CategoriaID", categoriaID);
                    command.Parameters.AddWithValue("@EstadoID", estadoID);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        // Método para leer todas las tareas
        public List<Tarea> LeerTareas(int userId)
        {
            List<Tarea> tareas = new List<Tarea>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT T.TareaID, U.NombreUsuario, T.NombreTarea, T.Descripcion, T.FechaInicio, T.FechaFin, 
                           C.CategoriaID, C.NombreCategoria, E.EstadoID, E.NombreEstado 
                    FROM Tareas T
                    JOIN Usuarios U ON T.UsuarioID = U.UsuarioID
                    JOIN Categorias C ON T.CategoriaID = C.CategoriaID
                    JOIN Estados E ON T.EstadoID = E.EstadoID
                    WHERE U.UsuarioID = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tarea tarea = new Tarea
                            {
                                TareaID = reader.GetInt32(0),
                                NombreUsuario = reader.GetString(1),
                                NombreTarea = reader.GetString(2),
                                Descripcion = reader.GetString(3),
                                FechaInicio = reader.GetDateTime(4),
                                FechaFin = reader.GetDateTime(5),
                                CategoriaID = reader.GetInt32(6), // Obtener el ID de la categoría
                                NombreCategoria = reader.GetString(7),
                                EstadoID = reader.GetInt32(8), // Obtener el ID del estado
                                NombreEstado = reader.GetString(9)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
            }
            return tareas;
        }



        // Método para eliminar una tarea por su ID
        public bool EliminarTarea(int tareaID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Tareas WHERE TareaID = @TareaID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TareaID", tareaID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verificar si se eliminó la tarea correctamente
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al eliminar la tarea: " + ex.Message);
                        return false;
                    }
                }
            }
        }


        // Método para actualizar una tarea
        public bool ActualizarTarea(int tareaID, string nombreTarea, string descripcion, DateTime fechaInicio, DateTime fechaFin, int categoriaID, int estadoID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Tareas SET NombreTarea = @NombreTarea, Descripcion = @Descripcion, " +
                               "FechaInicio = @FechaInicio, FechaFin = @FechaFin, CategoriaID = @CategoriaID, EstadoID = @EstadoID " +
                               "WHERE TareaID = @TareaID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TareaID", tareaID);
                    command.Parameters.AddWithValue("@NombreTarea", nombreTarea);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.Parameters.AddWithValue("@CategoriaID", categoriaID);
                    command.Parameters.AddWithValue("@EstadoID", estadoID);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true; // Éxito al actualizar la tarea
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al actualizar tarea: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoriaID, NombreCategoria FROM Categorias";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Categoria categoria = new Categoria
                            {
                                CategoriaID = reader.GetInt32(0),
                                NombreCategoria = reader.GetString(1)
                            };
                            categorias.Add(categoria);
                        }
                    }
                }
            }
            return categorias;
        }

        public List<Estado> ObtenerEstados()
        {
            List<Estado> estados = new List<Estado>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EstadoID, NombreEstado FROM Estados";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Estado estado = new Estado
                            {
                                EstadoID = reader.GetInt32(0),
                                NombreEstado = reader.GetString(1)
                            };
                            estados.Add(estado);
                        }
                    }
                }
            }
            return estados;
        }

    }

    public class Tarea
    {
        public int TareaID { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreTarea { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreEstado { get; set; }
        public int CategoriaID { get; internal set; }
        public int EstadoID { get; internal set; }
    }

    public class Categoria
    {
        public int CategoriaID { get; set; }
        public string NombreCategoria { get; set; }
    }

    public class Estado
    {
        public int EstadoID { get; set; }
        public string NombreEstado { get; set; }
    }

}
