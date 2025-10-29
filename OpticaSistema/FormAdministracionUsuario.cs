using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpticaSistema.FormLogin;

namespace OpticaSistema
{
    public partial class FormAdministracionUsuario : Form
    {
        private ConexionDB conexionBD;
        private DataGridView tablaUsuarios;
        private Panel panelRegistro;
        private TextBox txtNombre, txtApellido, txtDni, txtCorreo, txtClave, txtTipoUsuario, txtDireccion, txtCelular;
        private Button btnGuardar, btnCancelar;
        private bool modoEdicion = false;
        private string dniEditando = "";
        private ComboBox cmbSexo;
        private ComboBox cmbTipoUsuario;
        private Label lblTituloRegistro;
        private CheckBox chkEstado;


        Dictionary<string, string> sexoOpciones = new Dictionary<string, string>
            {
                { "Ingresar sexo", "" },
                { "Masculino", "M" },
                { "Femenino", "F" }
            };

        Dictionary<string, string> tipoUsuarioOpciones = new Dictionary<string, string>
            {
                { "Ingresar tipo de usuario", "" },
                { "Administrador", "A" },
                { "Admisión", "S" },
                { "Optómetra", "O" },
                { "Oftalmólogo", "F" },
                { "Retinólogo ","R" }

            };

        public FormAdministracionUsuario()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            conexionBD = new ConexionDB();
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
            titulo.Text = "REGISTRO DE USUARIOS";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.DarkBlue;
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.Margin = new Padding(0, 30, 0, 0); // Más abajo
            layout.Controls.Add(titulo, 0, 0);

