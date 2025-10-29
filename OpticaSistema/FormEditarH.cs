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
using static OpticaSistema.FormLogin;

namespace OpticaSistema
{
    public partial class FormEditarH : Form
    {
        private ConexionDB conexionBD;
        private FlowLayoutPanel panelHorizontal;
        private byte[] archivoPDF = null;
        private string nombreArchivo = null;
        private TableLayoutPanel tablaDiagnostico;

        Dictionary<string, string> mapaColumnas = new Dictionary<string, string>()
{
    //MEDIDA DE LA VISTA
    { "txtOptometro", "Nombre_optometra" },
    { "txtLEJOSODEsferico", "Lejos_OD_Esferico" },
    { "txtLEJOSODCilindrico", "Lejos_OD_Cilindrico" },
    { "txtLEJOSODEje", "Lejos_OD_EJE" },
    { "txtLEJOSODDIP", "Lejos_OD_DIP" },
    { "txtLEJOSODAgudezaVisual", "Lejos_OD_AV" },

    { "txtLEJOSOIEsferico", "Lejos_OI_Esferico" },
    { "txtLEJOSOICilindrico", "Lejos_OI_Cilindrico" },
    { "txtLEJOSOIEje", "Lejos_OI_EJE" },
    { "txtLEJOSOIDIP", "Lejos_OI_DIP" },
    { "txtLEJOSOIAgudezaVisual", "Lejos_OI_AV" },

    { "txtCERCAODEsferico", "Cerca_OD_Esferico" },
    { "txtCERCAODCilindrico", "Cerca_OD_Cilindrico" },
    { "txtCERCAODEje", "Cerca_OD_EJE" },
    { "txtCERCAODDIP", "Cerca_OD_DIP" },
    { "txtCERCAODAgudezaVisual", "Cerca_OD_AV" },

    { "txtCERCAOIEsferico", "Cerca_OI_Esferico" },
    { "txtCERCAOICilindrico", "Cerca_OI_Cilindrico" },
    { "txtCERCAOIEje", "Cerca_OI_EJE" },
    { "txtCERCAOIDIP", "Cerca_OI_DIP" },
    { "txtCERCAOIAgudezaVisual", "Cerca_OI_AV" },
    { "txtObservaciones1","Observaciones" },



    //CONSULTA OFTALMOLÓGICA
    { "txtDoctorExamenOftalmologico", "Nombre_oftalmologo" },
    { "txtSignosSintomas1", "SignosSintomas" },
    { "txtExamenOftalmologico","ExamenOftamologico" },
    {"panelOjoDerecho","OjoDerecho" },
    {"panelOjoIzquierdo","OjoIzquierdo" },


    //CONSULTA CON RETINOLOGO
    {"txtDoctorDiagnostico","Nombre_retinologo" },
    {"txtObservacionesDiagnostico","Diagnostico" },
    {"txtAVSC_OD","AV_SC_OD" },
    {"txtAVSC_OI","AV_SC_OI" },
    {"txtAVCC_OD","AV_CC_OD" },
    {"txtAVCC_OI","AV_CC_OI" },
    {"txtPIOICARE_OD","PIO_OD" },
    {"txtPIOICARE_OI","PIO_OI" },
    {"dtpFechaDiagnostico","Fecha_Diagnostico" },
    {"dtpHoraInicio","Hora_Inicio" },
    {"dtpHoraTermino","Hora_Termino" },
    {"txtTratamiento1","Tratamiento" },
    {"lblNombreArchivo","NombreArchivo" },
    {"Archivo","Archivo"}


};
        private int idHistorial;
        public FormEditarH(int idHistorial)
        {
            InitializeComponent();
            this.idHistorial = idHistorial;
            this.WindowState = FormWindowState.Maximized;
            conexionBD = new ConexionDB();
            this.BackColor = Color.White;
            

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));   // Título ocupa 10% del alto
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

            // Agregar wrapper a la columna central del contenedor
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
                "Dni", "Apellidos", "Nombres", "Edad"
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
            dtpFechaConsulta.Format = DateTimePickerFormat.Custom;
            dtpFechaConsulta.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpFechaConsulta.Enabled = false;
            fechaLayout.Controls.Add(dtpFechaConsulta, 0, 1);

            // 2. Motivo de Consulta
            TableLayoutPanel motivoLayout = CrearCampoLayout("MotivoConsulta", 400, 80, false);
            Control lblMotivo = motivoLayout.Controls[0];
            lblMotivo.Text = "Motivo de Consulta:";

