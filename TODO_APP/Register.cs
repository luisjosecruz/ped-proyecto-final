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
    public partial class Register : Form
    {

        private int userId;
        private string nombreUsuario;
        private Conexion dataAccess = new Conexion();
        public Register()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            txtPassword.UseSystemPasswordChar = true; // Enmascara la contraseña con asteriscos
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            main mainForm = new main(userId, nombreUsuario);
            mainForm.Show();
            this.Hide(); // Ocultar el formulario de inicio de sesión
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingresa un nombre de usuario y una contraseña.", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Tuple<bool, int, string> result = dataAccess.CrearUsuario(username, password);

            if (result.Item1)
            {
                MessageBox.Show("¡Usuario Creado Exitosamente!", "¡Registro Exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                int userId = result.Item2;
                string nombreUsuario = result.Item3;
                main mainForm = new main(userId, nombreUsuario);
                mainForm.Show();
                this.Hide(); // Ocultar el formulario de inicio de sesión
            }
            else
            {
                MessageBox.Show("No se pudo crear el usuario, contactar al desarrollador.", "Error de registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
