using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TODO_APP
{
    public partial class agregarTarea : Form
    {
        private int userId;
        private string nombreUsuario;
        private Conexion conexion;

        public agregarTarea(int userId, string nombreUsuario)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.userId = userId;
            this.nombreUsuario = nombreUsuario;
            conexion = new Conexion();
            CargarCategorias();
            CargarEstados();
        }

        private void CargarCategorias()
        {
            var categorias = conexion.ObtenerCategorias();
            comboBox3.DataSource = categorias;
            comboBox3.DisplayMember = "NombreCategoria";
            comboBox3.ValueMember = "CategoriaID";
        }

        private void CargarEstados()
        {
            var estados = conexion.ObtenerEstados();
            comboBox4.DataSource = estados;
            comboBox4.DisplayMember = "NombreEstado";
            comboBox4.ValueMember = "EstadoID";
        }

        private void btnAgregarTarea_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            main mainForm = new main(userId, nombreUsuario);
            mainForm.Show();
            this.Hide(); // Ocultar el formulario de inicio de sesión
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nombreTarea = textBox1.Text;
            string descripcion = textBox2.Text;
            DateTime fechaInicio = dateTimePicker3.Value;
            DateTime fechaFin = dateTimePicker4.Value;
            int categoriaID = (int)comboBox3.SelectedValue;
            int estadoID = (int)comboBox4.SelectedValue;

            if (conexion.CrearTarea(userId, nombreTarea, descripcion, fechaInicio, fechaFin, categoriaID, estadoID))
            {
                MessageBox.Show("Tarea agregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar DataGridView en el formulario principal
                main mainForm = new main(userId, nombreUsuario);
                mainForm.Show();
                this.Hide(); // Ocultar el formulario de inicio de sesión
            }
            else
            {
                MessageBox.Show("Error al agregar la tarea.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void agregarTarea_Load(object sender, EventArgs e)
        {

        }
    }


}
