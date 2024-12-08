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
    public partial class actualizarTarea : Form
    {
        private int userId;
        private int tareaID;
        private string nombreUsuario;
        private Conexion conexion;


        public actualizarTarea(int tareaID, int userId, string nombreUsuario, string nombreTarea, string descripcion, DateTime fechaInicio, DateTime fechaFin, int categoriaID, int estadoID)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.tareaID = tareaID;
            this.userId = userId;
            this.nombreUsuario = nombreUsuario;
            conexion = new Conexion();

            CargarCategorias(categoriaID);
            CargarEstados(estadoID);

            textBox1.Text = nombreTarea;
            textBox2.Text = descripcion;
            dateTimePicker3.Value = fechaInicio;
            dateTimePicker4.Value = fechaFin;
        }

        private void CargarCategorias(int selectedCategoriaID)
        {
            var categorias = conexion.ObtenerCategorias();
            comboBox3.DataSource = categorias;
            comboBox3.DisplayMember = "NombreCategoria";
            comboBox3.ValueMember = "CategoriaID";
            comboBox3.SelectedValue = selectedCategoriaID;
        }

        private void CargarEstados(int selectedEstadoID)
        {
            var estados = conexion.ObtenerEstados();
            comboBox4.DataSource = estados;
            comboBox4.DisplayMember = "NombreEstado";
            comboBox4.ValueMember = "EstadoID";
            comboBox4.SelectedValue = selectedEstadoID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            main mainForm = new main(userId, nombreUsuario);
            mainForm.Show();
            this.Hide(); // Ocultar el formulario de inicio de sesión
        }

        private void actualizarTarea_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nombreTarea = textBox1.Text;
            string descripcion = textBox2.Text;
            DateTime fechaInicio = dateTimePicker3.Value;
            DateTime fechaFin = dateTimePicker4.Value;
            int categoriaID = (int)comboBox3.SelectedValue;
            int estadoID = (int)comboBox4.SelectedValue;

            if (conexion.ActualizarTarea(tareaID, nombreTarea, descripcion, fechaInicio, fechaFin, categoriaID, estadoID))
            {
                MessageBox.Show("Tarea actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar DataGridView en el formulario principal
                main mainForm = new main(userId, nombreUsuario);
                mainForm.Show();
                this.Hide(); // Ocultar el formulario de inicio de sesión
            }
            else
            {
                MessageBox.Show("Error al actualizar la tarea.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
