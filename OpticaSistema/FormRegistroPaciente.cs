using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpticaSistema.FormLogin;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace OpticaSistema
{
    public partial class FormRegistroPaciente : Form
    {
        private ConexionDB conexionBD;
        private DataGridView tablaPaciente;
        private Panel panelRegistro;
        private TextBox txtApellidos, txtNombres, txtDireccion, txtTelefono, txtCorreo, txtEstadoCivil, txtCelular, txtInstruccion, txtDni, txtDepartamento, txtProvincia, txtDistrito,txtEdad, txtOcupacion;
        private Button btnGuardar, btnCancelar;
        private bool modoEdicion = false;
        private string dniEditando = "";
        private ComboBox cmbSexo;
        private Label lblTituloRegistro;
        private DateTimePicker dtpFechaNacimiento;
        private CheckBox chkEstado;
        Dictionary<string, string> sexoOpciones = new Dictionary<string, string>
            {
                { "Ingresar sexo", "" },
                { "Masculino", "M" },
                { "Femenino", "F" }
            };
        public FormRegistroPaciente()
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
            titulo.Text = "PACIENTES";
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
            btnBuscar.Text = "BUSCAR";
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
            btnRegistrar.Anchor = AnchorStyles.Right;
            btnRegistrar.Margin = new Padding(5, 10, 5, 10);
            panelSuperior.Controls.Add(btnRegistrar, 3, 0);
            btnRegistrar.MinimumSize = new Size(130, 45);

            // Tabla de usuarios
            tablaPaciente = new DataGridView();
            tablaPaciente.Dock = DockStyle.Fill;
            tablaPaciente.Font = new Font("Segoe UI", 11);
            tablaPaciente.BackgroundColor = Color.White;
            tablaPaciente.RowHeadersVisible = false;
            tablaPaciente.AllowUserToAddRows = false;
            tablaPaciente.AllowUserToResizeRows = false;
            tablaPaciente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaPaciente.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            tablaPaciente.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            tablaPaciente.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaPaciente.EnableHeadersVisualStyles = false;
            tablaPaciente.SelectionMode = DataGridViewSelectionMode.CellSelect;
            tablaPaciente.DefaultCellStyle.SelectionBackColor = tablaPaciente.DefaultCellStyle.BackColor;
            tablaPaciente.DefaultCellStyle.SelectionForeColor = tablaPaciente.DefaultCellStyle.ForeColor;
            tablaPaciente.CurrentCell = null;
            tablaPaciente.ReadOnly = true;
            tablaPaciente.MultiSelect = false;

            tablaPaciente.Columns.Add("DNI", "DNI");
            tablaPaciente.Columns.Add("NOMBRES", "NOMBRES");

            // Columna de botón EDITAR con ícono
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.HeaderText = "EDITAR";
            colEditar.Text = "✏️"; // Puedes usar texto o dejarlo vacío
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.FlatStyle = FlatStyle.Flat;
            colEditar.DefaultCellStyle.BackColor = Color.White;
            colEditar.DefaultCellStyle.ForeColor = Color.Black;
            tablaPaciente.Columns.Add(colEditar);

            // Columna de botón ELIMINAR con ícono
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.HeaderText = "ELIMINAR";
            colEliminar.Text = "🗑️"; // Puedes usar texto o dejarlo vacío
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = Color.White;
            colEliminar.DefaultCellStyle.ForeColor = Color.Black;
            tablaPaciente.Columns.Add(colEliminar);

            layout.Controls.Add(tablaPaciente, 0, 2);
            CargarPaciente();
            tablaPaciente.CellClick += TablaPacientes_CellClick;

            // Panel de registro
            panelRegistro = new Panel();
            panelRegistro.Size = new Size(650, 800);
            panelRegistro.Location = new Point((this.Width - panelRegistro.Width) / 2, -130);
            panelRegistro.BackColor = Color.White;
            panelRegistro.BorderStyle = BorderStyle.None;
            panelRegistro.Padding = new Padding(20);
            panelRegistro.Visible = false;
            panelRegistro.Anchor = AnchorStyles.None;

            // Borde decorativo
            panelRegistro.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelRegistro.ClientRectangle,
                    Color.LightGray, 2, ButtonBorderStyle.Solid,
                    Color.LightGray, 2, ButtonBorderStyle.Solid,
                    Color.LightGray, 2, ButtonBorderStyle.Solid,
                    Color.LightGray, 2, ButtonBorderStyle.Solid);
            };

            // Fuente y tamaño de campos
            Font campoFont = new Font("Segoe UI", 11);
            Size campoSize = new Size(520, 35);

            // Campos de entrada
            txtNombres = new TextBox {Font = campoFont, Size = campoSize, PlaceholderText = "Nombres", Margin = new Padding(0, 12, 0, 0) };
            txtApellidos = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Apellidos", Margin = new Padding(0, 12, 0, 0) };
            txtDireccion = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Dirección", Margin = new Padding(0, 12, 0, 0) };
            txtTelefono = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Teléfono", Margin = new Padding(0, 12, 0, 0) };
            txtCorreo = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Correo", Margin = new Padding(0, 12, 0, 0) };
            txtEstadoCivil = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Estado Civil", Margin = new Padding(0, 12, 0, 0) };
            txtCelular = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Celular", Margin = new Padding(0, 12, 0, 0) };
            txtInstruccion = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Instrucción", Margin = new Padding(0, 12, 0, 0) };
            txtDni = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "DNI", Margin = new Padding(0, 12, 0, 0) };
            txtDepartamento = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Departamento", Margin = new Padding(0, 12, 0, 0) };
            txtProvincia = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Provincia", Margin = new Padding(0, 12, 0, 0) };
            txtDistrito = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Distrito", Margin = new Padding(0, 12, 0, 0) };
            chkEstado = new CheckBox { Text = "Activo", Font = campoFont, Checked = true, Margin = new Padding(0, 12, 0, 0),AutoSize = true };

            dtpFechaNacimiento = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short, // Muestra formato dd/MM/yyyy
                Font = campoFont,
                Size = campoSize,
                Margin = new Padding(0, 12, 0, 0),
                MinDate = new DateTime(1920, 1, 1),
                MaxDate = DateTime.Today
            };
            txtEdad = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Edad", Margin = new Padding(0, 12, 0, 0) };
            txtEdad.ReadOnly = true;
            txtEdad.Enabled = false;
            txtOcupacion = new TextBox { Font = campoFont, Size = campoSize, PlaceholderText = "Ocupación", Margin = new Padding(0, 12, 0, 0) };
            Label lblFechaNacimiento = new Label
            {
                Text = "Fecha de Nacimiento",
                Font = campoFont,
                ForeColor = Color.Black,
                Size = campoSize,
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 0)
            };

            Label lblEdad = new Label
            {
                Text = "Edad",
                Font = campoFont,
                Size = campoSize,
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 0)
            };

            dtpFechaNacimiento.ValueChanged += (s, e) =>
            {
                DateTime fechaNacimiento = dtpFechaNacimiento.Value;
                DateTime hoy = DateTime.Today;

                int edad = hoy.Year - fechaNacimiento.Year;
                if (fechaNacimiento > hoy.AddYears(-edad)) edad--; // Ajuste si aún no ha cumplido años este año

                txtEdad.Text = edad.ToString();
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
                Text = "Registrar Paciente",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Fill,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelRegistro.Controls.Add(lblTituloRegistro);

            // Contenedor de campos
            FlowLayoutPanel contenedor = new FlowLayoutPanel();
            contenedor.FlowDirection = FlowDirection.TopDown;
            contenedor.Dock = DockStyle.Fill;
            contenedor.WrapContents = false;
            contenedor.AutoScroll = true;
            contenedor.Controls.Add(txtNombres);
            contenedor.Controls.Add(txtApellidos);
            contenedor.Controls.Add(txtDireccion);
            contenedor.Controls.Add(txtTelefono);
            contenedor.Controls.Add(txtCorreo);
            contenedor.Controls.Add(txtEstadoCivil);
            contenedor.Controls.Add(txtCelular);
            contenedor.Controls.Add(txtInstruccion);
            contenedor.Controls.Add(txtDni);
            contenedor.Controls.Add(txtDepartamento);
            contenedor.Controls.Add(txtProvincia);
            contenedor.Controls.Add(txtDistrito);
            contenedor.Controls.Add(cmbSexo);
            contenedor.Controls.Add(txtOcupacion);
            contenedor.Controls.Add(lblFechaNacimiento);
            contenedor.Controls.Add(dtpFechaNacimiento);
            contenedor.Controls.Add(lblEdad);
            contenedor.Controls.Add(txtEdad);
            contenedor.Controls.Add(chkEstado);


            // Contenedor de botones
            FlowLayoutPanel botones = new FlowLayoutPanel();
            botones.FlowDirection = FlowDirection.LeftToRight;
            botones.Dock = DockStyle.Fill;
            botones.Anchor = AnchorStyles.None;
            botones.Padding = new Padding(0, 20, 0, 0);
            botones.Margin = new Padding(0, 20, 0, 0);
            botones.WrapContents = false;
            botones.AutoSize = true;
            botones.Controls.Add(btnGuardar);
            botones.Controls.Add(btnCancelar);

            // Layout principal del panel
            TableLayoutPanel layoutRegistro = new TableLayoutPanel();
            layoutRegistro.Dock = DockStyle.Fill;
            layoutRegistro.RowCount = 3;
            layoutRegistro.ColumnCount = 1;
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // Título
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Campos
            layoutRegistro.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // Botones

            layoutRegistro.Controls.Add(lblTituloRegistro, 0, 0);
            layoutRegistro.Controls.Add(contenedor, 0, 1);
            layoutRegistro.Controls.Add(botones, 0, 2);

            // Agregar layout al panel
            panelRegistro.Controls.Clear();
            panelRegistro.Controls.Add(layoutRegistro);

            // Agregar panel al formulario
            this.Controls.Add(panelRegistro);

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
                lblTituloRegistro.Text = "Registrar Paciente";
                modoEdicion = false;
                dniEditando = null;

            };
            btnGuardar.Click += (s, e) =>
            {
                string sexoSeleccionado = cmbSexo.SelectedItem?.ToString();
                
                if (string.IsNullOrWhiteSpace(txtNombres.Text) ||
                    string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                    string.IsNullOrWhiteSpace(txtDni.Text)||
                    string.IsNullOrWhiteSpace(dtpFechaNacimiento.Text))
                    

                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                if (!Regex.IsMatch(txtDni.Text.Trim(), @"^\d{8}$"))
                {
                    MessageBox.Show("El DNI debe contener exactamente 8 dígitos numéricos.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(sexoSeleccionado) || sexoSeleccionado == "Ingresar sexo")
                {
                    MessageBox.Show("Por favor, seleccione el sexo");
                    return;
                }

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();

                        if (modoEdicion)
                        {
                            // Actualizar usuario existente
                            string query = @"
    UPDATE PacienteBD SET
        Apellidos        = @apellidos,
        Nombres          = @nombres,
        Direccion           = @direccion,
        Telefono         = @telefono,
        Correo           = @correo,
        EstadoCivil      = @estadoCivil,
        Celular          = @celular,
        Instruccion      = @instruccion,
        Departamento     = @departamento,
        Provincia        = @provincia,
        Distrito         = @distrito,
        Sexo             = @sexo,
        FechaNacimiento  = @fechaNacimiento,
        Edad             = @edad,
        Ocupacion           = @ocupacion,
        Estado = @estado
    WHERE Dni = @dni";

                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@nombres", txtNombres.Text.Trim());
                            cmd.Parameters.AddWithValue("@apellidos", txtApellidos.Text.Trim());
                            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@estadoCivil", txtEstadoCivil.Text.Trim());
                            cmd.Parameters.AddWithValue("@celular", txtCelular.Text.Trim());
                            cmd.Parameters.AddWithValue("@instruccion", txtInstruccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@departamento", txtDepartamento.Text.Trim());
                            cmd.Parameters.AddWithValue("@provincia", txtProvincia.Text.Trim());
                            cmd.Parameters.AddWithValue("@distrito", txtDistrito.Text.Trim());
                            cmd.Parameters.AddWithValue("@sexo", sexoOpciones[sexoSeleccionado]); // si usas ComboBox
                            string fechaNacimiento = dtpFechaNacimiento.Value.ToString("yyyy-MM-dd");
                            cmd.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                            cmd.Parameters.AddWithValue("@edad", txtEdad.Text.Trim());
                            cmd.Parameters.AddWithValue("@ocupacion", txtOcupacion.Text.Trim());
                            cmd.Parameters.AddWithValue("@dni", dniEditando);
                            cmd.Parameters.AddWithValue("@estado", chkEstado.Checked);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario actualizado correctamente.");
                        }
                        else
                        {
                            // Verificar si el DNI ya existe
                            string verificarQuery = "SELECT COUNT(*) FROM PacienteBD WHERE Dni = @dni";
                            SqlCommand verificarCmd = new SqlCommand(verificarQuery, cn);
                            verificarCmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                            int existe = (int)verificarCmd.ExecuteScalar();

                            if (existe > 0)
                            {
                                MessageBox.Show("Ya existe un usuario con ese DNI.");
                                return;
                            }

                            // Insertar nuevo paciente
                            string query = @"
    INSERT INTO PacienteBD (
        Apellidos, Nombres, Direccion, Telefono, Correo, EstadoCivil, Celular,
        Instruccion, Dni, Departamento, Provincia, Distrito, Sexo,
        FechaNacimiento, Edad, Ocupacion, Estado
    )
    VALUES (
        @apellidos, @nombres, @direccion, @telefono, @correo, @estadoCivil, @celular,
        @instruccion, @dni, @departamento, @provincia, @distrito, @sexo,
        @fechaNacimiento, @edad, @ocupacion, @estado
    )";


                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@apellidos", txtApellidos.Text.Trim());
                            cmd.Parameters.AddWithValue("@nombres", txtNombres.Text.Trim());
                            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@estadoCivil", txtEstadoCivil.Text.Trim());
                            cmd.Parameters.AddWithValue("@celular", txtCelular.Text.Trim());
                            cmd.Parameters.AddWithValue("@instruccion", txtInstruccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                            cmd.Parameters.AddWithValue("@departamento", txtDepartamento.Text.Trim());
                            cmd.Parameters.AddWithValue("@provincia", txtProvincia.Text.Trim());
                            cmd.Parameters.AddWithValue("@distrito", txtDistrito.Text.Trim());
                            cmd.Parameters.AddWithValue("@sexo", sexoOpciones[sexoSeleccionado]); // o txtSexo.Text.Trim()
                            string fechaNacimiento = dtpFechaNacimiento.Value.ToString("yyyy-MM-dd");
                            cmd.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                            cmd.Parameters.AddWithValue("@edad", txtEdad.Text.Trim());
                            cmd.Parameters.AddWithValue("@ocupacion", txtOcupacion.Text.Trim());
                            cmd.Parameters.AddWithValue("@estado", true);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Paciente registrado correctamente.");

                        }

                        // Limpiar campos y cerrar panel
                        LimpiarCamposRegistro();
                        modoEdicion = false;
                        dniEditando = "";

                        panelRegistro.Visible = false;
                        DesbloquearControles();
                        CargarPaciente();
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
                    CargarPaciente();
                    return;
                }

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();
                        string query = @"SELECT Dni, CONCAT(Nombres, ' ', Apellidos) AS NOMBRES
                                 FROM PacienteBD
                                 WHERE Dni = @dni";

                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        tablaPaciente.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            tablaPaciente.Rows.Add(
                                row["Dni"].ToString(),
                                row["NOMBRES"].ToString()
                            );
                        }


                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontró ningún usuario con ese DNI.");
                            CargarPaciente();
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
        private void FormRegistroPaciente_Load(object sender, EventArgs e)
        {

            this.Text = "OpticaSistema - Registrar Paciente";
            this.Icon = new Icon("Imagenes/log.ico");
        }

        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void LimpiarCamposRegistro()
        {
            txtApellidos.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtEstadoCivil.Text = string.Empty;
            txtCelular.Text = string.Empty;
            txtInstruccion.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtDepartamento.Text = string.Empty;
            txtProvincia.Text = string.Empty;
            txtDistrito.Text = string.Empty;
            cmbSexo.SelectedIndex = 0;
            dtpFechaNacimiento.Value = DateTime.Today;
            txtEdad.Text = string.Empty;
            txtOcupacion.Text = string.Empty;
            chkEstado.Checked = true;

        }
        private void TablaPacientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !(tablaPaciente.Columns[e.ColumnIndex] is DataGridViewButtonColumn))
                return;

            string dni = tablaPaciente.Rows[e.RowIndex].Cells["DNI"].Value.ToString();
            string header = tablaPaciente.Columns[e.ColumnIndex].HeaderText;

            if (header == "ELIMINAR")
            {
                DialogResult confirmacion = MessageBox.Show($"¿Deseas dar de baja al paciente con DNI {dni}?", "Confirmar baja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    using (SqlConnection cn = conexionBD.Conectar())
                    {
                        try
                        {
                            cn.Open();
                            string query = "UPDATE PacienteBD SET Estado = 0 WHERE Dni = @dni"; // ✅ baja lógica
                            SqlCommand cmd = new SqlCommand(query, cn);
                            cmd.Parameters.AddWithValue("@dni", dni);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Paciente dado de baja correctamente.");
                            CargarPaciente(); // ✅ recarga la vista sin mostrar pacientes inactivos
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al dar de baja al paciente: " + ex.Message);
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
                        string query = "SELECT * FROM PacienteBD WHERE Dni = @dni";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtNombres.Text = reader["Nombres"].ToString();
                            txtApellidos.Text = reader["Apellidos"].ToString();
                            txtDni.Text = reader["Dni"].ToString();
                            txtCorreo.Text = reader["Correo"].ToString();
                            txtDireccion.Text = reader["Direccion"].ToString();
                            txtCelular.Text = reader["Celular"].ToString();
                            txtTelefono.Text = reader["Telefono"].ToString();
                            txtEstadoCivil.Text = reader["EstadoCivil"].ToString();
                            txtInstruccion.Text = reader["Instruccion"].ToString();
                            txtDepartamento.Text = reader["Departamento"].ToString();
                            txtProvincia.Text = reader["Provincia"].ToString();
                            txtDistrito.Text = reader["Distrito"].ToString();
                            dtpFechaNacimiento.Value = DateTime.Parse(reader["FechaNacimiento"].ToString());
                            txtEdad.Text = reader["Edad"].ToString();
                            txtOcupacion.Text = reader["Ocupacion"].ToString();
                            chkEstado.Checked = Convert.ToBoolean(reader["Estado"]);

                            // Si estás usando ComboBox para Sexo
                            string sexo = reader["Sexo"].ToString();
                            string sexoTexto = sexoOpciones.FirstOrDefault(x => x.Value == sexo).Key;
                            cmbSexo.SelectedItem = cmbSexo.Items.Contains(sexoTexto) ? sexoTexto : "Ingresar sexo";

                            lblTituloRegistro.Text = "Editar Paciente";

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

        private void CargarPaciente()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                try
                {
                    cn.Open();
                    string query = @"SELECT Dni, CONCAT(Nombres, ' ', Apellidos) AS NOMBRES FROM PacienteBD WHERE Estado = 1";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    tablaPaciente.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        string dni = row["Dni"].ToString();
                        string nombreCompleto = row["NOMBRES"].ToString();

                        tablaPaciente.Rows.Add(dni, nombreCompleto);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar usuarios: " + ex.Message);
                }
            }
        }
    }

}
