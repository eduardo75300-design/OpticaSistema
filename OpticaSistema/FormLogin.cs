
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace OpticaSistema
{
    public partial class FormLogin : Form
    {
        private ConexionDB conexionBD;
        public static class SesionUsuario
        {
            public static string Nombre { get; set; }
            public static string TipoUsuario { get; set; }

        }
        public FormLogin()
        {
            InitializeComponent();
            this.FormClosing += Form1_Load;
            conexionBD = new ConexionDB();

            // Estilo del formulario fijo
            this.Size = new Size(850, 450);
            this.MinimumSize = new Size(850, 450);
            this.MaximumSize = new Size(850, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Imagen lateral
            string rutaImagen = "Imagenes/log.png";
            if (File.Exists(rutaImagen))
            {
                PictureBox imagen = new PictureBox();
                imagen.Image = Image.FromFile(rutaImagen);
                imagen.SizeMode = PictureBoxSizeMode.StretchImage;
                imagen.Size = new Size(300, 300);
                imagen.Location = new Point(60, 40); // posición fija dentro del formulario
                this.Controls.Add(imagen);
            }
            else
            {
                MessageBox.Show("No se encontró la imagen en:\n" + rutaImagen, "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Posicionamiento fijo dentro del formulario de 850x500
            int anchoCampo = 300;
            int posicionX = 470; // parte derecha del formulario
            int posicionY = 100;

            // Título LOGIN
            lblLogin.Text = "LOGIN";
            lblLogin.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblLogin.ForeColor = Color.DarkRed;
            lblLogin.TextAlign = ContentAlignment.MiddleCenter;
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(posicionX + (anchoCampo - lblLogin.Width) / 2, posicionY - 70);

            // Etiqueta Usuario
            lblUsuario.Text = "Usuario:";
            lblUsuario.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblUsuario.ForeColor = Color.Black;
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(posicionX, posicionY - 3);

            // Campo Usuario
            txtUsuario.Font = new Font("Segoe UI", 12);
            txtUsuario.BackColor = Color.White;
            txtUsuario.ForeColor = Color.Black;
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.Size = new Size(anchoCampo, 35);
            txtUsuario.Location = new Point(posicionX, posicionY + 25);

            // Etiqueta Contraseña
            lblContraseña.Text = "Contraseña:";
            lblContraseña.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblContraseña.ForeColor = Color.Black;
            lblContraseña.AutoSize = true;
            lblContraseña.Location = new Point(posicionX, posicionY + 78);

            // Campo Contraseña
            txtContrasena.Font = new Font("Segoe UI", 12);
            txtContrasena.BackColor = Color.White;
            txtContrasena.ForeColor = Color.Black;
            txtContrasena.BorderStyle = BorderStyle.FixedSingle;
            txtContrasena.UseSystemPasswordChar = true;
            txtContrasena.Size = new Size(anchoCampo, 35);
            txtContrasena.Location = new Point(posicionX, posicionY + 105);

            // Botón Ingresar
            btnIngresar.Text = "INGRESAR";
            btnIngresar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnIngresar.BackColor = Color.DarkRed;
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Cursor = Cursors.Hand;
            btnIngresar.Size = new Size(anchoCampo, 40);
            btnIngresar.Location = new Point(posicionX, posicionY + 180);
            btnIngresar.MouseEnter += (s, e) => btnIngresar.BackColor = Color.Firebrick;
            btnIngresar.MouseLeave += (s, e) => btnIngresar.BackColor = Color.DarkRed;

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Inicio de sesión";
            this.Icon = new Icon("Imagenes/log.ico");
            ActualizarEdades();

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

            if (ValidarUsuario(usuario, contrasena))
            {
                CargarDatosUsuario(usuario);
                Sesion.UsuarioDni = usuario;

                Bienvenido bienvenida = new Bienvenido();
                bienvenida.ShowDialog();

                FormInicio inicioForm = new FormInicio();

                // Cuando se cierre el menú, cerrar también el login
                inicioForm.FormClosed += (s, args) => this.Close();

                this.Hide(); // Oculta el login sin cerrarlo
                inicioForm.Show(); // Abre el menú principal
            }
            else
            {
                MessageBox.Show("Usuario o Contraseña incorrecto", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }







        }

        public class AppContext : ApplicationContext
        {
            public AppContext(Form initialForm)
            {
                initialForm.FormClosed += (s, e) =>
                {
                    // Cuando se cierra el formulario actual, termina la aplicación
                    ExitThread();
                };

                initialForm.Show();
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblContraseña_Click(object sender, EventArgs e)
        {

        }

        private void CargarDatosUsuario(string dni)
        {
            string nombre = "";
            string tipoUsuario = "";

            using (SqlConnection con = conexionBD.Conectar())
            {
                try
                {
                    con.Open();
                    string query = "SELECT Nombres, TipoUsuario FROM UsuarioBD WHERE Dni = @Dni";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Dni", dni);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            SesionUsuario.Nombre = reader["Nombres"].ToString();
                            SesionUsuario.TipoUsuario = reader["TipoUsuario"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener datos del usuario: " + ex.Message);
                }

            }

        }


        public static class Sesion
        {
            public static string UsuarioDni;
        }

        private void ActualizarEdades()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                try
                {
                    cn.Open();
                    string query = @"
                UPDATE PacienteBD
                SET Edad = DATEDIFF(YEAR, FechaNacimiento, GETDATE()) -
                           CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaNacimiento, GETDATE()), FechaNacimiento) > GETDATE() THEN 1 ELSE 0 END";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar edades: " + ex.Message);
                }
            }
        }



    }


}



