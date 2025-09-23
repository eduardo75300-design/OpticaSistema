
using System.Data.SqlClient;

namespace OpticaSistema
{
    public partial class Form1 : Form
    {
        private ConexionDB conexionBD;
        public Form1()
        {
            InitializeComponent();
            conexionBD = new ConexionDB();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();
            if(ValidarUsuario(usuario, contrasena))
            {
                MessageBox.Show("Bienvenido Usuario","SISTEMA",MessageBoxButtons.OK,MessageBoxIcon.Information);
                await Task.Delay(2000);

            }
            else
            {
                MessageBox.Show("Usuario o Contraseña incorrecto", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarUsuario(string usuario, string contrasena)
        {
            bool valido = false;
            using (SqlConnection con = conexionBD.Conectar())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM usuarioBD WHERE Dni = @Dni AND Contraseña = @Contraseña";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Dni", usuario);
                        cmd.Parameters.AddWithValue("@Contraseña", contrasena);

                        int count = (int)cmd.ExecuteScalar();
                        valido = count > 0;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
            return valido;

        }
    }
}