            // ComboBox en lugar de TextBox
            ComboBox cmbMotivo = new ComboBox();
            cmbMotivo.Name = "cmbMotivoConsulta1";
            cmbMotivo.Enabled = false;
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
            txtObservaciones.ReadOnly = true;
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
            txtExamenTitulo.ReadOnly = true;
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
            lblNombreArchivo.Name = "lblNombreArchivo";
            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblNombreArchivo.ForeColor = Color.DimGray;
            lblNombreArchivo.AutoSize = true;
            lblNombreArchivo.Margin = new Padding(0, 0, 0, 10);

            // Botón para subir PDF
            Button btnSubirPDF = new Button();
            btnSubirPDF.Text = "Subir PDF";
            btnSubirPDF.Name = "btnSubirPDF";
            btnSubirPDF.Enabled = false;
            btnSubirPDF.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSubirPDF.Width = 160;
            btnSubirPDF.Height = 40;
            btnSubirPDF.BackColor = Color.SteelBlue;
            btnSubirPDF.ForeColor = Color.White;
            btnSubirPDF.FlatStyle = FlatStyle.Flat;
            btnSubirPDF.FlatAppearance.BorderSize = 0;

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
                            // Leer bytes del archivo
                            byte[] bytesPDF = File.ReadAllBytes(rutaPDF);

                            // Guardar nombre y bytes
                            lblNombreArchivo.Text = nombreArchivoPDF;
                            lblNombreArchivo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                            lblNombreArchivo.ForeColor = Color.Black;

                            // 🔹 Guardamos bytes en Tag para acceso posterior
                            lblNombreArchivo.Tag = bytesPDF;

