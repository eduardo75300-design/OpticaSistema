using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpticaSistema
{
    public partial class FormGenerarHistorial : Form
    {
        private ConexionDB conexionBD;
        private FlowLayoutPanel panelHorizontal;
        TextBox txtBuscar = new TextBox();
        ComboBox cmbMotivo = new ComboBox();
        public FormGenerarHistorial()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            conexionBD = new ConexionDB();


            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));   // Título
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));   // Filtro y botones
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // Campos
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));    // Botón REGISTRAR
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));    // Espacio inferior
            layout.Padding = new Padding(50, 20, 50, 20);
            this.Controls.Add(layout);


            Label titulo = new Label();
            titulo.Text = "GENERAR HISTORIAL CLINICO";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.DarkBlue;
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.Margin = new Padding(0, 30, 0, 0); // Más abajo
            layout.Controls.Add(titulo, 0, 0);

            // FlowLayoutPanel para centrar búsqueda
            FlowLayoutPanel panelBusqueda = new FlowLayoutPanel();
            panelBusqueda.FlowDirection = FlowDirection.LeftToRight;
            panelBusqueda.WrapContents = false;
            panelBusqueda.AutoSize = true;
            panelBusqueda.Padding = new Padding(0, 10, 0, 10);
            panelBusqueda.Anchor = AnchorStyles.Top;
            panelBusqueda.Margin = new Padding(0, 10, 0, 0);

            // Label "DNI"
            Label lblBuscar = new Label();
            lblBuscar.Text = "DNI:";
            lblBuscar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBuscar.TextAlign = ContentAlignment.MiddleCenter;
            lblBuscar.AutoSize = false;
            lblBuscar.Width = 60;
            lblBuscar.Height = 40;
            lblBuscar.Margin = new Padding(10, 5, 5, 5);

            // TextBox de búsqueda
            txtBuscar = new TextBox();
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Font = new Font("Segoe UI", 12);
            txtBuscar.Width = 150;
            txtBuscar.Height = 40;
            txtBuscar.Margin = new Padding(5, 5, 5, 5);

            // Botón de búsqueda
            Button btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnBuscar.Width = 120;
            btnBuscar.Height = 40;
            btnBuscar.TextAlign = ContentAlignment.MiddleCenter;
            btnBuscar.Margin = new Padding(5, 5, 10, 5);

            // Botón de limpiar
            Button btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLimpiar.Width = 120;
            btnLimpiar.Height = 40;
            btnLimpiar.TextAlign = ContentAlignment.MiddleCenter;
            btnLimpiar.Margin = new Padding(5, 5, 10, 5);

            // Agregar controles al panel de búsqueda
            panelBusqueda.Controls.Add(lblBuscar);
            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(btnBuscar);
            panelBusqueda.Controls.Add(btnLimpiar);

            // Panel horizontal para motivo de consulta
            FlowLayoutPanel panelMotivo = new FlowLayoutPanel();
            panelMotivo.FlowDirection = FlowDirection.LeftToRight;
            panelMotivo.WrapContents = false;
            panelMotivo.AutoSize = true;
            panelMotivo.Anchor = AnchorStyles.Top;
            panelMotivo.Padding = new Padding(0);
            panelMotivo.Margin = new Padding(0, 0, 0, 0);

            // Label "Motivo de consulta"
            Label lblMotivo = new Label();
            lblMotivo.Text = "Motivo de consulta:";
            lblMotivo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblMotivo.TextAlign = ContentAlignment.MiddleCenter;
            lblMotivo.AutoSize = false;
            lblMotivo.Width = 200;
            lblMotivo.Height = 40;
            lblMotivo.Margin = new Padding(10, 5, 5, 5);

            // ComboBox
            cmbMotivo = new ComboBox();
            cmbMotivo.Name = "cmbMotivo";
            cmbMotivo.Font = new Font("Segoe UI", 12);
            cmbMotivo.Width = 250;
            cmbMotivo.Height = 40;
            cmbMotivo.Margin = new Padding(5, 5, 5, 5);
            cmbMotivo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMotivo.Items.AddRange(new string[]
            {
    "Exámen Ocular Completo",
    "Medida de la Vista",
    "Consulta Oftalmológica",
    "Consulta con Retinólogo",
                });
            cmbMotivo.SelectedIndex = 0;

            // Agregar controles al panel motivo
            panelMotivo.Controls.Add(lblMotivo);
            panelMotivo.Controls.Add(cmbMotivo);

            // Panel contenedor vertical para búsqueda + motivo
            TableLayoutPanel panelBusquedaMotivo = new TableLayoutPanel();
            panelBusquedaMotivo.ColumnCount = 1;
            panelBusquedaMotivo.RowCount = 2;
            panelBusquedaMotivo.Dock = DockStyle.Top;
            panelBusquedaMotivo.AutoSize = true;
            panelBusquedaMotivo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBusquedaMotivo.Anchor = AnchorStyles.Top;
            panelBusquedaMotivo.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: DNI
            panelBusquedaMotivo.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 1: Motivo

            // Agregar paneles al contenedor
            panelBusquedaMotivo.Controls.Add(panelBusqueda, 0, 0);
            panelBusquedaMotivo.Controls.Add(panelMotivo, 0, 1);

            // Agregar el contenedor completo al layout principal en la fila 1
            layout.RowStyles[1] = new RowStyle(SizeType.AutoSize); // ← Esto es clave
            layout.Controls.Add(panelBusquedaMotivo, 0, 1);



            // Contenedor centrado con 3 columnas
            TableLayoutPanel contenedorCentral = new TableLayoutPanel();
            contenedorCentral.Dock = DockStyle.Fill;
            contenedorCentral.ColumnCount = 1;
            contenedorCentral.ColumnStyles.Clear();
            contenedorCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Panel horizontal dentro de columna central
            panelHorizontal = new FlowLayoutPanel();
            panelHorizontal.FlowDirection = FlowDirection.LeftToRight;
            panelHorizontal.WrapContents = true;
            panelHorizontal.AutoScroll = true;
            panelHorizontal.AutoSize = true;
            panelHorizontal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelHorizontal.Dock = DockStyle.None; // ← importante
            panelHorizontal.Anchor = AnchorStyles.Top;
            panelHorizontal.Padding = new Padding(0);
            panelHorizontal.Margin = new Padding(0, 0, 0, 10);


            // Campos de la BD
            string[] campos = new string[]
            {
    "Apellidos", "Nombres", "Direccion", "Telefono", "Correo", "EstadoCivil",
    "Celular", "Instruccion", "Dni", "Departamento", "Provincia", "Distrito",
    "Sexo", "FechaNacimiento", "Edad", "Ocupacion"
            };

            foreach (string campo in campos)
            {
                TableLayoutPanel campoLayout = new TableLayoutPanel();
                campoLayout.Width = 200;
                campoLayout.Height = 80;
                campoLayout.ColumnCount = 1;
                campoLayout.RowCount = 2;
                campoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                campoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                campoLayout.Margin = new Padding(5, 10, 5, 10);

                Label lbl = new Label();
                lbl.Text = campo + ":";
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                TextBox txt = new TextBox();
                txt.Name = "txt" + campo;
                txt.Dock = DockStyle.Fill;
                txt.Font = new Font("Segoe UI", 12);
                txt.ReadOnly = true;
                campoLayout.Controls.Add(lbl, 0, 0);
                campoLayout.Controls.Add(txt, 0, 1);

                panelHorizontal.Controls.Add(campoLayout);
            }

            // Agregar el panel horizontal en la columna central
            contenedorCentral.Controls.Add(panelHorizontal, 0, 0);

            // Agregar al layout principal en la fila 2
            layout.Controls.Add(contenedorCentral, 0, 2);

            btnLimpiar.Click += (s, e) =>
            {
                LimpiarCamposRegistro();

            };

            // Panel para centrar el botón
            FlowLayoutPanel panelBotonRegistrar = new FlowLayoutPanel();
            panelBotonRegistrar.Dock = DockStyle.Fill;
            panelBotonRegistrar.FlowDirection = FlowDirection.LeftToRight;
            panelBotonRegistrar.WrapContents = false;
            panelBotonRegistrar.AutoSize = false;
            panelBotonRegistrar.Padding = new Padding(0);
            panelBotonRegistrar.Margin = new Padding(0);
            panelBotonRegistrar.Anchor = AnchorStyles.None;
            panelBotonRegistrar.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Botón REGISTRAR
            Button btnRegistrar = new Button();
            btnRegistrar.Text = "REGISTRAR";
            btnRegistrar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRegistrar.Width = 200;
            btnRegistrar.Height = 45;
            btnRegistrar.TextAlign = ContentAlignment.MiddleCenter;
            btnRegistrar.Margin = new Padding(0, 10, 0, 10);
            btnRegistrar.BackColor = Color.SteelBlue;
            btnRegistrar.ForeColor = Color.White;
            btnRegistrar.FlatStyle = FlatStyle.Flat;
            btnRegistrar.FlatAppearance.BorderSize = 0;

            // Agregar botón al panel
            panelBotonRegistrar.Controls.Add(btnRegistrar);

            // Agregar panel al layout en la fila 3
            layout.Controls.Add(panelBotonRegistrar, 0, 3);

            btnBuscar.Click += (s, e) =>
            {
                string dni = txtBuscar.Text.Trim();

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
                        string query = @"SELECT * FROM PacienteBD WHERE Dni = @dni AND Estado = 1";

                        SqlCommand cmd = new SqlCommand(query, cn);
                        cmd.Parameters.AddWithValue("@dni", dni);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontró ningún paciente con ese DNI.");
                            return;
                        }

                        // Obtener la fila
                        DataRow row = dt.Rows[0];

                        // Lista de campos que creaste dinámicamente
                        string[] campos = new string[]
                        {
                "Apellidos", "Nombres", "Direccion", "Telefono", "Correo", "EstadoCivil",
                "Celular", "Instruccion", "Dni", "Departamento", "Provincia", "Distrito",
                "Sexo", "FechaNacimiento", "Edad", "Ocupacion"
                        };

                        // Buscar cada TextBox por su nombre y asignar el valor
                        foreach (string campo in campos)
                        {
                            Control[] controles = panelHorizontal.Controls.Find("txt" + campo, true);
                            if (controles.Length > 0 && controles[0] is TextBox txt)
                            {
                                object valor = row[campo];
                                txt.Text = valor != null ? valor.ToString() : "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al consultar la base de datos: " + ex.Message);
                    }
                }
            };
            btnRegistrar.Click += (s, e) =>
            {
                string dni = txtBuscar.Text.Trim();
                string motivoConsulta = cmbMotivo.Text.Trim();

                if (dni.Length != 8)
                {
                    MessageBox.Show("Ingrese un DNI válido de 8 dígitos.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(motivoConsulta))
                {
                    MessageBox.Show("Seleccione un motivo de consulta.");
                    return;
                }

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();

                        // 1. Validar que el DNI existe y obtener el IdPaciente
                        string queryPaciente = @"SELECT Id FROM PacienteBD WHERE Dni = @dni AND Estado = 1";
                        SqlCommand cmdPaciente = new SqlCommand(queryPaciente, cn);
                        cmdPaciente.Parameters.AddWithValue("@dni", dni);

                        object result = cmdPaciente.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("No se encontró ningún paciente con ese DNI.");
                            return;
                        }

                        int idPaciente = Convert.ToInt32(result);

                        // 2. Insertar en HistorialClinicoBD
                        string queryInsert = @"
                INSERT INTO HistorialClinicoBD (
    IdPaciente,
    FechaConsulta,
    MotivoConsulta,
    EstadoHistorialTurno
)
VALUES (
    @idPaciente,
    @fechaConsulta,
    @motivoConsulta,
    @estadoHistorialTurno
)
";

                        SqlCommand cmdInsert = new SqlCommand(queryInsert, cn);
                        DateTime ahora = DateTime.Now;
                        int estadoHistorialTurno = ahora.Hour < 12 ? 1 : 0;

                        cmdInsert.Parameters.AddWithValue("@idPaciente", idPaciente);
                        cmdInsert.Parameters.AddWithValue("@fechaConsulta", ahora);
                        cmdInsert.Parameters.AddWithValue("@motivoConsulta", motivoConsulta);
                        cmdInsert.Parameters.AddWithValue("@estadoHistorialTurno", estadoHistorialTurno);

                        int filas = cmdInsert.ExecuteNonQuery();
                        LimpiarCamposRegistro();

                        if (filas > 0)
                        {
                            MessageBox.Show("Historial clínico registrado correctamente.");
                        }
                        else
                        {
                            MessageBox.Show("No se pudo registrar el historial clínico.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al registrar: " + ex.Message);
                    }
                }

            };



        }



        private void FormGenerarHistorial_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Generar Historial Clínico";
            this.Icon = new Icon("Imagenes/log.ico");
        }
        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void LimpiarCamposRegistro()
        {

            string[] campos = new string[]
            {
        "Apellidos", "Nombres", "Direccion", "Telefono", "Correo", "EstadoCivil",
        "Celular", "Instruccion", "Dni", "Departamento", "Provincia", "Distrito",
        "Sexo", "FechaNacimiento", "Edad", "Ocupacion"
            };

            foreach (string campo in campos)
            {
                Control[] controles = panelHorizontal.Controls.Find("txt" + campo, true);
                if (controles.Length > 0 && controles[0] is TextBox txt)
                {
                    txt.Text = string.Empty;
                }
            }
            txtBuscar.Text = string.Empty;

        }


    }


}

