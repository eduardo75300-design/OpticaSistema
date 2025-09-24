
using System.Data.SqlClient;
using System.Security.Policy;

namespace OpticaSistema
{
    public partial class Form1 : Form
    {
        private ConexionDB conexionBD;
        string rutaProyecto = Directory.GetParent(Application.StartupPath).Parent.Parent.Parent.FullName;
        public Form1()
        {
            InitializeComponent();
            conexionBD = new ConexionDB();

            int anchoCampo = 300;
            int margenDerecho = 40;
            int posicionX = this.ClientSize.Width - anchoCampo - margenDerecho;

            // Altura total del bloque de login
            int alturaBloque = 35 + 20 + 35 + 20 + 40; // txtUsuario + espacio + txtContrasena + espacio + botón
            int posicionY = (this.ClientSize.Height - alturaBloque) / 2;

            // Título LOGIN centrado sobre el bloque
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

            // Botón 
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

            // Estilo del formulario
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;


            //Imagen Inicio Sección
            //string rutaImagen = @"F:\Proyecto\OpticaSistema\OpticaSistema\Imagenes\log.png";

            string rutaImagen = Path.Combine(rutaProyecto, "Imagenes", "log.png");

            if (File.Exists(rutaImagen))
            {
                PictureBox imagen = new PictureBox();
                imagen.Image = Image.FromFile(rutaImagen);
                imagen.SizeMode = PictureBoxSizeMode.StretchImage;
                imagen.Location = new Point(60,70);
                imagen.Size = new Size(300, 300);
                this.Controls.Add(imagen);
            }
            else
            {
                MessageBox.Show("No se encontró la imagen en:\n" + rutaImagen, "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Inicio de sesión";
            //this.Icon = new Icon(@"F:\Proyecto\OpticaSistema\OpticaSistema\Imagenes\log.ico");
            string rutaIcono = Path.Combine(rutaProyecto, "Imagenes", "log.ico");
            this.Icon = new Icon(rutaIcono);

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
                MessageBox.Show("Bienvenido Usuario", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblContraseña_Click(object sender, EventArgs e)
        {

        }
    }
}