                            // También los guardamos en las variables globales (opcional)
                            archivoPDF = bytesPDF;
                            nombreArchivo = nombreArchivoPDF;

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
                            lblNombreArchivo.Tag = null;
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
            btnRegistrar.Text = "EDITAR";
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
            btnRegistrar.Click += (s, e) =>
            {
                EditarPorMotivo(idHistorial);

            };
            CargarDatosHistorial();
        }

        private void FormEditarH_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - EditarHistorial";
            this.Icon = new Icon("Imagenes/log.ico");
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
                imagenBase = new Bitmap(Image.FromFile(ruta), picOjo.Size);
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

            return panelOjo;


        }

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

        private Control CrearPanelDiagnostico()
        {// FlowLayoutPanel principal
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
            txtDiagnosticoTitulo.ReadOnly = true;
            txtDiagnosticoTitulo.Width = 250;
            txtDiagnosticoTitulo.Font = new Font("Segoe UI", 12);
            txtDiagnosticoTitulo.Margin = new Padding(20, 0, 0, 0);

            CheckBox chkMostrar = new CheckBox();
            chkMostrar.Text = "Mostrar Diagnóstico";
            chkMostrar.Name = "cmbMostrarDiagnóstico";
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
            tablaDiagnostico = new TableLayoutPanel();
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
                else if (campos[r].Contains("HORA"))
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
                    // TextBox OD
                    TextBox txtOD = new TextBox();
                    txtOD.Dock = DockStyle.Fill;
                    txtOD.Name = $"txt{campoBase}_OD";
                    txtOD.TextAlign = HorizontalAlignment.Center;
                    txtOD.Font = new Font("Segoe UI", 11);
                    txtOD.ReadOnly = true; // inicial deshabilitado
                    tablaDiagnostico.Controls.Add(txtOD, 1, fila);

                    // TextBox OI
                    TextBox txtOI = new TextBox();
                    txtOI.Dock = DockStyle.Fill;
                    txtOI.Name = $"txt{campoBase}_OI";
                    txtOI.TextAlign = HorizontalAlignment.Center;
                    txtOI.Font = new Font("Segoe UI", 11);
                    txtOI.ReadOnly = true; // inicial deshabilitado
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
            txtObservaciones.ReadOnly = true; // inicial deshabilitado

            // Estado inicial: solo textarea
            contenedor.ColumnCount = 1;
            contenedor.ColumnStyles.Clear();
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenedor.Controls.Add(txtObservaciones, 0, 0);
            txtObservaciones.Width = 780;

            panelReceta.Controls.Add(contenedor);

            // Evento CheckBox
            chkMostrar.CheckedChanged += (s, e) =>
            {
                contenedor.SuspendLayout();
                contenedor.Controls.Clear();

                if (chkMostrar.Checked)
                {
                    contenedor.ColumnCount = 2;
                    contenedor.RowCount = 1;
                    contenedor.ColumnStyles.Clear();
                    contenedor.RowStyles.Clear();
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 480F));
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
                    contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    contenedor.Controls.Add(tablaDiagnostico, 0, 0);
                    contenedor.Controls.Add(txtObservaciones, 1, 0);

                    tablaDiagnostico.Dock = DockStyle.Fill;
                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Margin = new Padding(10, 0, 0, 0);
                }
                else
                {
                    contenedor.ColumnCount = 1;
                    contenedor.RowCount = 1;
                    contenedor.ColumnStyles.Clear();
                    contenedor.RowStyles.Clear();
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    contenedor.Controls.Add(txtObservaciones, 0, 0);
                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Margin = new Padding(0);
                }

                contenedor.ResumeLayout();
            };

            return panelReceta;
        }
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
            txtOptometro.ReadOnly = true;
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
                    txtReceta.ReadOnly = true;
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

        private void CargarDatosHistorial()
        {
            string[] camposPaciente = new string[] { "Dni", "Apellidos", "Nombres", "Edad" };
            string tipoUsuario = SesionUsuario.TipoUsuario; // O, F, R

            using (SqlConnection con = conexionBD.Conectar())
            {
                string query = @"
SELECT 
     p.Dni, p.Apellidos, p.Nombres, p.Edad,
    h.FechaConsulta, h.MotivoConsulta,
    h.Nombre_optometra,
    h.Nombre_oftalmologo,
    h.Nombre_retinologo,
    h.Lejos_OD_Esferico, h.Lejos_OD_Cilindrico, h.Lejos_OD_EJE, h.Lejos_OD_DIP, h.Lejos_OD_AV,
    h.Lejos_OI_Esferico, h.Lejos_OI_Cilindrico, h.Lejos_OI_EJE, h.Lejos_OI_DIP, h.Lejos_OI_AV,
    h.Cerca_OD_Esferico, h.Cerca_OD_Cilindrico, h.Cerca_OD_EJE, h.Cerca_OD_DIP, h.Cerca_OD_AV,
    h.Cerca_OI_Esferico, h.Cerca_OI_Cilindrico, h.Cerca_OI_EJE, h.Cerca_OI_DIP, h.Cerca_OI_AV,
    h.Observaciones,
    h.SignosSintomas, h.ExamenOftamologico, h.OjoDerecho, h.OjoIzquierdo,
    h.Diagnostico, h.AV_SC_OD, h.AV_SC_OI, h.AV_CC_OD, h.AV_CC_OI, h.PIO_OD, h.PIO_OI,
    h.Fecha_Diagnostico, h.Hora_Inicio, h.Hora_Termino, h.Tratamiento, h.NombreArchivo, h.Archivo
FROM HistorialClinicoBD h
INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
WHERE h.Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", idHistorial);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // === Cargar datos del paciente ===
                        foreach (string campo in camposPaciente)
                        {
                            Control[] controles = panelHorizontal.Controls.Find("txt" + campo, true);
                            if (controles.Length > 0 && controles[0] is TextBox txt)
                                txt.Text = reader[campo].ToString();
                        }

                        // === Cargar fecha de consulta ===
                        Control[] fechaControl = panelHorizontal.Controls.Find("dtpFechaConsulta", true);
                        if (fechaControl.Length > 0 && fechaControl[0] is DateTimePicker dtp)
                            dtp.Value = Convert.ToDateTime(reader["FechaConsulta"]);

                        // === Cargar motivo de consulta ===
                        string motivoConsulta = reader["MotivoConsulta"].ToString();
                        Control[] controlesMotivo = panelHorizontal.Controls.Find("cmbMotivoConsulta1", true);

                        if (controlesMotivo.Length > 0 && controlesMotivo[0] is ComboBox cmbMotivo)
                        {
                            cmbMotivo.SelectedItem = motivoConsulta;

                            // Si no se encuentra, intentar asignar manualmente el texto
                            if (cmbMotivo.SelectedItem == null)
                                cmbMotivo.Text = motivoConsulta;

                            MessageBox.Show("" + motivoConsulta);
                            // 👇 Aplicar validaciones según el motivo
                            AplicarValidacionesPorMotivo(motivoConsulta, tipoUsuario);

                            if (motivoConsulta.Equals("exámen ocular completo", StringComparison.OrdinalIgnoreCase))
                            {
                                CargarDatosExamenOcularCompleto();

                            }else if(motivoConsulta.Equals("consulta con retinólogo", StringComparison.OrdinalIgnoreCase))
                            {
                                CargarCamposRetinologo(reader);

                            }else if(motivoConsulta.Equals("medida de la vista", StringComparison.OrdinalIgnoreCase))
                            {
                                CargarCamposOptometra(reader);
                            }
                            else if(motivoConsulta.Equals("consulta oftalmológica", StringComparison.OrdinalIgnoreCase))
                            {
                                CargarCamposOftalmologo(reader);
                            }else
                            {
                                MessageBox.Show("Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el control cmbMotivoConsulta en el formulario.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para el historial clínico especificado.");
                    }
                }
            }
        }

        private void AplicarValidacionesPorMotivo(string motivoConsulta, string tipoUsuario)
        {
            motivoConsulta = motivoConsulta.Trim().ToLower();
            
            // 🔹 MEDIDA DE LA VISTA
            if (motivoConsulta == "medida de la vista")
            {
                HabilitarControlesOftalmologo(false);
                HabilitarControlesRetinologo(false);
                HabilitarControlesMedidaVista(true);
            }

            // 🔹 CONSULTA OFTALMOLÓGICA
            else if (motivoConsulta == "consulta oftalmológica")
            {
                HabilitarControlesMedidaVista(false);
                HabilitarControlesOftalmologo(true);
                HabilitarControlesRetinologo(false);
            }

            // 🔹 CONSULTA CON RETINÓLOGO
            else if (motivoConsulta == "consulta con retinólogo")
            {
                HabilitarControlesMedidaVista(false);
                HabilitarControlesOftalmologo(false);
                HabilitarControlesRetinologo(true);
            }

            // 🔹 EXAMEN OCULAR COMPLETO
            else if (motivoConsulta == "exámen ocular completo")
            {
                if (tipoUsuario == "O") // 👓 Optometrista
                {
                    HabilitarControlesMedidaVista(true);
                    HabilitarControlesOftalmologo(false);
                    HabilitarControlesRetinologo(false);
                }
                else if (tipoUsuario == "F") // 👁 Oftalmólogo
                {
                    HabilitarControlesMedidaVista(false);
                    HabilitarControlesOftalmologo(true);
                    HabilitarControlesRetinologo(false);
                }
                else if (tipoUsuario == "R") // 🔬 Retinólogo
                {
                    HabilitarControlesMedidaVista(false);
                    HabilitarControlesOftalmologo(false);
                    HabilitarControlesRetinologo(true);

                }else if(tipoUsuario=="S")
                {
                    HabilitarControlesMedidaVista(true);
                    HabilitarControlesOftalmologo(true);
                    HabilitarControlesRetinologo(true);
                }
            }

            else
            {
                // 🔸 Motivo no reconocido → bloquear todo
                HabilitarControlesMedidaVista(false);
                HabilitarControlesOftalmologo(false);
                HabilitarControlesRetinologo(false);
            }
        }


        private void HabilitarControlesMedidaVista(bool habilitar)
        {
            string[] controles = {
        "txtLEJOSODEsferico","txtLEJOSODCilindrico","txtLEJOSODEje","txtLEJOSODDIP","txtLEJOSODAgudezaVisual",
        "txtLEJOSOIEsferico","txtLEJOSOICilindrico","txtLEJOSOIEje","txtLEJOSOIDIP","txtLEJOSOIAgudezaVisual",
        "txtCERCAODEsferico","txtCERCAODCilindrico","txtCERCAODEje","txtCERCAODDIP","txtCERCAODAgudezaVisual",
        "txtCERCAOIEsferico","txtCERCAOICilindrico","txtCERCAOIEje","txtCERCAOIDIP","txtCERCAOIAgudezaVisual",
        "txtObservaciones1",
        "lblNombreArchivo", // Label del PDF
        "btnSubirPDF"       // Botón de subir PDF
    };

            foreach (string nombre in controles)
            {
                Control[] encontrados = this.Controls.Find(nombre, true);
                if (encontrados.Length == 0) continue;

                Control ctrl = encontrados[0];

                if (ctrl is TextBox txt)
                {
                    txt.ReadOnly = !habilitar;
                }
                else if (ctrl is Label lbl && nombre == "lblNombreArchivo")
                {
                    lbl.Enabled = habilitar;
                    lbl.ForeColor = habilitar ? Color.Black : Color.Gray;
                }
                else if (ctrl is Button btn && nombre == "btnSubirPDF")
                {
                    btn.Enabled = habilitar;
                    btn.Visible = true; // opcional: siempre visible
                }
            }
        }

        private void HabilitarControlesOftalmologo(bool habilitar)
        {
            string[] controles = {
        "txtSignosSintomas1",
        "txtExamenOftalmologico",
        "panelOjoDerecho",
        "panelOjoIzquierdo"
    };

            foreach (string nombre in controles)
            {
                Control[] encontrados = this.Controls.Find(nombre, true);
                if (encontrados.Length == 0) continue;

                if (encontrados[0] is TextBox txt)
                    txt.ReadOnly = !habilitar;
                else if (encontrados[0] is FlowLayoutPanel panel)
                    panel.Enabled = habilitar;
            }
        }

        private void HabilitarControlesRetinologo(bool habilitar)
        {
            /// Controles fuera de la tabla
            string[] controlesExternos = {
        "txtObservacionesDiagnostico",
        "txtTratamiento1",
        "lblNombreArchivo",
        "btnSubirPDF"
    };

            foreach (string nombre in controlesExternos)
            {
                Control[] encontrados = this.Controls.Find(nombre, true);
                if (encontrados.Length == 0) continue;

                Control ctrl = encontrados[0];

                if (ctrl is TextBox txt) txt.ReadOnly = !habilitar;
                else if (ctrl is Label lbl && nombre == "lblNombreArchivo")
                {
                    lbl.Enabled = habilitar;
                    lbl.ForeColor = habilitar ? Color.Black : Color.Gray;
                }
                else if (ctrl is Button btn && nombre == "btnSubirPDF") btn.Enabled = habilitar;
            }

            // Controles dentro de la tabla
            if (tablaDiagnostico != null)
            {
                foreach (Control c in tablaDiagnostico.Controls)
                {
                    if (c is TextBox txt)
                    {
                        txt.ReadOnly = !habilitar; // habilita/deshabilita TextBox
                    }
                    else if (c is DateTimePicker dtp)
                    {
                        dtp.Enabled = habilitar; // habilita/deshabilita DateTimePicker
                    }
                }
            }
        }

        private void CargarDatosExamenOcularCompleto()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                string sql = @"
