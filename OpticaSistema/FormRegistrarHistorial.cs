using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO; // Asegúrate de tener este using
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text; // Asegúrate de tener iTextSharp.dll referenciado en tu proyecto
using iTextSharp.text.pdf;
using Font = System.Drawing.Font;

namespace OpticaSistema
{
    public partial class FormRegistrarHistorial : Form
    {
        private Dictionary<string, string> datosPacienteBD = new Dictionary<string, string>();
        private ConexionDB conexionBD;
        private FlowLayoutPanel panelHorizontal;
        byte[] archivoPDF = null; // Declarado aquí para que sea accesible en todo el formulario
        string nombreArchivo = null; // Para guardar el nombre del archivo PDF

        public FormRegistrarHistorial()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            conexionBD = new ConexionDB();
            // string rutaPDFSeleccionado = null; // Ya no es necesaria si manejamos todo en memoria

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));   // Título ocupa 15% del alto
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));   // Filtro y botones
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));   // Campos
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 7));    // Botón REGISTRAR
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 3));    // Espacio inferior
            layout.Padding = new Padding(50, 20, 50, 20);
            this.Controls.Add(layout);


            Label titulo = new Label();
            titulo.Text = "REGISTRAR HISTORIAL CLINICO";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.DarkBlue;
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.Margin = new Padding(0, 30, 0, 0); // Más abajo
            layout.Controls.Add(titulo, 0, 0);

            layout.RowStyles[1] = new RowStyle(SizeType.AutoSize);
            // FlowLayoutPanel para centrar búsqueda
            FlowLayoutPanel panelBusqueda = new FlowLayoutPanel();
            panelBusqueda.FlowDirection = FlowDirection.LeftToRight;
            panelBusqueda.WrapContents = false;
            panelBusqueda.AutoSize = true;
            panelBusqueda.Padding = new Padding(0, 10, 0, 10);
            panelBusqueda.Dock = DockStyle.None;
            panelBusqueda.Anchor = AnchorStyles.None;
            panelBusqueda.Margin = new Padding(0, 10, 0, 10);


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
            TextBox txtBuscar = new TextBox();
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

            // Botón de búsqueda
            Button btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLimpiar.Width = 120;
            btnLimpiar.Height = 40;
            btnLimpiar.TextAlign = ContentAlignment.MiddleCenter;
            btnLimpiar.Margin = new Padding(5, 5, 10, 5);

            // Agregar controles al panel
            panelBusqueda.Controls.Add(lblBuscar);
            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(btnBuscar);
            panelBusqueda.Controls.Add(btnLimpiar);

            // Agregar al layout principal en la fila 1
            layout.Controls.Add(panelBusqueda, 0, 1);


            // Contenedor centrado con 3 columnas
            TableLayoutPanel contenedorCentral = new TableLayoutPanel();
            contenedorCentral.Dock = DockStyle.Fill;
            contenedorCentral.ColumnCount = 3;
            contenedorCentral.RowCount = 1;
            contenedorCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15)); // Izquierda
            contenedorCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Centro
            contenedorCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            // Contenedor que maneja el scroll y mantiene centrado
            Panel wrapper = new Panel();
            wrapper.Dock = DockStyle.Fill;
            wrapper.AutoScroll = true;   // El scroll vive aquí
            wrapper.Padding = new Padding(10);

            // FlowLayoutPanel dentro del wrapper
            panelHorizontal = new FlowLayoutPanel();
            panelHorizontal.FlowDirection = FlowDirection.TopDown;
            panelHorizontal.WrapContents = false;
            panelHorizontal.AutoSize = true;
            panelHorizontal.Anchor = AnchorStyles.Top;  // que se quede arriba centrado

            // IMPORTANTE: centrado manual
            panelHorizontal.Location = new Point(
                (wrapper.ClientSize.Width - panelHorizontal.Width) / 2,
                10
            );

            // Recalcular centrado cada vez que cambie tamaño
            wrapper.Resize += (s, e) =>
            {
                panelHorizontal.Left = (wrapper.ClientSize.Width - panelHorizontal.Width) / 2;
            };

            // Agregar flow al wrapper
            wrapper.Controls.Add(panelHorizontal);

            // Agregar a la columna central del contenedor
            contenedorCentral.Controls.Add(wrapper, 1, 0);


            // --- SECCIÓN 1: DATOS BÁSICOS Y FECHA (Organizado en un sub-panel horizontal) ---
            FlowLayoutPanel panelDatosBasicos = new FlowLayoutPanel();
            panelDatosBasicos.FlowDirection = FlowDirection.LeftToRight;
            panelDatosBasicos.AutoSize = true;
            panelDatosBasicos.WrapContents = true;
            panelDatosBasicos.Margin = new Padding(0);

            // Campos de la BD (Paciente)
            string[] camposPaciente = new string[]
            {
                "Dni", "Apellidos", "Nombres"
            };

            foreach (string campo in camposPaciente)
            {
                TableLayoutPanel campoLayout = CrearCampoLayout(campo, 250, 80, true);
                panelDatosBasicos.Controls.Add(campoLayout);
            }

            // --- NUEVOS CAMPOS DEL HISTORIAL ---

            // Sub-panel para agrupar Fecha y Motivo en la misma fila
            FlowLayoutPanel panelFechaMotivo = new FlowLayoutPanel();
            panelFechaMotivo.FlowDirection = FlowDirection.LeftToRight;
            panelFechaMotivo.AutoSize = true;
            panelFechaMotivo.WrapContents = false;
            panelFechaMotivo.Margin = new Padding(0);

            // 1. Fecha de Consulta
            TableLayoutPanel fechaLayout = CrearCampoLayout("FechaConsulta", 250, 80, false);
            Control lblFecha = fechaLayout.Controls[0];
            lblFecha.Text = "Fecha Consulta:";

            DateTimePicker dtpFechaConsulta = new DateTimePicker();
            dtpFechaConsulta.Name = "dtpFechaConsulta";
            dtpFechaConsulta.Dock = DockStyle.Fill;
            dtpFechaConsulta.Font = new Font("Segoe UI", 12);
            dtpFechaConsulta.Value = DateTime.Now; // Valor inicial
            dtpFechaConsulta.Format = DateTimePickerFormat.Short; // Formato de fecha corta
            fechaLayout.Controls.Add(dtpFechaConsulta, 0, 1);


            // 2. Motivo de Consulta
            TableLayoutPanel motivoLayout = CrearCampoLayout("MotivoConsulta", 400, 80, false);
            Control lblMotivo = motivoLayout.Controls[0];
            lblMotivo.Text = "Motivo de Consulta:";

            // ComboBox en lugar de TextBox
            ComboBox cmbMotivo = new ComboBox();
            cmbMotivo.Name = "cmbMotivoConsulta";
            cmbMotivo.Dock = DockStyle.Fill;
            cmbMotivo.Font = new Font("Segoe UI", 12);
            cmbMotivo.DropDownStyle = ComboBoxStyle.DropDownList;

            // Opciones del selector (puedes modificarlas según tus necesidades)
            cmbMotivo.Items.AddRange(new string[]
            {
                    "Exámen Ocular Completo",
                    "Medida de la Vista",
                    "Consulta Oftalmológica",
                    "Consulta con Retinólogo"
            });

            // Agregar al layout
            motivoLayout.Controls.Add(cmbMotivo, 0, 1);

            panelHorizontal.Controls.Add(panelDatosBasicos);
            panelFechaMotivo.Controls.Add(fechaLayout);
            panelFechaMotivo.Controls.Add(motivoLayout);
            panelHorizontal.Controls.Add(panelFechaMotivo);


            // 3. Tabla de Correctores (Receta)
            panelHorizontal.Controls.Add(CrearPanelCorrectores());

            // === OBSERVACIONES ===
            TableLayoutPanel observacionesLayout = CrearCampoLayout("Observaciones", 820, 120, false);
            Control lblObservaciones = observacionesLayout.Controls[0];
            lblObservaciones.Text = "OBSERVACIONES:";

            TextBox txtObservaciones = new TextBox();
            txtObservaciones.Name = "txtObservaciones1";
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Font = new Font("Segoe UI", 12);
            txtObservaciones.Multiline = true;
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Height = 100;
            observacionesLayout.Controls.Add(txtObservaciones, 0, 1);


            // === SIGNOS Y SÍNTOMAS ===
            TableLayoutPanel signosLayout = CrearCampoLayout("SignosSintomas", 820, 120, false);
            Control lblSignos = signosLayout.Controls[0];
            lblSignos.Text = "SIGNOS Y SÍNTOMAS:";

            TextBox txtSignos = new TextBox();
            txtSignos.Name = "txtSignosSintomas1";
            txtSignos.Dock = DockStyle.Fill;
            txtSignos.Font = new Font("Segoe UI", 12);
            txtSignos.Multiline = true;
            txtSignos.ScrollBars = ScrollBars.Vertical;
            txtSignos.Height = 100;
            signosLayout.Controls.Add(txtSignos, 0, 1);

            // === EXAMEN OFTALMOLÓGICO ===
            FlowLayoutPanel panelExamenTitulo = new FlowLayoutPanel();
            panelExamenTitulo.FlowDirection = FlowDirection.LeftToRight;
            panelExamenTitulo.AutoSize = true;
            panelExamenTitulo.Margin = new Padding(10, 10, 10, 0);
            panelExamenTitulo.Width = 820;

            Label lblExamen = new Label();
            lblExamen.Text = "EXAMEN OFTALMOLÓGICO:";
            lblExamen.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblExamen.TextAlign = ContentAlignment.MiddleLeft;
            lblExamen.AutoSize = true;

            TextBox txtExamenTitulo = new TextBox();
            txtExamenTitulo.Name = "txtDoctorExamenOftalmologico";
            txtExamenTitulo.Width = 250;
            txtExamenTitulo.Font = new Font("Segoe UI", 12);
            txtExamenTitulo.Margin = new Padding(20, 0, 0, 0);

            panelExamenTitulo.Controls.Add(lblExamen);
            panelExamenTitulo.Controls.Add(txtExamenTitulo);

            // Crear el área de texto grande (sin título dentro)
            TableLayoutPanel examenLayout = new TableLayoutPanel();
            examenLayout.Width = 820;
            examenLayout.Height = 120;
            examenLayout.ColumnCount = 1;
            examenLayout.RowCount = 1;
            examenLayout.Margin = new Padding(10, 0, 10, 0);

            TextBox txtExamen = new TextBox();
            txtExamen.Name = "txtExamenOftalmologico";
            txtExamen.Dock = DockStyle.Fill;
            txtExamen.Font = new Font("Segoe UI", 12);
            txtExamen.Multiline = true;
            txtExamen.ScrollBars = ScrollBars.Vertical;
            txtExamen.Height = 100;

            examenLayout.Controls.Add(txtExamen, 0, 0);


            // === PANEL DE DIBUJO DE OJOS ===
            FlowLayoutPanel panelOjos = new FlowLayoutPanel();
            panelOjos.FlowDirection = FlowDirection.LeftToRight;
            panelOjos.Width = 820;

            panelOjos.AutoSize = true;
            panelOjos.Margin = new Padding(10, 20, 10, 10);
            panelOjos.WrapContents = false;

            panelOjos.Controls.Add(CrearPanelDibujoOjo("Ojo Derecho"));
            panelOjos.Controls.Add(CrearPanelDibujoOjo("Ojo Izquierdo"));



            // === TRATAMIENTO ===
            TableLayoutPanel tratamientoLayout = CrearCampoLayout("Tratamiento", 820, 120, false);
            Control lblTratamiento = tratamientoLayout.Controls[0];
            lblTratamiento.Text = "TRATAMIENTO:";

            TextBox txtTratamiento = new TextBox();
            txtTratamiento.Name = "txtTratamiento1";
            txtTratamiento.Dock = DockStyle.Fill;
            txtTratamiento.Font = new Font("Segoe UI", 12);
            txtTratamiento.Multiline = true;
            txtTratamiento.ScrollBars = ScrollBars.Vertical;
            txtTratamiento.Height = 100;
            tratamientoLayout.Controls.Add(txtTratamiento, 0, 1);

            // === SECCIÓN ADJUNTAR ANÁLISIS ===
            FlowLayoutPanel panelAdjuntar = new FlowLayoutPanel();
            panelAdjuntar.FlowDirection = FlowDirection.TopDown;
            panelAdjuntar.AutoSize = true;
            panelAdjuntar.Width = 820;
            panelAdjuntar.Margin = new Padding(10, 10, 10, 20);

            // Título
            Label lblAdjuntar = new Label();
            lblAdjuntar.Text = "ADJUNTAR ANÁLISIS:";
            lblAdjuntar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblAdjuntar.TextAlign = ContentAlignment.MiddleLeft;
            lblAdjuntar.AutoSize = true;
            lblAdjuntar.Margin = new Padding(0, 0, 0, 5);

            //Nombre del archivo
            Label lblNombreArchivo = new Label();
            lblNombreArchivo.Text = "Ningún archivo seleccionado";
            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblNombreArchivo.ForeColor = Color.DimGray;
            lblNombreArchivo.AutoSize = true;
            lblNombreArchivo.Margin = new Padding(0, 0, 0, 10);

            // Botón para subir PDF
            Button btnSubirPDF = new Button();
            btnSubirPDF.Text = "Subir PDF";
            btnSubirPDF.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSubirPDF.Width = 160;
            btnSubirPDF.Height = 40;
            btnSubirPDF.BackColor = Color.SteelBlue;
            btnSubirPDF.ForeColor = Color.White;
            btnSubirPDF.FlatStyle = FlatStyle.Flat;
            btnSubirPDF.FlatAppearance.BorderSize = 0;

            // La variable archivoPDF ya está declarada a nivel de clase.
            // La variable nombreArchivo también está declarada a nivel de clase.


            // Evento para seleccionar el archivo
            btnSubirPDF.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Archivos PDF (*.pdf)|*.pdf";
                    ofd.Title = "Seleccionar análisis en PDF";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string rutaPDF = ofd.FileName;
                        string nombreArchivoPDF = Path.GetFileName(rutaPDF);

                        try
                        {
                            archivoPDF = File.ReadAllBytes(rutaPDF); // Lee el archivo en bytes
                            nombreArchivo = nombreArchivoPDF; // Guarda solo el nombre

                            lblNombreArchivo.Text = nombreArchivoPDF;
                            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                            lblNombreArchivo.ForeColor = Color.Black;

                            MessageBox.Show("Archivo cargado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al leer el archivo: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            archivoPDF = null;
                            nombreArchivo = null;

                            lblNombreArchivo.Text = "Ningún archivo seleccionado";
                            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                            lblNombreArchivo.ForeColor = Color.DimGray;
                        }
                    }
                }
            };

            // Agregar controles al panel
            panelAdjuntar.Controls.Add(lblAdjuntar);
            panelAdjuntar.Controls.Add(lblNombreArchivo);
            panelAdjuntar.Controls.Add(btnSubirPDF);

            panelHorizontal.Controls.Add(observacionesLayout);
            panelHorizontal.Controls.Add(panelExamenTitulo);
            panelHorizontal.Controls.Add(examenLayout);

            panelHorizontal.Controls.Add(signosLayout);
            panelHorizontal.Controls.Add(panelOjos);
            panelHorizontal.Controls.Add(CrearPanelDiagnostico());
            panelHorizontal.Controls.Add(tratamientoLayout);
            panelHorizontal.Controls.Add(panelAdjuntar);
            // --- FIN NUEVOS CAMPOS ---

            // Agregar el panel horizontal en la columna central
            //contenedorCentral.Controls.Add(panelHorizontal, 1, 0);

            // Agregar al layout principal en la fila 2
            layout.Controls.Add(contenedorCentral, 0, 2);

            btnLimpiar.Click += (s, e) =>
            {
                LimpiarCamposRegistro();
                txtBuscar.Text = string.Empty; // Limpiar también el DNI de búsqueda
                // También limpiar los datos del PDF adjunto
                archivoPDF = null;
                nombreArchivo = null;
                lblNombreArchivo.Text = "Ningún archivo seleccionado";
                lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                lblNombreArchivo.ForeColor = Color.DimGray;
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
            btnRegistrar.Click += (s, e) =>
            {
                string dni = txtBuscar.Text.Trim();

                // 🔹 Validar DNI
                if (dni.Length != 8)
                {
                    MessageBox.Show("Ingrese un DNI válido de 8 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔹 Motivo de consulta
                string motivoConsulta = null;
                Control[] ctrlMotivo = panelHorizontal.Controls.Find("cmbMotivoConsulta", true);
                if (ctrlMotivo.Length > 0 && ctrlMotivo[0] is ComboBox cmbMotivo)
                {
                    if (cmbMotivo.SelectedItem != null && !string.IsNullOrWhiteSpace(cmbMotivo.SelectedItem.ToString()))
                        motivoConsulta = cmbMotivo.SelectedItem.ToString();
                }
                if (string.IsNullOrEmpty(motivoConsulta))
                {
                    MessageBox.Show("Debe seleccionar un motivo de consulta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔹 Fecha de consulta
                DateTime fechaConsulta = DateTime.Now;
                Control[] ctrlFecha = panelHorizontal.Controls.Find("dtpFechaConsulta", true);
                if (ctrlFecha.Length > 0 && ctrlFecha[0] is DateTimePicker dtpFecha)
                    fechaConsulta = dtpFecha.Value;

                // 🔹 Turno
                int estadoTurno = (DateTime.Now.Hour < 14) ? 1 : 0;

                using (SqlConnection cn = conexionBD.Conectar())
                {
                    try
                    {
                        cn.Open();

                        // 🔹 Buscar IdPaciente
                        int idPaciente;
                        using (SqlCommand cmdPaciente = new SqlCommand("SELECT Id FROM PacienteBD WHERE Dni = @dni AND Estado = 1", cn))
                        {
                            cmdPaciente.Parameters.AddWithValue("@dni", dni);
                            object result = cmdPaciente.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("El paciente no existe.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            idPaciente = Convert.ToInt32(result);
                        }

                        // 🔹 Helper functions
                        object GetTextBoxValue(string nombre)
                        {
                            Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                            if (ctrls.Length > 0 && ctrls[0] is TextBox txt)
                                return string.IsNullOrWhiteSpace(txt.Text) ? DBNull.Value : txt.Text.Trim();
                            return DBNull.Value;
                        }

                        object GetDecimalValue(string nombre)
                        {
                            Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                            if (ctrls.Length > 0 && ctrls[0] is TextBox txt)
                            {
                                if (decimal.TryParse(txt.Text.Trim(), out decimal val))
                                    return val;
                            }
                            return DBNull.Value;
                        }

                        object GetImageValue(string nombre)
                        {
                            Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                            if (ctrls.Length > 0 && ctrls[0] is FlowLayoutPanel panel)
                            {
                                PictureBox pic = panel.Controls.OfType<PictureBox>().FirstOrDefault();
                                if (pic?.Image != null)
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        pic.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        return ms.ToArray();
                                    }
                                }
                            }
                            return DBNull.Value;
                        }

                        object GetDateValue(string nombre)
                        {
                            Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                            if (ctrls.Length > 0 && ctrls[0] is DateTimePicker dtp)
                                return dtp.Value.Date;
                            return DBNull.Value;
                        }

                        object GetTimeValue(string nombre)
                        {
                            Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                            if (ctrls.Length > 0 && ctrls[0] is DateTimePicker dtp)
                                return dtp.Value.TimeOfDay;
                            return DBNull.Value;
                        }

                        // 🔹 Diccionario solo para campos que antes daban null
                        Dictionary<string, string> mapaColumnasR = new Dictionary<string, string>()
                        {
                            { "txtAVSC_OD", "AV_SC_OD" },
                            { "txtAVSC_OI", "AV_SC_OI" },
                            { "txtAVCC_OD", "AV_CC_OD" },
                            { "txtAVCC_OI", "AV_CC_OI" },
                            { "txtPIOICARE_OD", "PIO_OD" },
                            { "txtPIOICARE_OI", "PIO_OI" }
                        };

                        // 🔹 Comando INSERT completo
                        string sqlInsert = @"
INSERT INTO HistorialClinicoBD
(
    IdPaciente, FechaConsulta, MotivoConsulta, EstadoHistorial, EstadoHistorialTurno,
    Nombre_optometra, Lejos_OD_Esferico, Lejos_OD_Cilindrico, Lejos_OD_EJE, Lejos_OD_DIP, Lejos_OD_AV,
    Lejos_OI_Esferico, Lejos_OI_Cilindrico, Lejos_OI_EJE, Lejos_OI_DIP, Lejos_OI_AV,
    Cerca_OD_Esferico, Cerca_OD_Cilindrico, Cerca_OD_EJE, Cerca_OD_DIP, Cerca_OD_AV,
    Cerca_OI_Esferico, Cerca_OI_Cilindrico, Cerca_OI_EJE, Cerca_OI_DIP, Cerca_OI_AV,
    Observaciones,
    Nombre_oftalmologo, SignosSintomas, ExamenOftamologico, OjoDerecho, OjoIzquierdo,
    Nombre_retinologo, Diagnostico, AV_SC_OD, AV_SC_OI, AV_CC_OD, AV_CC_OI,
    PIO_OD, PIO_OI, Fecha_Diagnostico, Hora_Inicio, Hora_Termino,
    Tratamiento, NombreArchivo, Archivo, PDFHistorialClinico
)
VALUES
(
    @IdPaciente, @FechaConsulta, @MotivoConsulta, @EstadoHistorial, @EstadoHistorialTurno,
    @Nombre_optometra, @Lejos_OD_Esferico, @Lejos_OD_Cilindrico, @Lejos_OD_EJE, @Lejos_OD_DIP, @Lejos_OD_AV,
    @Lejos_OI_Esferico, @Lejos_OI_Cilindrico, @Lejos_OI_EJE, @Lejos_OI_DIP, @Lejos_OI_AV,
    @Cerca_OD_Esferico, @Cerca_OD_Cilindrico, @Cerca_OD_EJE, @Cerca_OD_DIP, @Cerca_OD_AV,
    @Cerca_OI_Esferico, @Cerca_OI_Cilindrico, @Cerca_OI_EJE, @Cerca_OI_DIP, @Cerca_OI_AV,
    @Observaciones,
    @Nombre_oftalmologo, @SignosSintomas, @ExamenOftamologico, @OjoDerecho, @OjoIzquierdo,
    @Nombre_retinologo, @Diagnostico, @AV_SC_OD, @AV_SC_OI, @AV_CC_OD, @AV_CC_OI,
    @PIO_OD, @PIO_OI, @Fecha_Diagnostico, @Hora_Inicio, @Hora_Termino,
    @Tratamiento, @NombreArchivo, @Archivo, @PDFHistorialClinico
)";

                        using (SqlCommand cmd = new SqlCommand(sqlInsert, cn))
                        {
                            // 🔹 Parámetros básicos
                            cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                            cmd.Parameters.Add("@FechaConsulta", SqlDbType.DateTime).Value = fechaConsulta;
                            cmd.Parameters.Add("@MotivoConsulta", SqlDbType.NVarChar, 100).Value = motivoConsulta;
                            cmd.Parameters.Add("@EstadoHistorial", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.Add("@EstadoHistorialTurno", SqlDbType.Bit).Value = estadoTurno;

                            // 🔹 Medidas de la vista y demás campos (igual que tu código original)
                            cmd.Parameters.Add("@Nombre_optometra", SqlDbType.NVarChar, 400).Value = GetTextBoxValue("txtOptometro");
                            cmd.Parameters.Add("@Lejos_OD_Esferico", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSODEsferico");
                            cmd.Parameters.Add("@Lejos_OD_Cilindrico", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSODCilindrico");
                            cmd.Parameters.Add("@Lejos_OD_EJE", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSODEje");
                            cmd.Parameters.Add("@Lejos_OD_DIP", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSODDIP");
                            cmd.Parameters.Add("@Lejos_OD_AV", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSODAgudezaVisual");

                            cmd.Parameters.Add("@Lejos_OI_Esferico", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSOIEsferico");
                            cmd.Parameters.Add("@Lejos_OI_Cilindrico", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSOICilindrico");
                            cmd.Parameters.Add("@Lejos_OI_EJE", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSOIEje");
                            cmd.Parameters.Add("@Lejos_OI_DIP", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSOIDIP");
                            cmd.Parameters.Add("@Lejos_OI_AV", SqlDbType.Decimal).Value = GetDecimalValue("txtLEJOSOIAgudezaVisual");

                            cmd.Parameters.Add("@Cerca_OD_Esferico", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODEsferico");
                            cmd.Parameters.Add("@Cerca_OD_Cilindrico", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODCilindrico");
                            cmd.Parameters.Add("@Cerca_OD_EJE", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODEje");
                            cmd.Parameters.Add("@Cerca_OD_DIP", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODDIP");
                            cmd.Parameters.Add("@Cerca_OD_AV", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODAgudezaVisual");

                            cmd.Parameters.Add("@Cerca_OI_Esferico", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAOIEsferico");
                            cmd.Parameters.Add("@Cerca_OI_Cilindrico", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAOICilindrico");
                            cmd.Parameters.Add("@Cerca_OI_EJE", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAOIEje");
                            cmd.Parameters.Add("@Cerca_OI_DIP", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAODDIP");
                            cmd.Parameters.Add("@Cerca_OI_AV", SqlDbType.Decimal).Value = GetDecimalValue("txtCERCAOIAgudezaVisual");

                            cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar).Value = GetTextBoxValue("txtObservaciones1");

                            cmd.Parameters.Add("@Nombre_oftalmologo", SqlDbType.NVarChar, 400).Value = GetTextBoxValue("txtDoctorExamenOftalmologico");
                            cmd.Parameters.Add("@SignosSintomas", SqlDbType.VarChar).Value = GetTextBoxValue("txtSignosSintomas1");
                            cmd.Parameters.Add("@ExamenOftamologico", SqlDbType.NVarChar).Value = GetTextBoxValue("txtExamenOftalmologico");
                            cmd.Parameters.Add("@OjoDerecho", SqlDbType.VarBinary).Value = GetImageValue("panelOjoDerecho");
                            cmd.Parameters.Add("@OjoIzquierdo", SqlDbType.VarBinary).Value = GetImageValue("panelOjoIzquierdo");

                            cmd.Parameters.Add("@Nombre_retinologo", SqlDbType.NVarChar, 400).Value = GetTextBoxValue("txtDoctorDiagnostico");
                            cmd.Parameters.Add("@Diagnostico", SqlDbType.NVarChar).Value = GetTextBoxValue("txtObservacionesDiagnostico");

                            // 🔹 Campos retinólogo usando foreach + mapaColumnasR
                            foreach (var kvp in mapaColumnasR)
                            {
                                string nombreControl = kvp.Key;
                                string parametroBD = kvp.Value;

                                cmd.Parameters.Add($"@{parametroBD}", SqlDbType.VarChar, 50).Value = GetTextBoxValue(nombreControl);
                            }

                            cmd.Parameters.Add("@Fecha_Diagnostico", SqlDbType.Date).Value = GetDateValue("dtpFechaDiagnostico");
                            cmd.Parameters.Add("@Hora_Inicio", SqlDbType.Time).Value = GetTimeValue("dtpHoraInicio");
                            cmd.Parameters.Add("@Hora_Termino", SqlDbType.Time).Value = GetTimeValue("dtpHoraTermino");
                            cmd.Parameters.Add("@Tratamiento", SqlDbType.NVarChar).Value = GetTextBoxValue("txtTratamiento1");

                            // 🔹 Nombre del archivo adjunto
                            cmd.Parameters.Add("@NombreArchivo", SqlDbType.NVarChar).Value =
                                string.IsNullOrWhiteSpace(lblNombreArchivo.Text) || lblNombreArchivo.Text == "Ningún archivo seleccionado"
                                ? DBNull.Value
                                : lblNombreArchivo.Text;

                            // 🔹 Archivo PDF adjunto original (si existe)
                            if (archivoPDF != null)
                                cmd.Parameters.Add("@Archivo", SqlDbType.VarBinary).Value = archivoPDF;
                            else
                                cmd.Parameters.Add("@Archivo", SqlDbType.VarBinary).Value = DBNull.Value;


                            // ======================= NUEVA SECCIÓN PARA PDF EN BD ==========================

                            string signosSintomas = GetTextBoxValue("txtSignosSintomas1")?.ToString() ?? "";
                            string drexamenOftalmologico = GetTextBoxValue("txtDoctorExamenOftalmologico")?.ToString() ?? "";
                            string examenOftalmologico = GetTextBoxValue("txtExamenOftalmologico")?.ToString() ?? "";
                            string tratamiento = GetTextBoxValue("txtTratamiento1")?.ToString() ?? "";
                            string observaciones = GetTextBoxValue("txtObservaciones1")?.ToString() ?? "";
                            string observacionesdiagnostico = GetTextBoxValue("txtObservacionesDiagnostico")?.ToString() ?? "";
                            string optometra = GetTextBoxValue("txtOptometro")?.ToString() ?? "";
                            string doctorDiagnostico = GetTextBoxValue("txtDoctorDiagnostico")?.ToString() ?? "";

                            // Captura los dibujos de los ojos (si existen)
                            Bitmap dibujoOjoDerecho = ObtenerImagenDesdePanel("panelOjoDerecho");
                            Bitmap dibujoOjoIzquierdo = ObtenerImagenDesdePanel("panelOjoIzquierdo");

                            // ==== DATOS PRINCIPALES ====
                            var datosParaImagen = new
                            {
                                Dni = datosPacienteBD.ContainsKey("Dni") ? datosPacienteBD["Dni"] : "",
                                Apellidos = datosPacienteBD.ContainsKey("Apellidos") ? datosPacienteBD["Apellidos"] : "",
                                Nombres = datosPacienteBD.ContainsKey("Nombres") ? datosPacienteBD["Nombres"] : "",
                                FechaConsulta = fechaConsulta.ToShortDateString(),
                                MotivoConsulta = motivoConsulta,
                                ObservacionesDiagnostico = observacionesdiagnostico,
                                Observaciones = observaciones,
                                DRExamenOftalmologico = drexamenOftalmologico,
                                ExamenOftalmologico = examenOftalmologico,
                                SignosSintomas = signosSintomas,
                                Tratamiento = tratamiento,
                                Optometro = optometra,
                                DoctorDiagnostico = doctorDiagnostico,
                            };

                            // ==== RECETA VISUAL ====
                            Dictionary<string, string> recetaVisual = new Dictionary<string, string>();
                            string[] ojosReceta = { "OD", "OI" };
                            string[] tiposReceta = { "LEJOS", "CERCA" };
                            string[] camposReceta = { "Esferico", "Cilindrico", "Eje", "DIP", "AgudezaVisual" };

                            foreach (var tipo in tiposReceta)
                            {
                                foreach (var ojo in ojosReceta)
                                {
                                    foreach (var campo in camposReceta)
                                    {
                                        string nombre = $"txt{tipo}{ojo}{campo}";
                                        recetaVisual[$"{tipo}_{ojo}_{campo}"] = GetTextBoxValue(nombre)?.ToString() ?? "";
                                    }
                                }
                            }

                            Dictionary<string, string> datosDiagnosticoDic = new Dictionary<string, string>();

                            string[] diagnosticos = { "AVSC", "AVCC", "PIOICARE" };

                            foreach (string campo in diagnosticos)
                            {
                                string valorOD = GetTextBoxValue($"txt{campo}_OD")?.ToString() ?? "";
                                string valorOI = GetTextBoxValue($"txt{campo}_OI")?.ToString() ?? "";

                                datosDiagnosticoDic[$"{campo}_OD"] = valorOD;
                                datosDiagnosticoDic[$"{campo}_OI"] = valorOI;
                            }


                            // Fecha y horas del diagnóstico
                            var dtpFechaDiag = panelHorizontal.Controls.Find("dtpFechaDiagnostico", true).FirstOrDefault() as DateTimePicker;
                            if (dtpFechaDiag != null)
                                datosDiagnosticoDic["FECHA"] = dtpFechaDiag.Value.ToShortDateString();

                            var dtpInicio = panelHorizontal.Controls.Find("dtpHoraInicio", true).FirstOrDefault() as DateTimePicker;
                            if (dtpInicio != null)
                                datosDiagnosticoDic["HORA DE INICIO"] = dtpInicio.Value.ToShortTimeString();

                            var dtpFin = panelHorizontal.Controls.Find("dtpHoraTermino", true).FirstOrDefault() as DateTimePicker;
                            if (dtpFin != null)
                                datosDiagnosticoDic["HORA DE TÉRMINO"] = dtpFin.Value.ToShortTimeString();


                            // ==== GENERAR LA IMAGEN EN MEMORIA ====
                            byte[] imagenHistorialBytes = GenerarImagenConDatos(datosParaImagen, datosPacienteBD, recetaVisual, datosDiagnosticoDic, dibujoOjoDerecho, dibujoOjoIzquierdo);

                            byte[] pdfFinalHistorial = null;

                            if (imagenHistorialBytes != null && imagenHistorialBytes.Length > 0)
                            {
                                if (archivoPDF != null && archivoPDF.Length > 0)
                                {
                                    // Si hay un PDF adjunto, unir la imagen con el PDF
                                    pdfFinalHistorial = UnirImagenConPDFsEnMemoria(imagenHistorialBytes, archivoPDF);
                                }
                                else
                                {
                                    // Si no hay PDF adjunto, solo convertir la imagen a PDF
                                    pdfFinalHistorial = ConvertirImagenBytesAPDFBytes(imagenHistorialBytes);
                                }
                            }

                            if (pdfFinalHistorial != null && pdfFinalHistorial.Length > 0)
                            {
                                cmd.Parameters.Add("@PDFHistorialClinico", SqlDbType.VarBinary).Value = pdfFinalHistorial;
                            }
                            else
                            {
                                cmd.Parameters.Add("@PDFHistorialClinico", SqlDbType.VarBinary).Value = DBNull.Value;
                            }

                            // ======================= FIN NUEVA SECCIÓN ==========================

                            // 🔹 Ejecutar
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Historial clínico registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LimpiarControles(panelHorizontal);
                            LimpiarCamposRegistro();
                            txtBuscar.Text = string.Empty;

                            // Limpiar los paneles de dibujo de ojos
                            FlowLayoutPanel panelOjoDerecho = this.Controls.Find("panelOjoDerecho", true).FirstOrDefault() as FlowLayoutPanel;
                            FlowLayoutPanel panelOjoIzquierdo = this.Controls.Find("panelOjoIzquierdo", true).FirstOrDefault() as FlowLayoutPanel;

                            if (panelOjoDerecho != null) LimpiarOjo(panelOjoDerecho);
                            if (panelOjoIzquierdo != null) LimpiarOjo(panelOjoIzquierdo);

                            // Limpiar también los datos del PDF adjunto después de registrar
                            archivoPDF = null;
                            nombreArchivo = null;
                            lblNombreArchivo.Text = "Ningún archivo seleccionado";
                            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                            lblNombreArchivo.ForeColor = Color.DimGray;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al registrar el historial: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            };



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

                        // Lista de campos de paciente
                        string[] campos = new string[]
                        {
                           "Apellidos", "Nombres", "Dni","Direccion", "Telefono", "Correo", "EstadoCivil",
                           "Celular", "Instruccion", "Dni", "Departamento", "Provincia", "Distrito",
                           "Sexo", "FechaNacimiento", "Edad", "Ocupacion"
                        };

                        datosPacienteBD.Clear();
                        foreach (string campo in campos.Distinct())
                        {
                            string valor = row.Table.Columns.Contains(campo) ? row[campo]?.ToString() ?? "" : "";
                            datosPacienteBD[campo] = valor;

                            // Asignar al TextBox si existe
                            Control[] controles = panelHorizontal.Controls.Find("txt" + campo, true);
                            if (controles.Length > 0 && controles[0] is TextBox txt)
                            {
                                txt.Text = valor;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al consultar la base de datos: " + ex.Message);
                    }
                }
            };

        }

        private Control CrearPanelDibujoOjo(string titulo)
        {
            // --- Panel principal ---
            FlowLayoutPanel panelOjo = new FlowLayoutPanel();
            panelOjo.FlowDirection = FlowDirection.TopDown;
            panelOjo.Width = 390;
            panelOjo.AutoSize = true;
            panelOjo.Margin = new Padding(10);
            panelOjo.Name = titulo.Contains("Derecho") ? "panelOjoDerecho" : "panelOjoIzquierdo";

            // --- Etiqueta ---
            Label lblOjo = new Label();
            lblOjo.Text = titulo;
            lblOjo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblOjo.TextAlign = ContentAlignment.MiddleCenter;
            lblOjo.Dock = DockStyle.Top;
            lblOjo.Width = 390;
            panelOjo.Controls.Add(lblOjo);

            // --- Imagen base ---
            PictureBox picOjo = new PictureBox();
            picOjo.Width = 360;
            picOjo.Height = 240;
            picOjo.BorderStyle = BorderStyle.FixedSingle;
            picOjo.BackColor = Color.White;
            picOjo.SizeMode = PictureBoxSizeMode.StretchImage;

            string ruta = Path.Combine(Application.StartupPath, "Imagenes",
                titulo.Contains("Derecho") ? "derecho.png" : "izquierdo.png");

            Bitmap imagenBase;
            if (File.Exists(ruta))
                imagenBase = new Bitmap(System.Drawing.Image.FromFile(ruta), picOjo.Size);
            else
            {
                imagenBase = new Bitmap(picOjo.Width, picOjo.Height);
                using (Graphics g = Graphics.FromImage(imagenBase))
                {
                    g.Clear(Color.White);
                    g.DrawString("Imagen no encontrada",
                        new Font("Segoe UI", 10, FontStyle.Italic),
                        Brushes.Gray, new PointF(70, 110));
                }
            }

            // --- Capa de dibujo ---
            Bitmap capaDibujo = new Bitmap(picOjo.Width, picOjo.Height);
            picOjo.Image = Combinar(imagenBase, capaDibujo);

            // --- Variables de dibujo ---
            bool dibujando = false;
            Point puntoPrevio = Point.Empty;

            picOjo.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    dibujando = true;
                    puntoPrevio = e.Location;
                }
            };

            picOjo.MouseMove += (s, e) =>
            {
                if (dibujando)
                {
                    using (Graphics g = Graphics.FromImage(capaDibujo))
                    {
                        Pen lapiz = new Pen(Color.Red, 2);
                        g.DrawLine(lapiz, puntoPrevio, e.Location);
                    }

                    picOjo.Image = Combinar(imagenBase, capaDibujo);
                    puntoPrevio = e.Location;
                }
            };

            picOjo.MouseUp += (s, e) => dibujando = false;

            // --- Botón limpiar ---
            Button btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar dibujo";
            btnLimpiar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLimpiar.Width = 160;
            btnLimpiar.Height = 40;
            btnLimpiar.Margin = new Padding(5);
            btnLimpiar.BackColor = Color.LightSteelBlue;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.FlatAppearance.BorderSize = 1;

            btnLimpiar.Click += (s, e) =>
            {
                capaDibujo = new Bitmap(picOjo.Width, picOjo.Height);
                picOjo.Image = Combinar(imagenBase, capaDibujo);
            };

            // --- Panel botones ---
            FlowLayoutPanel panelBotones = new FlowLayoutPanel();
            panelBotones.FlowDirection = FlowDirection.LeftToRight;
            panelBotones.AutoSize = true;
            panelBotones.Margin = new Padding(0, 8, 0, 0);
            panelBotones.Controls.Add(btnLimpiar);

            panelOjo.Controls.Add(picOjo);
            panelOjo.Controls.Add(panelBotones);
            picOjo.Tag = Tuple.Create(imagenBase, capaDibujo);
            return panelOjo;
        }
        void LimpiarOjo(FlowLayoutPanel panelOjo)
        {
            PictureBox picOjo = panelOjo.Controls.OfType<PictureBox>().FirstOrDefault();
            if (picOjo != null && picOjo.Tag is Tuple<Bitmap, Bitmap> tupla)
            {
                Bitmap baseImg = tupla.Item1;
                Bitmap capa = tupla.Item2;

                // Limpiar la capa de dibujo
                using (Graphics g = Graphics.FromImage(capa))
                    g.Clear(Color.Transparent);

                // Combinar con la imagen base
                picOjo.Image = Combinar(baseImg, capa);
            }
        }
        // --- Función auxiliar para combinar dos bitmaps ---
        private Bitmap Combinar(Bitmap baseImg, Bitmap capa)
        {
            Bitmap resultado = new Bitmap(baseImg.Width, baseImg.Height);
            using (Graphics g = Graphics.FromImage(resultado))
            {
                g.DrawImage(baseImg, 0, 0);
                g.DrawImage(capa, 0, 0);
            }
            return resultado;
        }

        // --- MÉTODOS AUXILIARES ---

        // Método para crear el layout estándar de Etiqueta + Control
        private TableLayoutPanel CrearCampoLayout(string nombreCampo, int ancho, int alto, bool esReadOnly)
        {
            TableLayoutPanel campoLayout = new TableLayoutPanel();
            campoLayout.Width = ancho;
            campoLayout.Height = alto;
            campoLayout.ColumnCount = 1;
            campoLayout.RowCount = 2;
            campoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            campoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            campoLayout.Margin = new Padding(10);

            Label lbl = new Label();
            lbl.Text = (nombreCampo == "Dni" ? "DNI" : nombreCampo) + ":";
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            TextBox txt = new TextBox();
            txt.Name = "txt" + nombreCampo;
            txt.Dock = DockStyle.Fill;
            txt.Font = new Font("Segoe UI", 12);
            txt.ReadOnly = esReadOnly;

            campoLayout.Controls.Add(lbl, 0, 0);
            campoLayout.Controls.Add(txt, 0, 1);

            return campoLayout;
        }
        // Método para crear la tabla de Correctores (la parte más compleja)
        private Control CrearPanelDiagnostico()
        {
            // FlowLayoutPanel principal
            FlowLayoutPanel panelReceta = new FlowLayoutPanel();
            panelReceta.FlowDirection = FlowDirection.TopDown;
            panelReceta.AutoSize = true;
            panelReceta.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelReceta.Margin = new Padding(10);
            panelReceta.Width = 820;

            // --- Título + campo ---
            FlowLayoutPanel panelTituloTexto = new FlowLayoutPanel();
            panelTituloTexto.FlowDirection = FlowDirection.LeftToRight;
            panelTituloTexto.AutoSize = true;
            panelTituloTexto.Margin = new Padding(0, 10, 10, 10);
            panelTituloTexto.Width = 820;

            Label lblTitulo = new Label();
            lblTitulo.Text = "DIAGNÓSTICO:";
            lblTitulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            lblTitulo.AutoSize = true;

            TextBox txtDiagnosticoTitulo = new TextBox();
            txtDiagnosticoTitulo.Name = "txtDoctorDiagnostico";
            txtDiagnosticoTitulo.Width = 250;
            txtDiagnosticoTitulo.Font = new Font("Segoe UI", 12);
            txtDiagnosticoTitulo.Margin = new Padding(20, 0, 0, 0);

            CheckBox chkMostrar = new CheckBox();
            chkMostrar.Text = "Mostrar Diagnóstico";
            chkMostrar.Name = "cmbMostrarDiagnostico"; // Renombrado para evitar conflicto con cmbMotivoConsulta
            chkMostrar.Checked = false; // Inicia oculto
            chkMostrar.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            chkMostrar.AutoSize = true;
            chkMostrar.Margin = new Padding(10, 13, 0, 0);

            panelTituloTexto.Controls.Add(lblTitulo);
            panelTituloTexto.Controls.Add(txtDiagnosticoTitulo);

            panelReceta.Controls.Add(chkMostrar);
            panelReceta.Controls.Add(panelTituloTexto);

            // --- Contenedor principal ---
            TableLayoutPanel contenedor = new TableLayoutPanel();
            contenedor.AutoSize = true;
            contenedor.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            contenedor.ColumnCount = 2;
            contenedor.RowCount = 1;
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

            // --- Tabla Diagnóstico ---
            TableLayoutPanel tablaDiagnostico = new TableLayoutPanel();
            tablaDiagnostico.Name = "tblDiagnostico";
            tablaDiagnostico.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablaDiagnostico.BackColor = Color.WhiteSmoke;
            tablaDiagnostico.Width = 480;
            tablaDiagnostico.Height = 220;
            tablaDiagnostico.AutoSize = true;
            tablaDiagnostico.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            tablaDiagnostico.ColumnCount = 3;
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            string[] campos = { "AV.SC.", "AV.CC.", "PIO/ICARE", "FECHA:", "HORA DE INICIO:", "HORA DE TÉRMINO:" };
            tablaDiagnostico.RowCount = campos.Length + 1;
            tablaDiagnostico.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            // Cabecera
            string[] headers = { "", "OD", "OI" };
            for (int i = 0; i < headers.Length; i++)
            {
                Label header = new Label();
                header.Text = headers[i];
                header.Dock = DockStyle.Fill;
                header.TextAlign = ContentAlignment.MiddleCenter;
                header.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                header.BackColor = Color.LightSteelBlue;
                tablaDiagnostico.Controls.Add(header, i, 0);
            }

            // Filas
            for (int r = 0; r < campos.Length; r++)
            {
                int fila = r + 1;
                Label lblCampo = new Label();
                lblCampo.Text = campos[r];
                lblCampo.Dock = DockStyle.Fill;
                lblCampo.TextAlign = ContentAlignment.MiddleCenter;
                lblCampo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                tablaDiagnostico.Controls.Add(lblCampo, 0, fila);

                string campoBase = campos[r]
                    .Replace("/", "")
                    .Replace(".", "")
                    .Replace(" ", "")
                    .Replace(":", "")
                    .Replace("-", "");

                if (campos[r] == "FECHA:")
                {
                    DateTimePicker dtpFecha = new DateTimePicker();
                    dtpFecha.Format = DateTimePickerFormat.Short;
                    dtpFecha.Name = "dtpFechaDiagnostico";
                    dtpFecha.Dock = DockStyle.Fill;
                    dtpFecha.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(dtpFecha, 1, fila);
                    tablaDiagnostico.SetColumnSpan(dtpFecha, 2);
                }
                else if (campos[r] == "HORA DE INICIO:" || campos[r] == "HORA DE TÉRMINO:")
                {
                    DateTimePicker dtpHora = new DateTimePicker();
                    dtpHora.Format = DateTimePickerFormat.Time;
                    dtpHora.ShowUpDown = true;
                    dtpHora.Name = campos[r].Contains("INICIO") ? "dtpHoraInicio" : "dtpHoraTermino";
                    dtpHora.Dock = DockStyle.Fill;
                    dtpHora.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(dtpHora, 1, fila);
                    tablaDiagnostico.SetColumnSpan(dtpHora, 2);
                }
                else
                {
                    TextBox txtOD = new TextBox();
                    txtOD.Dock = DockStyle.Fill;
                    txtOD.Name = $"txt{campoBase}_OD";
                    txtOD.TextAlign = HorizontalAlignment.Center;
                    txtOD.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(txtOD, 1, fila);

                    TextBox txtOI = new TextBox();
                    txtOI.Dock = DockStyle.Fill;
                    txtOI.Name = $"txt{campoBase}_OI";
                    txtOI.TextAlign = HorizontalAlignment.Center;
                    txtOI.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(txtOI, 2, fila);
                }
            }

            // --- TextArea ---
            TextBox txtObservaciones = new TextBox();
            txtObservaciones.Multiline = true;
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Font = new Font("Segoe UI", 11);
            txtObservaciones.Height = 220;
            txtObservaciones.Name = "txtObservacionesDiagnostico";
            txtObservaciones.Margin = new Padding(0, 0, 0, 0);

            // === Estado inicial: solo textarea, pero con ancho total ===
            contenedor.ColumnCount = 1;
            contenedor.ColumnStyles.Clear();
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenedor.Controls.Add(txtObservaciones, 0, 0);
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Width = 820 - 40; // igual al ancho de la versión completa

            panelReceta.Controls.Add(contenedor);

            // === Evento del CheckBox ===
            chkMostrar.CheckedChanged += (s, e) =>
            {
                contenedor.SuspendLayout();
                contenedor.Controls.Clear();

                if (chkMostrar.Checked)
                {
                    // Mostrar tabla y textarea
                    contenedor.ColumnCount = 2;
                    contenedor.RowCount = 1;
                    contenedor.ColumnStyles.Clear();
                    contenedor.RowStyles.Clear();
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 480F)); // tabla
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F)); // textarea
                    contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    contenedor.Controls.Add(tablaDiagnostico, 0, 0);
                    contenedor.Controls.Add(txtObservaciones, 1, 0);

                    tablaDiagnostico.Dock = DockStyle.Fill;
                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Margin = new Padding(10, 0, 0, 0);
                    txtObservaciones.Height = tablaDiagnostico.Height;
                }
                else
                {
                    // Solo textarea
                    contenedor.ColumnCount = 1;
                    contenedor.RowCount = 1;
                    contenedor.ColumnStyles.Clear();
                    contenedor.RowStyles.Clear();
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    contenedor.Controls.Add(txtObservaciones, 0, 0);

                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Margin = new Padding(0);
                    txtObservaciones.Width = 780; // ocupa casi todo el ancho
                }

                contenedor.ResumeLayout();
            };

            return panelReceta;
        }

        void LimpiarControles(Control parent)
        {
            // TextBox
            string[] txts = {
                "txtOptometro", "txtLEJOSODEsferico", "txtLEJOSODCilindrico", "txtLEJOSODEje", "txtLEJOSODDIP", "txtLEJOSODAgudezaVisual",
                "txtLEJOSOIEsferico", "txtLEJOSOICilindrico", "txtLEJOSOIEje", "txtLEJOSOIDIP", "txtLEJOSOIAgudezaVisual",
                "txtCERCAODEsferico", "txtCERCAODCilindrico", "txtCERCAODEje", "txtCERCAODDIP", "txtCERCAODAgudezaVisual",
                "txtCERCAOIEsferico", "txtCERCAOICilindrico", "txtCERCAOIEje", "txtCERCAOIDIP", "txtCERCAOIAgudezaVisual",
                "txtObservaciones1", "txtDoctorExamenOftalmologico", "txtSignosSintomas1", "txtExamenOftalmologico",
                "txtDoctorDiagnostico", "txtObservacionesDiagnostico",
                "txtAVSC_OD","txtAVSC_OI","txtAVCC_OD","txtAVCC_OI",
                "txtPIOICARE_OD","txtPIOICARE_OI",
                "txtTratamiento1"
            };

            foreach (string nombre in txts)
            {
                Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                if (ctrls.Length > 0 && ctrls[0] is TextBox txt)
                    txt.Clear();
            }

            // ComboBox
            Control[] ctrlMotivo = panelHorizontal.Controls.Find("cmbMotivoConsulta", true);
            if (ctrlMotivo.Length > 0 && ctrlMotivo[0] is ComboBox cmbMotivo)
                cmbMotivo.SelectedIndex = -1;

            // DateTimePicker
            string[] dtps = { "dtpFechaConsulta", "dtpFechaDiagnostico", "dtpHoraInicio", "dtpHoraTermino" };
            foreach (string nombre in dtps)
            {
                Control[] ctrls = panelHorizontal.Controls.Find(nombre, true);
                if (ctrls.Length > 0 && ctrls[0] is DateTimePicker dtp)
                    dtp.Value = DateTime.Now;
            }


        }



        // Método para crear la tabla de Correctores (la parte más compleja)
        private Control CrearPanelCorrectores()
        {
            // FlowLayoutPanel para el título y la tabla
            FlowLayoutPanel panelReceta = new FlowLayoutPanel();
            panelReceta.FlowDirection = FlowDirection.TopDown;
            panelReceta.AutoSize = true;
            panelReceta.Margin = new Padding(10, 10, 10, 0);
            panelReceta.Width = 820; // Ajustar ancho para la tabla


            // === Subpanel horizontal para "Optometro (a):" ===
            FlowLayoutPanel panelOptometro = new FlowLayoutPanel();
            panelOptometro.FlowDirection = FlowDirection.LeftToRight;
            panelOptometro.AutoSize = true;
            panelOptometro.Margin = new Padding(0, 0, 0, 5);
            panelOptometro.Width = 820;
            panelOptometro.WrapContents = false;

            Label lblOptometro = new Label();
            lblOptometro.Text = "OPTÓMETRO (A): ";
            lblOptometro.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblOptometro.TextAlign = ContentAlignment.MiddleLeft;
            lblOptometro.AutoSize = true;

            TextBox txtOptometro = new TextBox();
            txtOptometro.Name = "txtOptometro";
            txtOptometro.Width = 250;
            txtOptometro.Font = new Font("Segoe UI", 12);
            txtOptometro.Margin = new Padding(20, 0, 0, 0);
            panelOptometro.Controls.Add(lblOptometro);
            panelOptometro.Controls.Add(txtOptometro);

            // Agregar el subpanel al principal
            panelReceta.Controls.Add(panelOptometro);


            // TableLayoutPanel para la rejilla de datos
            TableLayoutPanel tablaCorrectores = new TableLayoutPanel();
            tablaCorrectores.Name = "tblCorrectores";
            tablaCorrectores.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablaCorrectores.BackColor = Color.WhiteSmoke;
            tablaCorrectores.Width = 800;
            tablaCorrectores.Height = 150;

            // Definición de columnas
            tablaCorrectores.ColumnCount = 7;
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // CORRECTORES
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));  // Columna 'OD/OI'
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // ESFÉRICO
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // CILÍNDRICO
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10)); // EJE
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10)); // DIP
            tablaCorrectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // AGUDEZA VISUAL

            // Definición de filas: 1 (Cabecera) + 4 (Lejos OD, Lejos OI, Cerca OD, Cerca OI)
            tablaCorrectores.RowCount = 5;
            tablaCorrectores.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            tablaCorrectores.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            tablaCorrectores.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            tablaCorrectores.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            tablaCorrectores.RowStyles.Add(new RowStyle(SizeType.Percent, 25));


            // --- Cabecera (Fila 0) ---
            string[] headers = { "", "", "ESFÉRICO", "CILÍNDRICO", "EJE", "DIP", "AGUDEZA VISUAL" };
            for (int i = 0; i < tablaCorrectores.ColumnCount; i++)
            {
                if (!string.IsNullOrEmpty(headers[i]))
                {
                    Label headerLabel = new Label();
                    headerLabel.Text = headers[i];
                    headerLabel.Dock = DockStyle.Fill;
                    headerLabel.TextAlign = ContentAlignment.MiddleCenter;
                    headerLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    headerLabel.BackColor = Color.LightSteelBlue;
                    tablaCorrectores.Controls.Add(headerLabel, i, 0);
                }
            }

            // Label 'CORRECTORES' (ocupa celdas (0,0) y (0,1))
            Label lblCorrectores = new Label();
            lblCorrectores.Text = "CORRECTORES";

            lblCorrectores.Dock = DockStyle.Fill;
            lblCorrectores.TextAlign = ContentAlignment.MiddleCenter;
            lblCorrectores.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCorrectores.BackColor = Color.LightSteelBlue;
            tablaCorrectores.Controls.Add(lblCorrectores, 0, 0);
            tablaCorrectores.SetColumnSpan(lblCorrectores, 2);


            // --- Filas de Datos (1 a 4) ---
            string[] ojos = { "OD", "OI", "OD", "OI" };
            string[] tipos = { "LEJOS", "LEJOS", "CERCA", "CERCA" };

            for (int r = 0; r < 4; r++)
            {
                int fila = r + 1;

                // Columna 1: Tipo (LEJOS/CERCA)
                if (r == 0) // LEJOS - Ocupa 2 filas
                {
                    Label lblTipo = new Label();
                    lblTipo.Text = "LEJOS";
                    lblTipo.Dock = DockStyle.Fill;
                    lblTipo.TextAlign = ContentAlignment.MiddleCenter;
                    lblTipo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    tablaCorrectores.Controls.Add(lblTipo, 0, 1);
                    tablaCorrectores.SetRowSpan(lblTipo, 2);
                }
                else if (r == 2) // CERCA - Ocupa 2 filas
                {
                    Label lblTipo = new Label();
                    lblTipo.Text = "CERCA";
                    lblTipo.Dock = DockStyle.Fill;
                    lblTipo.TextAlign = ContentAlignment.MiddleCenter;
                    lblTipo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    tablaCorrectores.Controls.Add(lblTipo, 0, 3);
                    tablaCorrectores.SetRowSpan(lblTipo, 2);
                }

                // Columna 2: OD / OI
                Label lblOjo = new Label();
                lblOjo.Text = ojos[r];
                lblOjo.Dock = DockStyle.Fill;
                lblOjo.TextAlign = ContentAlignment.MiddleCenter;
                lblOjo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                tablaCorrectores.Controls.Add(lblOjo, 1, fila);


                // Columnas de datos (3 a 6) - ESFÉRICO a AGUDEZA VISUAL
                string[] camposReceta = { "Esferico", "Cilindrico", "Eje", "DIP", "AgudezaVisual" };

                for (int c = 0; c < camposReceta.Length; c++)
                {
                    TextBox txtReceta = new TextBox();
                    txtReceta.Name = $"txt{tipos[r]}{ojos[r]}{camposReceta[c]}";
                    txtReceta.Dock = DockStyle.Fill;
                    txtReceta.BorderStyle = BorderStyle.None;
                    txtReceta.TextAlign = HorizontalAlignment.Center;
                    txtReceta.Font = new Font("Segoe UI", 11);

                    // Columna en la tabla es 2 + c
                    tablaCorrectores.Controls.Add(txtReceta, 2 + c, fila);
                }


            }

            panelReceta.Controls.Add(tablaCorrectores);
            return panelReceta;
        }


        // --- FIN MÉTODOS AUXILIARES ---

        private void FormRegistrarHistorial_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - RegistrarHistorial";
            this.Icon = new Icon("Imagenes/log.ico");
        }

        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void LimpiarCamposRegistro()
        {
            // Limpiar campos de paciente
            string[] camposPaciente = new string[]
            {
               "Dni", "Apellidos", "Nombres"
            };

            foreach (string campo in camposPaciente)
            {
                Control[] controles = panelHorizontal.Controls.Find("txt" + campo, true);
                if (controles.Length > 0 && controles[0] is TextBox txt)
                {
                    txt.Text = string.Empty;
                }
            }

            // Limpiar Motivo de Consulta
            // Ya no es un TextBox, es un ComboBox
            Control[] cmbMotivo = panelHorizontal.Controls.Find("cmbMotivoConsulta", true);
            if (cmbMotivo.Length > 0 && cmbMotivo[0] is ComboBox)
            {
                ((ComboBox)cmbMotivo[0]).SelectedIndex = -1; // Deseleccionar
            }


            // Limpiar Correctores (la tabla)
            Control[] tabla = panelHorizontal.Controls.Find("tblCorrectores", true);
            if (tabla.Length > 0 && tabla[0] is TableLayoutPanel tbl)
            {
                // Iterar sobre todos los controles de la tabla que son TextBox y limpiarlos
                foreach (Control control in tbl.Controls)
                {
                    if (control is TextBox txtReceta)
                    {
                        txtReceta.Text = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Genera la imagen del historial clínico con los datos proporcionados y la devuelve como un array de bytes.
        /// </summary>
        /// <param name="datos">Objeto anónimo con los datos principales.</param>
        /// <param name="datosPaciente">Diccionario con los datos del paciente.</param>
        /// <param name="recetaVisual">Diccionario con los datos de la receta visual.</param>
        /// <param name="datosDiagnostico">Diccionario con los datos del diagnóstico.</param>
        /// <param name="dibujoOjoDerecho">Bitmap del dibujo del ojo derecho.</param>
        /// <param name="dibujoOjoIzquierdo">Bitmap del dibujo del ojo izquierdo.</param>
        /// <returns>Array de bytes de la imagen generada en formato PNG.</returns>
        private byte[] GenerarImagenConDatos(
            dynamic datos,
            Dictionary<string, string> datosPaciente,
            Dictionary<string, string> recetaVisual,
            Dictionary<string, string> datosDiagnostico,
            Bitmap dibujoOjoDerecho,
            Bitmap dibujoOjoIzquierdo)
        {
            string rutaPlantilla = Path.Combine(Application.StartupPath, "Resources", "plantilla_historial.jpg");

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show("Plantilla de imagen no encontrada en: " + rutaPlantilla, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            Bitmap plantilla = new Bitmap(System.Drawing.Image.FromFile(rutaPlantilla));
            using (Graphics g = Graphics.FromImage(plantilla))
            {
                Font fuente = new Font("Tahoma", 12, FontStyle.Regular);
                Font fuente2 = new Font("Tahoma", 11, FontStyle.Regular);
                Brush pincel = Brushes.Black;

                float ancho = plantilla.Width;
                float alto = plantilla.Height;

                // === Datos principales ===
                g.DrawString(datos.Apellidos, fuente, pincel, new PointF(ancho * 0.14f, alto * 0.0925f));
                g.DrawString(datos.Nombres, fuente, pincel, new PointF(ancho * 0.14f, alto * 0.1125f));
                g.DrawString(datos.Dni, fuente, pincel, new PointF(ancho * 0.58f, alto * 0.092f));
                g.DrawString(datos.FechaConsulta, fuente, pincel, new PointF(ancho * 0.85f, alto * 0.21f));
                g.DrawString(datos.MotivoConsulta, fuente, pincel, new PointF(ancho * 0.23f, alto * 0.225f));
                g.DrawString(datos.Tratamiento, fuente, pincel, new PointF(ancho * 0.13f, alto * 0.87f));
                g.DrawString(datos.Observaciones, fuente, pincel, new PointF(ancho * 0.13f, alto * 0.46f));
                g.DrawString(datos.DRExamenOftalmologico, fuente, pincel, new PointF(ancho * 0.25f, alto * 0.65f));
                g.DrawString(datos.ObservacionesDiagnostico, fuente, pincel, new PointF(ancho * 0.50f, alto * 0.77f));
                g.DrawString(datos.Optometro, fuente, pincel, new PointF(ancho * 0.40f, alto * 0.225f));
                g.DrawString(datos.SignosSintomas, fuente, pincel, new PointF(ancho * 0.13f, alto * 0.57f));
                g.DrawString(datos.ExamenOftalmologico, fuente, pincel, new PointF(ancho * 0.13f, alto * 0.67f));
                g.DrawString(datos.DoctorDiagnostico, fuente, pincel, new PointF(ancho * 0.17f, alto * 0.755f));

                // === Receta Visual ===
                Dictionary<string, PointF> posicionesReceta = new Dictionary<string, PointF>
                {
                    { "LEJOS_OD_Esferico", new PointF(ancho * 0.25f, alto * 0.28f) },
                    { "LEJOS_OD_Cilindrico", new PointF(ancho * 0.39f, alto * 0.28f) },
                    { "LEJOS_OD_Eje", new PointF(ancho * 0.545f, alto * 0.28f) },
                    { "LEJOS_OD_DIP", new PointF(ancho * 0.705f, alto * 0.28f) },
                    { "LEJOS_OD_AgudezaVisual", new PointF(ancho * 0.875f, alto * 0.28f) },

                    { "LEJOS_OI_Esferico", new PointF(ancho * 0.25f, alto * 0.325f) },
                    { "LEJOS_OI_Cilindrico", new PointF(ancho * 0.39f, alto * 0.325f)},
                    { "LEJOS_OI_Eje", new PointF(ancho * 0.545f, alto * 0.325f) },
                    { "LEJOS_OI_DIP", new PointF(ancho * 0.705f, alto * 0.325f) },
                    { "LEJOS_OI_AgudezaVisual", new PointF(ancho * 0.875f, alto * 0.325f) },

                    { "CERCA_OD_Esferico", new PointF(ancho * 0.25f, alto * 0.37f) },
                    { "CERCA_OD_Cilindrico", new PointF(ancho * 0.39f, alto * 0.37f)},
                    { "CERCA_OD_Eje", new PointF(ancho * 0.545f, alto * 0.37f) },
                    { "CERCA_OD_DIP", new PointF(ancho * 0.705f, alto * 0.37f) },
                    { "CERCA_OD_AgudezaVisual", new PointF(ancho * 0.875f, alto * 0.37f) },

                    { "CERCA_OI_Esferico", new PointF(ancho * 0.25f, alto * 0.41f) },
                    { "CERCA_OI_Cilindrico", new PointF(ancho * 0.39f, alto * 0.41f) },
                    { "CERCA_OI_Eje", new PointF(ancho * 0.545f, alto * 0.41f) },
                    { "CERCA_OI_DIP", new PointF(ancho * 0.705f, alto * 0.41f) },
                    { "CERCA_OI_AgudezaVisual", new PointF(ancho * 0.875f, alto * 0.41f) }
                };

                foreach (var kvp in recetaVisual)
                {
                    if (posicionesReceta.TryGetValue(kvp.Key, out PointF posicion))
                        g.DrawString(kvp.Value, fuente, pincel, posicion);
                }

                // === Datos del paciente ===
                Dictionary<string, PointF> posicionesPaciente = new Dictionary<string, PointF>
                {
                    { "Direccion", new PointF(ancho * 0.14f, alto * 0.1325f) },
                    { "Telefono", new PointF(ancho * 0.14f, alto * 0.1525f) },
                    { "Correo", new PointF(ancho * 0.14f, alto * 0.1725f) },
                    { "EstadoCivil", new PointF(ancho * 0.14f, alto * 0.195f) },

                    { "Celular", new PointF(ancho * 0.36f, alto * 0.1525f) },
                    { "Instruccion", new PointF(ancho * 0.35f, alto * 0.195f) },
                    { "Distrito", new PointF(ancho * 0.58f, alto * 0.112f) },
                    { "Sexo", new PointF(ancho * 0.58f, alto * 0.152f) },
                    { "FechaNacimiento", new PointF(ancho * 0.58f, alto * 0.175f) },

                    { "Edad", new PointF(ancho * 0.74f, alto * 0.175f) },
                    { "Ocupacion", new PointF(ancho * 0.58f, alto * 0.195f) }
                };

                foreach (var kvp in datosPaciente)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value) && posicionesPaciente.TryGetValue(kvp.Key, out PointF posicion))
                    {
                        string valor = kvp.Value;
                        if (kvp.Key == "FechaNacimiento" && DateTime.TryParse(valor, out DateTime fecha))
                            valor = fecha.ToShortDateString();

                        g.DrawString(valor, fuente, pincel, posicion);
                    }
                }

                // === Diagnóstico ===
                Dictionary<string, PointF> posicionesDiagnostico = new Dictionary<string, PointF>
                {
                    { "AVSC_OD", new PointF(ancho * 0.20f, alto * 0.775f) },
                    { "AVSC_OI", new PointF(ancho * 0.31f, alto * 0.775f) },

                    { "AVCC_OD", new PointF(ancho * 0.20f, alto * 0.785f) },
                    { "AVCC_OI", new PointF(ancho * 0.31f, alto * 0.785f) },

                    { "PIOICARE_OD", new PointF(ancho * 0.20f, alto * 0.795f) },
                    { "PIOICARE_OI", new PointF(ancho * 0.31f, alto * 0.795f) },

                    { "FECHA", new PointF(ancho * 0.20f, alto * 0.805f) },

                    { "HORA DE INICIO", new PointF(ancho * 0.20f, alto * 0.815f) },

                    { "HORA DE TÉRMINO", new PointF(ancho * 0.20f, alto * 0.825f) }
                };

                foreach (var kvp in datosDiagnostico)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value) && posicionesDiagnostico.TryGetValue(kvp.Key, out PointF posicion))
                        g.DrawString($"{kvp.Key}: {kvp.Value}", fuente2, pincel, posicion);
                }

                // === Ojos ===
                Dictionary<string, PointF> posicionesOjos = new Dictionary<string, PointF>
                {
                    { "OjoDerecho_Titulo", new PointF(ancho * 0.68f, alto * 0.56f) },
                    { "OjoDerecho_Imagen", new PointF(ancho * 0.58f, alto * 0.58f) },
                    { "OjoIzquierdo_Titulo", new PointF(ancho * 0.8f, alto * 0.56f) },
                    { "OjoIzquierdo_Imagen", new PointF(ancho * 0.8f, alto * 0.58f) }
                };

                if (dibujoOjoDerecho != null)
                {
                    g.DrawString("DERECHO", fuente, pincel, posicionesOjos["OjoDerecho_Titulo"]);
                    g.DrawImage(dibujoOjoDerecho, new System.Drawing.Rectangle((int)posicionesOjos["OjoDerecho_Imagen"].X, (int)posicionesOjos["OjoDerecho_Imagen"].Y, 240, 120));
                }

                if (dibujoOjoIzquierdo != null)
                {
                    g.DrawString("IZQUIERDO", fuente, pincel, posicionesOjos["OjoIzquierdo_Titulo"]);
                    g.DrawImage(dibujoOjoIzquierdo, new System.Drawing.Rectangle((int)posicionesOjos["OjoIzquierdo_Imagen"].X, (int)posicionesOjos["OjoIzquierdo_Imagen"].Y, 240, 120));
                }
            }

            // Convertir la imagen a bytes y devolver
            using (MemoryStream ms = new MemoryStream())
            {
                plantilla.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convierte un array de bytes de una imagen (PNG/JPG) a un array de bytes de un documento PDF.
        /// </summary>
        /// <param name="imagenBytes">Array de bytes de la imagen a convertir.</param>
        /// <returns>Array de bytes del PDF resultante.</returns>
        private byte[] ConvertirImagenBytesAPDFBytes(byte[] imagenBytes)
        {
            if (imagenBytes == null || imagenBytes.Length == 0) return null;

            using (MemoryStream msPdf = new MemoryStream())
            {
                iTextSharp.text.Document documento = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                iTextSharp.text.pdf.PdfWriter.GetInstance(documento, msPdf);
                documento.Open();

                try
                {
                    iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(imagenBytes);
                    imagen.ScaleToFit(iTextSharp.text.PageSize.A4.Width - 40, iTextSharp.text.PageSize.A4.Height - 40);
                    imagen.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    documento.Add(imagen);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al convertir imagen a PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                finally
                {
                    documento.Close();
                }
                return msPdf.ToArray();
            }
        }

        /// <summary>
        /// Une un array de bytes de una imagen (convertida a PDF internamente) con un array de bytes de un PDF existente.
        /// Todo el proceso se realiza en memoria.
        /// </summary>
        /// <param name="imagenBytes">Array de bytes de la imagen a añadir al inicio del PDF.</param>
        /// <param name="pdfOriginalBytes">Array de bytes del PDF existente a unir.</param>
        /// <returns>Array de bytes del PDF resultante con la imagen y el PDF original.</returns>
        private byte[] UnirImagenConPDFsEnMemoria(byte[] imagenBytes, byte[] pdfOriginalBytes)
        {
            if (imagenBytes == null || imagenBytes.Length == 0) return pdfOriginalBytes;
            if (pdfOriginalBytes == null || pdfOriginalBytes.Length == 0) return ConvertirImagenBytesAPDFBytes(imagenBytes);

            byte[] pdfImagenBytes = ConvertirImagenBytesAPDFBytes(imagenBytes);
            if (pdfImagenBytes == null || pdfImagenBytes.Length == 0) return pdfOriginalBytes;

            using (MemoryStream msFinal = new MemoryStream())
            {
                iTextSharp.text.Document documento = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfCopy copia = new iTextSharp.text.pdf.PdfCopy(documento, msFinal);
                documento.Open();

                try
                {
                    // Añadir la página de la imagen convertida a PDF
                    using (iTextSharp.text.pdf.PdfReader lectorImagen = new iTextSharp.text.pdf.PdfReader(pdfImagenBytes))
                    {
                        copia.AddPage(copia.GetImportedPage(lectorImagen, 1));
                        lectorImagen.Close();
                    }

                    // Añadir las páginas del PDF original
                    using (iTextSharp.text.pdf.PdfReader lectorOriginal = new iTextSharp.text.pdf.PdfReader(pdfOriginalBytes))
                    {
                        for (int i = 1; i <= lectorOriginal.NumberOfPages; i++)
                        {
                            copia.AddPage(copia.GetImportedPage(lectorOriginal, i));
                        }
                        lectorOriginal.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al unir PDFs en memoria: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                finally
                {
                    documento.Close();
                }
                return msFinal.ToArray();
            }
        }


        private Bitmap ObtenerImagenDesdePanel(string nombrePanel)
        {
            Control[] ctrls = panelHorizontal.Controls.Find(nombrePanel, true);
            if (ctrls.Length > 0 && ctrls[0] is FlowLayoutPanel pnl)
            {
                PictureBox pic = pnl.Controls.OfType<PictureBox>().FirstOrDefault();
                if (pic?.Image != null)
                    return new Bitmap(pic.Image);
            }
            return null;
        }
    }
}