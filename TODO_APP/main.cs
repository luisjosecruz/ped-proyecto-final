using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TODO_APP
{
    public partial class main : KryptonForm
    {
        private int userId;
        private string nombreUsuario;

        public main(int userId, string nombreUsuario)
        {
            InitializeComponent();
            this.userId = userId;
            this.nombreUsuario = nombreUsuario;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += new EventHandler(Form1_Load); // Agregar el manejador del evento Load
            ConfigurarDataGridView(); // Configurar DataGridView
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblUser.Text = "Bienvenido: " + nombreUsuario;
            CargarTareas();
        }

        private void ConfigurarDataGridView()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoGenerateColumns = false;

            // Asegúrate de que la columna TareaID esté presente
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID sesion", DataPropertyName = "TareaID", Name = "TareaID" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Usuario", DataPropertyName = "NombreUsuario", Name = "NombreUsuario" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Actividad", DataPropertyName = "NombreTarea", Name = "NombreTarea" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "Descripcion", Name = "Descripcion" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha Inicio", DataPropertyName = "FechaInicio", Name = "FechaInicio" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha Fin", DataPropertyName = "FechaFin", Name = "FechaFin" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Categoría", DataPropertyName = "NombreCategoria", Name = "NombreCategoria" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Estado", DataPropertyName = "NombreEstado", Name = "NombreEstado" });

            // Agregar columnas ocultas para CategoriaID y EstadoID
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "CategoriaID", DataPropertyName = "CategoriaID", Name = "CategoriaID", Visible = false });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "EstadoID", DataPropertyName = "EstadoID", Name = "EstadoID", Visible = false });
        }




        public void CargarTareas()
        {
            var conexion = new Conexion(); // Asegúrate de que la clase Conexion esté disponible
            List<Tarea> tareas = conexion.LeerTareas(userId);
            dataGridView1.DataSource = tareas;
        }



        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Check each cell in the row except the last cell for emptiness or null
            for (int columnIndex = 0; columnIndex < row.Cells.Count - 1; columnIndex++)
            {
                DataGridViewCell cell = row.Cells[columnIndex];
                if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                {
                    // Display a message or perform other actions as needed
                    MessageBox.Show("Missing information.");

                    // Cancel the row validation to prevent leaving an empty row
                    e.Cancel = true;
                    break; // Stop checking further cells if one is empty
                }
            }
        }



        private void kryptonPalette1_PalettePaint(object sender, PaletteLayoutEventArgs e)
        {

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kryptonDataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void kryptonDataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DeleteSelectedRow()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    int tareaID = (int)selectedRow.Cells["TareaID"].Value;

                    // Llamar a la función para eliminar la tarea
                    Conexion conexion = new Conexion(); // Crear una instancia de la clase Conexion
                    bool eliminada = conexion.EliminarTarea(tareaID); // Llamar al método EliminarTarea en la instancia de Conexion


                    if (eliminada)
                    {
                        // Volver a cargar las tareas en el DataGridView después de eliminar
                        CargarTareas();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una actividad para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ninguna actividad eliminada \nError text : " + ex.Message);
            }
            finally
            {
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedRow();
        }

        private void kryptonLabel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kryptonSeparator1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }

        private void lblUser_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarTarea_Click(object sender, EventArgs e)
        {
            agregarTarea agregarTarea = new agregarTarea(userId, nombreUsuario);
            agregarTarea.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int tareaID = (int)selectedRow.Cells["TareaID"].Value;
                string nombreTarea = (string)selectedRow.Cells["NombreTarea"].Value;
                string descripcion = (string)selectedRow.Cells["Descripcion"].Value;
                DateTime fechaInicio = (DateTime)selectedRow.Cells["FechaInicio"].Value;
                DateTime fechaFin = (DateTime)selectedRow.Cells["FechaFin"].Value;
                int categoriaID = (int)selectedRow.Cells["CategoriaID"].Value;
                int estadoID = (int)selectedRow.Cells["EstadoID"].Value;

                actualizarTarea actualizarTareaForm = new actualizarTarea(tareaID, userId, nombreUsuario, nombreTarea, descripcion, fechaInicio, fechaFin, categoriaID , estadoID);
                actualizarTareaForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Seleccione una actividad para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteSelectedRow();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register registro = new Register();
            registro.Show();
            this.Hide(); // Ocultar el formulario de inicio de sesión
        }
    }
}