            // Panel superior: búsqueda + botones
            TableLayoutPanel panelSuperior = new TableLayoutPanel();
            panelSuperior.Dock = DockStyle.Fill;
            panelSuperior.ColumnCount = 5;
            panelSuperior.RowCount = 1;
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));   // Label DNI
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));  // TextBox
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));  // Buscar
            panelSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // Registrar
            panelSuperior.ColumnStyles[3] = new ColumnStyle(SizeType.Absolute, 120);
            layout.Controls.Add(panelSuperior, 0, 1);

            // Label DNI
            Label lblDni = new Label();
            lblDni.Text = "DNI:";
            lblDni.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDni.TextAlign = ContentAlignment.MiddleLeft;
            lblDni.Dock = DockStyle.Fill;
            lblDni.Margin = new Padding(5, 10, 0, 0);
            panelSuperior.Controls.Add(lblDni, 0, 0);

            // TextBox
            TextBox txtBuscarDni = new TextBox();
            txtBuscarDni.Font = new Font("Segoe UI", 11);
            txtBuscarDni.Width = 120;
            txtBuscarDni.Anchor = AnchorStyles.Left;
            txtBuscarDni.Margin = new Padding(0, 10, 0, 0);
            panelSuperior.Controls.Add(txtBuscarDni, 1, 0);

            // Botón BUSCAR
            Button btnBuscar = new Button();
            btnBuscar.Text = "BUSCAR";
            btnBuscar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBuscar.BackColor = Color.SteelBlue;
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Dock = DockStyle.Fill;
            btnBuscar.Margin = new Padding(5, 10, 5, 10);
            btnBuscar.MinimumSize = new Size(130, 45);
            panelSuperior.Controls.Add(btnBuscar, 2, 0);

            // Botón LIMPIAR
            Button btnLimpiar = new Button();
            btnLimpiar.Text = "ACTUALIZAR";
            btnLimpiar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLimpiar.BackColor = Color.SteelBlue;
            btnLimpiar.ForeColor = Color.White;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Dock = DockStyle.Fill;
            btnLimpiar.Margin = new Padding(5, 10, 5, 10);
            btnLimpiar.MinimumSize = new Size(150, 45);
            panelSuperior.Controls.Add(btnLimpiar, 3, 0);

            // Botón REGISTRAR
            Button btnRegistrar = new Button();
            btnRegistrar.Text = "REGISTRAR";
            btnRegistrar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRegistrar.BackColor = Color.SteelBlue;
            btnRegistrar.ForeColor = Color.White;
            btnRegistrar.FlatStyle = FlatStyle.Flat;
            btnRegistrar.Anchor = AnchorStyles.Right;
            btnRegistrar.Margin = new Padding(5, 10, 5, 10);
            btnRegistrar.MinimumSize = new Size(130, 45);
            panelSuperior.Controls.Add(btnRegistrar, 4, 0);


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
            tablaUsuarios.Columns.Add("TIPO USUARIO", "TIPO USUARIO");

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
            tablaUsuarios.CellClick += TablaUsuarios_CellClick;

            // Panel de registro
            panelRegistro = new Panel();
            panelRegistro.Location = new Point((this.Width - panelRegistro.Width) / 2, -130);
            panelRegistro.BackColor = Color.White;
            panelRegistro.BorderStyle = BorderStyle.None;
            panelRegistro.Padding = new Padding(20);
            panelRegistro.Visible = false;
            panelRegistro.Anchor = AnchorStyles.None;
            panelRegistro.BorderStyle = BorderStyle.FixedSingle;


            // Fuente y tamaño de campos
            Font campoFont = new Font("Segoe UI", 11);
            Size campoSize = new Size(400, 45);

            // Campos de entrada
            txtNombre = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Nombres", Margin = new Padding(0, 12, 0, 0) };
            txtApellido = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Apellidos", Margin = new Padding(0, 12, 0, 0) };
            txtDni = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "DNI", Margin = new Padding(0, 12, 0, 0) };
            txtCorreo = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Correo", Margin = new Padding(0, 12, 0, 0) };
            txtClave = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Contraseña", UseSystemPasswordChar = true, Margin = new Padding(0, 12, 0, 0) };
            txtDireccion = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Dirección", Margin = new Padding(0, 12, 0, 0) };
            txtCelular = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Celular", Margin = new Padding(0, 12, 0, 0) };
            chkEstado = new CheckBox {
                Text = "Activo",
                Font = campoFont,
                Checked = true, // por defecto activo
                Margin = new Padding(0, 12, 0, 0),
                AutoSize = true
            };

            cmbSexo = new ComboBox
            {
                Font = campoFont,
                Size = campoSize,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 12, 0, 0)
            };
            cmbSexo.Items.AddRange(sexoOpciones.Keys.ToArray());
            cmbSexo.SelectedIndex = 0;

            cmbTipoUsuario = new ComboBox
            {
                Font = campoFont,
                Size = campoSize,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 12, 0, 0)
            };
            cmbTipoUsuario.Items.AddRange(tipoUsuarioOpciones.Keys.ToArray());
            cmbTipoUsuario.SelectedIndex = 0;
            
                
            btnLimpiar.Click += (s, e) =>
            {
                CargarUsuarios();
            };

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 40),
                Margin = new Padding(10)
            };

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 40),
                Margin = new Padding(10)
            };

            // Título del panel
            lblTituloRegistro = new Label
            {
                Text = "Registrar nuevo Usuario",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Fill,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelRegistro.Controls.Add(lblTituloRegistro);
            Control CrearTitulo(string titulo)
            {
                return new Label
                {
                    Text = titulo,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true,
                    Padding = new Padding(0, 10, 0, 0), // espacio abajo dentro del label
                    TextAlign = ContentAlignment.BottomLeft // alinea el texto abajo



                };


            }
            // Contenedor de campos
            FlowLayoutPanel contenedor = new FlowLayoutPanel();
            contenedor.Dock = DockStyle.None;
            contenedor.Anchor = AnchorStyles.None;
            contenedor.AutoSize = true;
            contenedor.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            contenedor.Margin = new Padding(0);
            contenedor.FlowDirection = FlowDirection.TopDown;
            contenedor.WrapContents = false;
            contenedor.Controls.Add(CrearTitulo("Nombres"));
            contenedor.Controls.Add(txtNombre);
            contenedor.Controls.Add(CrearTitulo("Apellidos"));
            contenedor.Controls.Add(txtApellido);
            contenedor.Controls.Add(CrearTitulo("DNI"));
            contenedor.Controls.Add(txtDni);
            contenedor.Controls.Add(CrearTitulo("Correo"));
            contenedor.Controls.Add(txtCorreo);
            contenedor.Controls.Add(CrearTitulo("Contraseña"));
            contenedor.Controls.Add(txtClave);
            contenedor.Controls.Add(CrearTitulo("Dirección"));
            contenedor.Controls.Add(txtDireccion);
            contenedor.Controls.Add(CrearTitulo("Celular"));
            contenedor.Controls.Add(txtCelular);
            contenedor.Controls.Add(CrearTitulo("Sexo"));
            contenedor.Controls.Add(cmbSexo);
            contenedor.Controls.Add(CrearTitulo("Tipo de Usuario"));
            contenedor.Controls.Add(cmbTipoUsuario);
            contenedor.Controls.Add(chkEstado);

            // Contenedor de botones
            FlowLayoutPanel botones = new FlowLayoutPanel();
            botones.FlowDirection = FlowDirection.LeftToRight;
            botones.Dock = DockStyle.None;
            botones.Anchor = AnchorStyles.None;
            botones.AutoSize = true;
            botones.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            botones.Margin = new Padding(0, 20, 0, 0);
            botones.Controls.Add(btnGuardar);
            botones.Controls.Add(btnCancelar);

            // Layout principal del panel
            TableLayoutPanel layoutRegistro = new TableLayoutPanel();
            layoutRegistro.RowCount = 3;
            layoutRegistro.ColumnCount = 1;
            layoutRegistro.AutoSize = true;
            layoutRegistro.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layoutRegistro.Dock = DockStyle.Top;
            layoutRegistro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // Título
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Campos
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Botones
            layoutRegistro.Controls.Add(lblTituloRegistro, 0, 0);
            layoutRegistro.Controls.Add(contenedor, 0, 1);
            layoutRegistro.Controls.Add(botones, 0, 2);

            // Centrado explícito de contenido en celdas
            layoutRegistro.SetCellPosition(contenedor, new TableLayoutPanelCellPosition(0, 1));
            layoutRegistro.Controls[1].Anchor = AnchorStyles.None;

            layoutRegistro.SetCellPosition(botones, new TableLayoutPanelCellPosition(0, 2));
            layoutRegistro.Controls[2].Anchor = AnchorStyles.None;

            // Agregar layout al panel
            panelRegistro.Controls.Clear();
            panelRegistro.Controls.Add(layoutRegistro);
            panelRegistro.AutoScroll = true;

            // Agregar panel al formulario
            this.Controls.Add(panelRegistro);
            this.Resize += (s, e) => AjustarPanelRegistro();
            AjustarPanelRegistro();

            btnRegistrar.Click += (s, e) =>
            {
                panelRegistro.Visible = true;
                panelRegistro.BringToFront(); // 👈 esto asegura que se vea
                BloquearControles();
                chkEstado.Visible = false;

            };

            btnCancelar.Click += (s, e) =>
            {
                panelRegistro.Visible = false;
                DesbloquearControles();
                LimpiarCamposRegistro();
                lblTituloRegistro.Text = "Registrar nuevo usuario";
                modoEdicion = false;
                dniEditando = null;

            };

            
            btnGuardar.Click += (s, e) =>
            {
                string sexoSeleccionado = cmbSexo.SelectedItem?.ToString();
                string tipoSeleccionado = cmbTipoUsuario.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDni.Text) ||
                    string.IsNullOrWhiteSpace(txtClave.Text))

                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                if (!Regex.IsMatch(txtDni.Text.Trim(), @"^\d{8}$"))
                {
                    MessageBox.Show("El DNI debe contener exactamente 8 dígitos numéricos.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(sexoSeleccionado) || sexoSeleccionado == "Ingresar sexo" ||
                    string.IsNullOrWhiteSpace(tipoSeleccionado) || tipoSeleccionado == "Ingresar tipo de usuario")
                {
                    MessageBox.Show("Por favor, seleccione el sexo y el tipo de usuario.");
                    return;
                }
                if (!Regex.IsMatch(txtCelular.Text.Trim(), @"^\d{9}$"))
                {
                    MessageBox.Show("El celular debe contener exactamente 9 dígitos numéricos.");
                    return;
                }
                if (!Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Ingrese un correo electrónico válido.");
                    return;
                }

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();
  
                        if (modoEdicion)
                        {
                            string verificarQuery = "SELECT COUNT(*) FROM UsuarioBD WHERE Dni = @dni AND Dni <> @dniActual";
                            SqlCommand verificarCmd = new SqlCommand(verificarQuery, cn);
                            verificarCmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                            verificarCmd.Parameters.AddWithValue("@dniActual", dniEditando);

                            int existe = (int)verificarCmd.ExecuteScalar();

                            if (existe > 0)
                            {
                                MessageBox.Show("Ya existe un usuario registrado con ese DNI.");
                                return;
                            }


                            // Actualizar usuario existente
                            string query = @"UPDATE UsuarioBD SET Nombres = @nombres, Apellidos = @apellidos, Contraseña = @clave, Correo = @correo, Direccion = @direccion, Celular = @celular, Sexo = @sexo, TipoUsuario = @tipo, Estado = @estado, Dni = @dniNuevo WHERE Dni = @dni";

                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@nombres", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@apellidos", txtApellido.Text.Trim());
                            cmd.Parameters.AddWithValue("@clave", txtClave.Text.Trim());
                            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@celular", txtCelular.Text.Trim());
                            cmd.Parameters.AddWithValue("@sexo", sexoOpciones[sexoSeleccionado]);
                            cmd.Parameters.AddWithValue("@tipo", tipoUsuarioOpciones[tipoSeleccionado]);
                            cmd.Parameters.AddWithValue("@estado", chkEstado.Checked);
                            cmd.Parameters.AddWithValue("@dni", dniEditando);
                            cmd.Parameters.AddWithValue("@dniNuevo", txtDni.Text.Trim());
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario actualizado correctamente.");

                        }
                        else
                        {
                            
                            // Verificar si el DNI ya existe
                            string verificarQuery = "SELECT COUNT(*) FROM UsuarioBD WHERE Dni = @dni";
                            SqlCommand verificarCmd = new SqlCommand(verificarQuery, cn);
                            verificarCmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                            int existe = (int)verificarCmd.ExecuteScalar();

                            if (existe > 0)
                            {
                                MessageBox.Show("Ya existe un usuario con ese DNI.");
                                return;
                            }
                            // Insertar nuevo usuario con Estado y Firma
                            string query = @"INSERT INTO UsuarioBD (Nombres, Apellidos, Contraseña, Dni, Correo, Direccion, Sexo, TipoUsuario, Celular, Estado) VALUES (@nombres, @apellidos, @clave, @dni, @correo, @direccion, @sexo, @tipo, @celular, @estado)";

                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@nombres", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@apellidos", txtApellido.Text.Trim());
                            cmd.Parameters.AddWithValue("@clave", txtClave.Text.Trim());
                            cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@celular", txtCelular.Text.Trim());
                            cmd.Parameters.AddWithValue("@sexo", sexoOpciones[sexoSeleccionado]);
                            cmd.Parameters.AddWithValue("@estado", true);
                            cmd.Parameters.AddWithValue("@tipo", tipoUsuarioOpciones[tipoSeleccionado]);



                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario registrado correctamente.");

                        }

                        // Limpiar campos y cerrar panel
                        LimpiarCamposRegistro();

                        modoEdicion = false;
                        dniEditando = "";

                        panelRegistro.Visible = false;
                        DesbloquearControles();
                        CargarUsuarios();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar usuario: " + ex.Message);
                    }
                }
            };

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
                            CargarUsuarios();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al consultar la base de datos: " + ex.Message);
                    }

                }

            };


        }

        private void BloquearControles()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != panelRegistro)
                {
                    ctrl.Enabled = false;
                }
            }
        }
        private void DesbloquearControles()
        {
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Enabled = true;
            }
        }

        private void CargarUsuarios()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                try
                {
                    cn.Open();
                    string query = @"SELECT Dni, CONCAT(Nombres, ' ', Apellidos) AS NOMBRES, TipoUsuario 
                 FROM UsuarioBD 
                 WHERE Estado = 1";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    tablaUsuarios.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        string dni = row["Dni"].ToString();
                        string nombreCompleto = row["NOMBRES"].ToString();
                        string tipoCodigo = row["TipoUsuario"].ToString();

                        // Buscar el texto completo en el diccionario
                        string tipoTexto = tipoUsuarioOpciones.FirstOrDefault(x => x.Value == tipoCodigo).Key;

                        // Si no se encuentra, mostrar el código original
                        if (string.IsNullOrEmpty(tipoTexto))
                            tipoTexto = tipoCodigo;

                        tablaUsuarios.Rows.Add(dni, nombreCompleto, tipoTexto);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar usuarios: " + ex.Message);
                }
            }
        }


        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void TablaUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !(tablaUsuarios.Columns[e.ColumnIndex] is DataGridViewButtonColumn))
                return;

            string dni = tablaUsuarios.Rows[e.RowIndex].Cells["DNI"].Value.ToString();
            string header = tablaUsuarios.Columns[e.ColumnIndex].HeaderText;

            if (header == "ELIMINAR")
            {
                if (dni == Sesion.UsuarioDni)
                {
                    MessageBox.Show("No puedes dar de baja al usuario con el que estás conectado.");
                    return;
                }

                DialogResult confirmacion = MessageBox.Show($"¿Seguro que deseas eliminar el historial del paciente con DNI {dni}?",             "Confirmar eliminación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    using (SqlConnection cn = conexionBD.Conectar())
                    {
                        try
                        {
                            cn.Open();
                            string query = "UPDATE UsuarioBD SET Estado = 0 WHERE Dni = @dni";
                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@dni", dni);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarUsuarios();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al dar de baja al usuario: " + ex.Message);
                        }
                    }
                }
            }

            else if (header == "EDITAR")
            {
                chkEstado.Visible = true;

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();
                        string query = "SELECT * FROM UsuarioBD WHERE Dni = @dni";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtNombre.Text = reader["Nombres"].ToString();
                            txtApellido.Text = reader["Apellidos"].ToString();
                            txtDni.Text = reader["Dni"].ToString();
                            txtCorreo.Text = reader["Correo"].ToString();
                            txtClave.Text = reader["Contraseña"].ToString();
                            txtDireccion.Text = reader["Direccion"].ToString();
                            txtCelular.Text = reader["Celular"].ToString();

                            string sexo = reader["Sexo"].ToString();
                            string tipo = reader["TipoUsuario"].ToString();

                            string sexoTexto = sexoOpciones.FirstOrDefault(x => x.Value == sexo).Key;
                            string tipoTexto = tipoUsuarioOpciones.FirstOrDefault(x => x.Value == tipo).Key;

                            cmbSexo.SelectedItem = cmbSexo.Items.Contains(sexoTexto) ? sexoTexto : "Ingresar sexo";
                            cmbTipoUsuario.SelectedItem = cmbTipoUsuario.Items.Contains(tipoTexto) ? tipoTexto : "Ingresar tipo de usuario";
                            cmd.Parameters.AddWithValue("@Estado", true);
                            chkEstado.Visible = false;       

                            lblTituloRegistro.Text = "Editar usuario";
                            modoEdicion = true;
                            dniEditando = dni;

                            panelRegistro.Visible = true;
                            panelRegistro.BringToFront();
                            BloquearControles();

                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar datos del usuario: " + ex.Message);
                    }
                }
            }

        }

        private void FormAdministracionUsuario_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Administración Usuario";
            this.Icon = new Icon("Imagenes/log.ico");
        }
        private void LimpiarCamposRegistro()
        {
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtClave.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtCelular.Text = string.Empty;
            chkEstado.Checked = true; // Estado por defecto activo
            cmbSexo.SelectedIndex = 0;
            cmbTipoUsuario.SelectedIndex = 0;

        }
        private void AjustarPanelRegistro()
        {
            if (panelRegistro != null)
            {
                int ancho = (int)(this.ClientSize.Width * 0.35);   // 60% del ancho de la ventana
                int alto = (int)(this.ClientSize.Height * 0.82);   // 80% del alto de la ventana

                panelRegistro.Size = new Size(ancho, alto);
                panelRegistro.Location = new Point(
                    (this.ClientSize.Width - panelRegistro.Width) / 2,
                    (this.ClientSize.Height - panelRegistro.Height) / 2
                );
            }
        }

    }

}
