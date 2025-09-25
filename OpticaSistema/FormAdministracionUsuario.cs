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
using static OpticaSistema.Login;

namespace OpticaSistema
{
    public partial class FormAdministracionUsuario : Form
    {
        private ConexionDB conexionBD;
        private DataGridView tablaUsuarios;

        public FormAdministracionUsuario()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            conexionBD = new ConexionDB();
            this.Text = "USUARIOS";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Contenedor principal
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));   // Título
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));    // Filtro y botones
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // Tabla
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));    // Espacio inferior
            layout.Padding = new Padding(50, 20, 50, 20);
            this.Controls.Add(layout);
            

            // Título
            Label titulo = new Label();
            titulo.Text = "USUARIOS";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.DarkBlue;
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.Margin = new Padding(0, 30, 0, 0); // Más abajo
            layout.Controls.Add(titulo, 0, 0);

            // Panel superior: búsqueda + botones
            TableLayoutPanel panelSuperior = new TableLayoutPanel();
            panelSuperior.Dock = DockStyle.Fill;
            panelSuperior.ColumnCount = 4;
            panelSuperior.RowCount = 1;
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));   // Label DNI
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));  // TextBox
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));  // Buscar
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // Registrar
            layout.Controls.Add(panelSuperior, 0, 1);

            Label lblDni = new Label();
            lblDni.Text = "DNI:";
            lblDni.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDni.TextAlign = ContentAlignment.MiddleLeft;
            lblDni.Dock = DockStyle.Fill;
            lblDni.Margin = new Padding(5, 10, 0, 0);
            panelSuperior.Controls.Add(lblDni, 0, 0);

            TextBox txtBuscarDni = new TextBox();
            txtBuscarDni.Font = new Font("Segoe UI", 11);
            txtBuscarDni.Width = 120;
            txtBuscarDni.Anchor = AnchorStyles.Left;
            txtBuscarDni.Margin = new Padding(0, 10, 0, 0);
            panelSuperior.Controls.Add(txtBuscarDni, 1, 0);

            Button btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBuscar.BackColor = Color.SteelBlue;
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Dock = DockStyle.Fill;
            btnBuscar.Margin = new Padding(5, 10, 5, 10);
            panelSuperior.Controls.Add(btnBuscar, 2, 0);

            Button btnRegistrar = new Button();
            btnRegistrar.Text = "REGISTRAR";
            btnRegistrar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRegistrar.BackColor = Color.SteelBlue;
            btnRegistrar.ForeColor = Color.White;
            btnRegistrar.FlatStyle = FlatStyle.Flat;
            btnRegistrar.Height = 35;
            btnRegistrar.Width = 130;
            btnRegistrar.Anchor = AnchorStyles.Right;
            btnRegistrar.Margin = new Padding(5, 10, 5, 10);
            panelSuperior.Controls.Add(btnRegistrar, 3, 0);

            // Tabla de usuarios
            tablaUsuarios = new DataGridView();
            tablaUsuarios.Dock = DockStyle.Fill;
            tablaUsuarios.Font = new Font("Segoe UI", 11);
            tablaUsuarios.BackgroundColor = Color.White;
            tablaUsuarios.RowHeadersVisible = false;
            tablaUsuarios.AllowUserToAddRows = false;
            tablaUsuarios.AllowUserToResizeRows = false;
            tablaUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            tablaUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            tablaUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaUsuarios.EnableHeadersVisualStyles = false;
            tablaUsuarios.SelectionMode = DataGridViewSelectionMode.CellSelect;
            tablaUsuarios.DefaultCellStyle.SelectionBackColor = tablaUsuarios.DefaultCellStyle.BackColor;
            tablaUsuarios.DefaultCellStyle.SelectionForeColor = tablaUsuarios.DefaultCellStyle.ForeColor;
            tablaUsuarios.CurrentCell = null;
            tablaUsuarios.ReadOnly = true;
            tablaUsuarios.MultiSelect = false;

            tablaUsuarios.Columns.Add("DNI", "DNI");
            tablaUsuarios.Columns.Add("NOMBRES", "NOMBRES");
            tablaUsuarios.Columns.Add("USUARIO", "USUARIO");

            // Columna de botón EDITAR con ícono
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.HeaderText = "EDITAR";
            colEditar.Text = "✏️"; // Puedes usar texto o dejarlo vacío
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.FlatStyle = FlatStyle.Flat;
            colEditar.DefaultCellStyle.BackColor = Color.White;
            colEditar.DefaultCellStyle.ForeColor = Color.Black;
            tablaUsuarios.Columns.Add(colEditar);

            // Columna de botón ELIMINAR con ícono
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.HeaderText = "ELIMINAR";
            colEliminar.Text = "🗑️"; // Puedes usar texto o dejarlo vacío
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = Color.White;
            colEliminar.DefaultCellStyle.ForeColor = Color.Black;
            tablaUsuarios.Columns.Add(colEliminar);

            layout.Controls.Add(tablaUsuarios, 0, 2);
            CargarUsuarios();

            // Evento Buscar
            btnBuscar.Click += (s, e) =>
            {
                string dni = txtBuscarDni.Text.Trim();

                if (dni.Length != 8)
                {
                    MessageBox.Show("Ingrese un DNI válido de 8 dígitos.");
                    return;
                }

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();
                        string query = @"SELECT Dni, CONCAT(Nombres, ' ', Apellidos) AS NOMBRES, Correo AS USUARIO
                                 FROM UsuarioBD
                                 WHERE Dni = @dni";

                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        tablaUsuarios.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            tablaUsuarios.Rows.Add(
                                row["Dni"].ToString(),
                                row["NOMBRES"].ToString(),
                                row["USUARIO"].ToString()
                            );
                        }

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontró ningún usuario con ese DNI.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al consultar la base de datos: " + ex.Message);
                    }

                }

            };

        }

        private void CargarUsuarios()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                try
                {
                    cn.Open();
                    string query = @"SELECT Dni, CONCAT(Nombres, ' ', Apellidos) AS NOMBRES, Correo AS USUARIO FROM UsuarioBD";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd); // ✅ agregado
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    tablaUsuarios.Rows.Clear(); // ✅ no recrear la tabla

                    foreach (DataRow row in dt.Rows)
                    {
                        tablaUsuarios.Rows.Add(
                            row["Dni"].ToString(),
                            row["NOMBRES"].ToString(),
                            row["USUARIO"].ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar usuarios: " + ex.Message);
                }

            }
        }



        private void FormAdministracionUsuario_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Usuario";
            this.Icon = new Icon("Imagenes/log.ico");

        }
        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        

    }

}