SELECT 
    Nombre_optometra,
    Nombre_oftalmologo,
    Nombre_retinologo,
    Lejos_OD_Esferico, Lejos_OD_Cilindrico, Lejos_OD_EJE, Lejos_OD_DIP, Lejos_OD_AV,
    Lejos_OI_Esferico, Lejos_OI_Cilindrico, Lejos_OI_EJE, Lejos_OI_DIP, Lejos_OI_AV,
    Cerca_OD_Esferico, Cerca_OD_Cilindrico, Cerca_OD_EJE, Cerca_OD_DIP, Cerca_OD_AV,
    Cerca_OI_Esferico, Cerca_OI_Cilindrico, Cerca_OI_EJE, Cerca_OI_DIP, Cerca_OI_AV,
    Observaciones,
    SignosSintomas, ExamenOftamologico, OjoDerecho, OjoIzquierdo,
    Diagnostico, AV_SC_OD, AV_SC_OI, AV_CC_OD, AV_CC_OI, PIO_OD, PIO_OI,
    Fecha_Diagnostico, Hora_Inicio, Hora_Termino, Tratamiento, NombreArchivo, Archivo
FROM HistorialClinicoBD
WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@Id", idHistorial);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // ==== CARGAR NOMBRES DE LOS DOCTORES ====
                        if (!dr.IsDBNull(dr.GetOrdinal("Nombre_optometra")))
                        {
                            Control[] ctrlOpt = this.Controls.Find("txtOptometro", true);
                            if (ctrlOpt.Length > 0 && ctrlOpt[0] is TextBox txt)
                                txt.Text = dr["Nombre_optometra"].ToString();
                        }

                        if (!dr.IsDBNull(dr.GetOrdinal("Nombre_oftalmologo")))
                        {
                            Control[] ctrlOft = this.Controls.Find("txtDoctorExamenOftalmologico", true);
                            if (ctrlOft.Length > 0 && ctrlOft[0] is TextBox txt)
                                txt.Text = dr["Nombre_oftalmologo"].ToString();
                        }

                        if (!dr.IsDBNull(dr.GetOrdinal("Nombre_retinologo")))
                        {
                            Control[] ctrlRet = this.Controls.Find("txtDoctorDiagnostico", true);
                            if (ctrlRet.Length > 0 && ctrlRet[0] is TextBox txt)
                                txt.Text = dr["Nombre_retinologo"].ToString();
                        }

                        // ==== CARGAR CAMPOS DE CADA SECCIÓN ====
                        CargarCamposOptometra(dr);
                        CargarCamposOftalmologo(dr);
                        CargarCamposRetinologo(dr);
                    }
                }
            }
        }

        
        private void CargarCamposOptometra(SqlDataReader dr)
        {
            string[] campos = {"Nombre_optometra",
        "Lejos_OD_Esferico","Lejos_OD_Cilindrico","Lejos_OD_EJE","Lejos_OD_DIP","Lejos_OD_AV",
        "Lejos_OI_Esferico","Lejos_OI_Cilindrico","Lejos_OI_EJE","Lejos_OI_DIP","Lejos_OI_AV",
        "Cerca_OD_Esferico","Cerca_OD_Cilindrico","Cerca_OD_EJE","Cerca_OD_DIP","Cerca_OD_AV",
        "Cerca_OI_Esferico","Cerca_OI_Cilindrico","Cerca_OI_EJE","Cerca_OI_DIP","Cerca_OI_AV",
        "Observaciones"
    };

            foreach (var campo in campos)
            {
                if (dr[campo] != DBNull.Value)
                {
                    Control[] encontrados = this.Controls.Find(mapaColumnas.FirstOrDefault(x => x.Value == campo).Key, true);
                    if (encontrados.Length > 0 && encontrados[0] is TextBox txt)
                        txt.Text = dr[campo].ToString();
                }
            }
        }

        private void CargarCamposOftalmologo(SqlDataReader dr)
        {
            if (dr["SignosSintomas"] != DBNull.Value)
            {
                Control[] c = this.Controls.Find("txtSignosSintomas1", true);
                if (c.Length > 0 && c[0] is TextBox txt)
                    txt.Text = dr["SignosSintomas"].ToString();
            }

            if (dr["ExamenOftamologico"] != DBNull.Value)
            {
                Control[] c = this.Controls.Find("txtExamenOftalmologico", true);
                if (c.Length > 0 && c[0] is TextBox txt)
                    txt.Text = dr["ExamenOftamologico"].ToString();
            }

            // === Imagen Ojo Derecho ===
            if (dr["OjoDerecho"] != DBNull.Value)
            {
                Control[] panelOD = this.Controls.Find("panelOjoDerecho", true);
                if (panelOD.Length > 0 && panelOD[0] is FlowLayoutPanel panel)
                {
                    PictureBox pic = panel.Controls.OfType<PictureBox>().FirstOrDefault();
                    if (pic != null)
                    {
                        byte[] data = (byte[])dr["OjoDerecho"];
                        using (MemoryStream ms = new MemoryStream(data))
                            pic.Image = Image.FromStream(ms);
                    }
                }
            }

            // === Imagen Ojo Izquierdo ===
            if (dr["OjoIzquierdo"] != DBNull.Value)
            {
                Control[] panelOI = this.Controls.Find("panelOjoIzquierdo", true);
                if (panelOI.Length > 0 && panelOI[0] is FlowLayoutPanel panel)
                {
                    PictureBox pic = panel.Controls.OfType<PictureBox>().FirstOrDefault();
                    if (pic != null)
                    {
                        byte[] data = (byte[])dr["OjoIzquierdo"];
                        using (MemoryStream ms = new MemoryStream(data))
                            pic.Image = Image.FromStream(ms);
                    }
                }
            }
        }

        private void CargarCamposRetinologo(SqlDataReader dr)
        {
            // Nombre del retinólogo
            if (dr["Nombre_retinologo"] != DBNull.Value)
                ((TextBox)this.Controls.Find("txtDoctorDiagnostico", true)[0]).Text = dr["Nombre_retinologo"].ToString();

            // Diagnóstico
            if (dr["Diagnostico"] != DBNull.Value)
                ((TextBox)this.Controls.Find("txtObservacionesDiagnostico", true)[0]).Text = dr["Diagnostico"].ToString();

            var controles = tablaDiagnostico.Controls.Find("txtAVSC_OD", true);
            if (controles.Length > 0 && dr["AV_SC_OD"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["AV_SC_OD"].ToString();

            controles = tablaDiagnostico.Controls.Find("txtAVSC_OI", true);
            if (controles.Length > 0 && dr["AV_SC_OI"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["AV_SC_OI"].ToString();

            // AV.CC.
            controles = tablaDiagnostico.Controls.Find("txtAVCC_OD", true);
            if (controles.Length > 0 && dr["AV_CC_OD"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["AV_CC_OD"].ToString();

            controles = tablaDiagnostico.Controls.Find("txtAVCC_OI", true);
            if (controles.Length > 0 && dr["AV_CC_OI"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["AV_CC_OI"].ToString();

            // PIO/ICARE
            controles = tablaDiagnostico.Controls.Find("txtPIOICARE_OD", true);
            if (controles.Length > 0 && dr["PIO_OD"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["PIO_OD"].ToString();

            controles = tablaDiagnostico.Controls.Find("txtPIOICARE_OI", true);
            if (controles.Length > 0 && dr["PIO_OI"] != DBNull.Value)
                ((TextBox)controles[0]).Text = dr["PIO_OI"].ToString();

            // FECHA
            controles = tablaDiagnostico.Controls.Find("dtpFechaDiagnostico", true);
            if (controles.Length > 0 && dr["Fecha_Diagnostico"] != DBNull.Value)
                ((DateTimePicker)controles[0]).Value = Convert.ToDateTime(dr["Fecha_Diagnostico"]);

            // HORA DE INICIO
            controles = tablaDiagnostico.Controls.Find("dtpHoraInicio", true);
            if (controles.Length > 0 && dr["Hora_Inicio"] != DBNull.Value)
                ((DateTimePicker)controles[0]).Value = DateTime.Today.Add(TimeSpan.Parse(dr["Hora_Inicio"].ToString()));

            // HORA DE TÉRMINO
            controles = tablaDiagnostico.Controls.Find("dtpHoraTermino", true);
            if (controles.Length > 0 && dr["Hora_Termino"] != DBNull.Value)
                ((DateTimePicker)controles[0]).Value = DateTime.Today.Add(TimeSpan.Parse(dr["Hora_Termino"].ToString()));

            // Tratamiento
            if (dr["Tratamiento"] != DBNull.Value)
                ((TextBox)this.Controls.Find("txtTratamiento1", true)[0]).Text = dr["Tratamiento"].ToString();

            // === 📎 ARCHIVO PDF (label y bytes) ===
            Control[] ctrlLblArchivo = this.Controls.Find("lblNombreArchivo", true);
            if (ctrlLblArchivo.Length > 0 && ctrlLblArchivo[0] is Label lblArchivo)
            {
                bool tieneColumnaArchivo = false;

                // Verificar si la columna existe (por seguridad)
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (string.Equals(dr.GetName(i), "NombreArchivo", StringComparison.OrdinalIgnoreCase))
                    {
                        tieneColumnaArchivo = true;
                        break;
                    }
                }

                if (tieneColumnaArchivo && dr["NombreArchivo"] != DBNull.Value)
                {
                    lblArchivo.Text = dr["NombreArchivo"].ToString();
                }
                else if (dr["Archivo"] != DBNull.Value)
                {
                    byte[] archivoBytes = (byte[])dr["Archivo"];
                    lblArchivo.Text = $"Archivo cargado ({archivoBytes.Length} bytes)";
                }
                else
                {
                    lblArchivo.Text = string.Empty;
                }
            }
        }


        private void EditarPorMotivo(int id)
        {
            Control[] encontradosMotivo = this.Controls.Find("cmbMotivoConsulta1", true);
            if (encontradosMotivo.Length == 0)
            {
                MessageBox.Show("No se encontró el campo MotivoConsulta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string motivo = "";

            if (encontradosMotivo[0] is TextBox txtMotivo)
            {
                motivo = txtMotivo.Text.Trim().ToLower();
            }
            else if (encontradosMotivo[0] is ComboBox cmbMotivo)
            {
                motivo = cmbMotivo.SelectedItem != null ? cmbMotivo.SelectedItem.ToString().Trim().ToLower() : "";
            }

            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("No se ha seleccionado un motivo de consulta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (motivo == "medida de la vista")
            {

                try
                {
                    using (SqlConnection cn = conexionBD.Conectar())
                    {
                        cn.Open();

                        string query = @"
                UPDATE HistorialClinicoBD
                SET 
                    Lejos_OD_Esferico = @Lejos_OD_Esferico,
                    Lejos_OD_Cilindrico = @Lejos_OD_Cilindrico,
                    Lejos_OD_EJE = @Lejos_OD_EJE,
                    Lejos_OD_DIP = @Lejos_OD_DIP,
                    Lejos_OD_AV = @Lejos_OD_AV,
                    Lejos_OI_Esferico = @Lejos_OI_Esferico,
                    Lejos_OI_Cilindrico = @Lejos_OI_Cilindrico,
                    Lejos_OI_EJE = @Lejos_OI_EJE,
                    Lejos_OI_DIP = @Lejos_OI_DIP,
                    Lejos_OI_AV = @Lejos_OI_AV,
                    Cerca_OD_Esferico = @Cerca_OD_Esferico,
                    Cerca_OD_Cilindrico = @Cerca_OD_Cilindrico,
                    Cerca_OD_EJE = @Cerca_OD_EJE,
                    Cerca_OD_DIP = @Cerca_OD_DIP,
                    Cerca_OD_AV = @Cerca_OD_AV,
                    Cerca_OI_Esferico = @Cerca_OI_Esferico,
                    Cerca_OI_Cilindrico = @Cerca_OI_Cilindrico,
                    Cerca_OI_EJE = @Cerca_OI_EJE,
                    Cerca_OI_DIP = @Cerca_OI_DIP,
                    Cerca_OI_AV = @Cerca_OI_AV,
                    Observaciones = @Observaciones,
                    NombreArchivo=@NombreArchivo,
                    Archivo=@Archivo
                WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            var tabla = this.Controls.Find("tblCorrectores", true).FirstOrDefault() as TableLayoutPanel;

                            if (tabla != null)
                            {
                                cmd.Parameters.AddWithValue("@Lejos_OD_Esferico", GetDecimalFromTextBox(tabla, "txtLEJOSODEsferico"));
                                cmd.Parameters.AddWithValue("@Lejos_OD_Cilindrico", GetDecimalFromTextBox(tabla, "txtLEJOSODCilindrico"));
                                cmd.Parameters.AddWithValue("@Lejos_OD_EJE", GetDecimalFromTextBox(tabla, "txtLEJOSODEje"));
                                cmd.Parameters.AddWithValue("@Lejos_OD_DIP", GetDecimalFromTextBox(tabla, "txtLEJOSODDIP"));
                                cmd.Parameters.AddWithValue("@Lejos_OD_AV", GetDecimalFromTextBox(tabla, "txtLEJOSODAgudezaVisual"));

                                cmd.Parameters.AddWithValue("@Lejos_OI_Esferico", GetDecimalFromTextBox(tabla, "txtLEJOSOIEsferico"));
                                cmd.Parameters.AddWithValue("@Lejos_OI_Cilindrico", GetDecimalFromTextBox(tabla, "txtLEJOSOICilindrico"));
                                cmd.Parameters.AddWithValue("@Lejos_OI_EJE", GetDecimalFromTextBox(tabla, "txtLEJOSOIEje"));
                                cmd.Parameters.AddWithValue("@Lejos_OI_DIP", GetDecimalFromTextBox(tabla, "txtLEJOSOIDIP"));
                                cmd.Parameters.AddWithValue("@Lejos_OI_AV", GetDecimalFromTextBox(tabla, "txtLEJOSOIAgudezaVisual"));

                                cmd.Parameters.AddWithValue("@Cerca_OD_Esferico", GetDecimalFromTextBox(tabla, "txtCERCAODEsferico"));
                                cmd.Parameters.AddWithValue("@Cerca_OD_Cilindrico", GetDecimalFromTextBox(tabla, "txtCERCAODCilindrico"));
                                cmd.Parameters.AddWithValue("@Cerca_OD_EJE", GetDecimalFromTextBox(tabla, "txtCERCAODEje"));
                                cmd.Parameters.AddWithValue("@Cerca_OD_DIP", GetDecimalFromTextBox(tabla, "txtCERCAODDIP"));
                                cmd.Parameters.AddWithValue("@Cerca_OD_AV", GetDecimalFromTextBox(tabla, "txtCERCAODAgudezaVisual"));

                                cmd.Parameters.AddWithValue("@Cerca_OI_Esferico", GetDecimalFromTextBox(tabla, "txtCERCAOIEsferico"));
                                cmd.Parameters.AddWithValue("@Cerca_OI_Cilindrico", GetDecimalFromTextBox(tabla, "txtCERCAOICilindrico"));
                                cmd.Parameters.AddWithValue("@Cerca_OI_EJE", GetDecimalFromTextBox(tabla, "txtCERCAOIEje"));
                                cmd.Parameters.AddWithValue("@Cerca_OI_DIP", GetDecimalFromTextBox(tabla, "txtCERCAOIDIP"));
                                cmd.Parameters.AddWithValue("@Cerca_OI_AV", GetDecimalFromTextBox(tabla, "txtCERCAOIAgudezaVisual"));

                                                               
                            }

                            cmd.Parameters.AddWithValue("@Id", id);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Medida de la vista actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar la medida de la vista: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else if(motivo== "consulta oftalmológica")
            {

            }else if(motivo== "consulta con retinólogo")
            {

            }else if(motivo== "exámen ocular completo")
            {

            }else
            {
                MessageBox.Show("Error al editar No se encontro el motivo de consulta");
            }
        }
        private decimal GetDecimalFromTextBox(TableLayoutPanel tabla, string nombre)
        {
            var controles = tabla.Controls.Find(nombre, true);
            if (controles.Length > 0 && decimal.TryParse(((TextBox)controles[0]).Text, out decimal valor))
                return valor;
            return 0m;
        }
    }



}
