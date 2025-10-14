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

namespace OpticaSistema
{
    public partial class FormRegistrarHistorial : Form
    {
        private ConexionDB conexionBD;
        private FlowLayoutPanel panelHorizontal;
        public FormRegistrarHistorial()
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
                "Evaluación de la vista",
                "Molestias oculares",
                "Control rutinario",
                "Cambio de lentes",
                "Dolor o irritación ocular",
                "Visión borrosa",
                "Otro"
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
            txtObservaciones.Name = "txtObservaciones";
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
            txtSignos.Name = "txtSignosSintomas";
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
            txtTratamiento.Name = "txtTratamiento";
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
                        MessageBox.Show("Archivo seleccionado:\n" + rutaPDF, "Archivo cargado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Aquí podrías guardar la ruta o subir el archivo a la BD si lo deseas
                    }
                }
            };

            // Agregar controles al panel
            panelAdjuntar.Controls.Add(lblAdjuntar);
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

                        // Lista de campos de paciente
                        string[] campos = new string[]
                        {
                            "Apellidos", "Nombres", "Dni"
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
        }

        private Control CrearPanelDibujoOjo(string titulo)
        {
            // --- Panel principal ---
            FlowLayoutPanel panelOjo = new FlowLayoutPanel();
            panelOjo.FlowDirection = FlowDirection.TopDown;
            panelOjo.Width = 390;
            panelOjo.AutoSize = true;
            panelOjo.Margin = new Padding(10);

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
            // FlowLayoutPanel para el título y la tabla+textarea
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
            chkMostrar.Checked = true; // Por defecto visible
            chkMostrar.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            chkMostrar.AutoSize = true;
            chkMostrar.Margin = new Padding(10, 13, 0, 0);

            // Añadir al subpanel
            panelTituloTexto.Controls.Add(lblTitulo);
            panelTituloTexto.Controls.Add(txtDiagnosticoTitulo);

            panelReceta.Controls.Add(chkMostrar);
            panelReceta.Controls.Add(panelTituloTexto);


            // --- Contenedor con 2 columnas: tabla (izq) + textarea (der) ---
            TableLayoutPanel contenedor = new TableLayoutPanel();
            contenedor.ColumnCount = 2;
            contenedor.RowCount = 1;
            contenedor.AutoSize = true;
            contenedor.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            // Columnas con porcentaje: tabla 55%, textarea 45% -> ajustamos al 50%-50% para que textarea sea más ancho
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // tabla 45%
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); // textarea 55%

            // --- Tabla Diagnóstico ---
            TableLayoutPanel tablaDiagnostico = new TableLayoutPanel();
            tablaDiagnostico.Name = "tblDiagnostico";
            tablaDiagnostico.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablaDiagnostico.BackColor = Color.WhiteSmoke;
            tablaDiagnostico.Width = 480;
            tablaDiagnostico.Height = 220;
            tablaDiagnostico.AutoSize = true;
            tablaDiagnostico.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Columnas: Campo | OD | OI
            tablaDiagnostico.ColumnCount = 3;
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Campo
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // OD
            tablaDiagnostico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // OI

            string[] campos = { "AV.SC.", "AV.CC.", "PIO/ICARE", "FECHA:", "HORA DE INICIO:", "HORA DE TÉRMINO:" };
            tablaDiagnostico.RowCount = campos.Length + 1;
            tablaDiagnostico.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // cabecera

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

            // --- Filas ---
            for (int r = 0; r < campos.Length; r++)
            {
                int fila = r + 1;
                Label lblCampo = new Label();
                lblCampo.Text = campos[r];
                lblCampo.Dock = DockStyle.Fill;
                lblCampo.TextAlign = ContentAlignment.MiddleCenter;
                lblCampo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                tablaDiagnostico.Controls.Add(lblCampo, 0, fila);

                if (campos[r] == "FECHA:")
                {
                    DateTimePicker dtpFecha = new DateTimePicker();
                    dtpFecha.Format = DateTimePickerFormat.Short;
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
                    dtpHora.Dock = DockStyle.Fill;
                    dtpHora.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(dtpHora, 1, fila);
                    tablaDiagnostico.SetColumnSpan(dtpHora, 2);
                }
                else
                {
                    TextBox txtOD = new TextBox();
                    txtOD.Dock = DockStyle.Fill;
                    txtOD.TextAlign = HorizontalAlignment.Center;
                    txtOD.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(txtOD, 1, fila);

                    TextBox txtOI = new TextBox();
                    txtOI.Dock = DockStyle.Fill;
                    txtOI.TextAlign = HorizontalAlignment.Center;
                    txtOI.Font = new Font("Segoe UI", 11);
                    tablaDiagnostico.Controls.Add(txtOI, 2, fila);
                }
            }

            // --- TextArea al costado ---
            TextBox txtObservaciones = new TextBox();
            txtObservaciones.Multiline = true;
            txtObservaciones.ScrollBars = ScrollBars.Vertical;
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Font = new Font("Segoe UI", 11);
            txtObservaciones.Height = 220; // mismo alto que la tabla
            txtObservaciones.Name = "txtObservacionesDiagnostico";

            // Calcular margen izquierdo como 5% del ancho del contenedor
            int margenIzquierdo = (int)(contenedor.Width * 0.05);
            txtObservaciones.Margin = new Padding(margenIzquierdo, 0, 0, 0);

            // Ajustar margen dinámicamente al cambiar tamaño del contenedor
            contenedor.Resize += (s, e) =>
            {
                txtObservaciones.Margin = new Padding((int)(contenedor.Width * 0.05), 0, 0, 0);
            };

            // Agregar tabla y textarea al contenedor
            contenedor.Controls.Add(tablaDiagnostico, 0, 0);
            contenedor.Controls.Add(txtObservaciones, 1, 0);

            // Agregar al panel principal
            panelReceta.Controls.Add(contenedor);
            // --- Evento del CheckBox ---
            chkMostrar.CheckedChanged += (s, e) =>
            {
                panelReceta.SuspendLayout();
                contenedor.SuspendLayout();

                if (chkMostrar.Checked)
                {
                    // --- Volver a mostrar la tabla ---
                    contenedor.Controls.Clear();
                    
                    contenedor.ColumnStyles.Clear();
                    contenedor.ColumnCount = 2;
                    contenedor.RowCount = 1;
                    contenedor.AutoSize = true;
                    contenedor.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // tabla
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f)); // textarea


                    contenedor.Controls.Add(tablaDiagnostico, 0, 0);
                    contenedor.Controls.Add(txtObservaciones, 1, 0);

                    // Restaurar ancho automático del textarea
                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Width = tablaDiagnostico.Width-90;
                    txtObservaciones.Margin = new Padding(10, 0, 0, 0); // margen pequeño opcional
                }
                else
                {
                    // --- Ocultar la tabla y expandir el textarea ---
                    contenedor.Controls.Clear();
                    contenedor.ColumnCount = 1;
                    contenedor.ColumnStyles.Clear();
                    contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                    contenedor.Controls.Add(txtObservaciones, 0, 0);

                    // Asegurar que ocupe todo el ancho visible
                    txtObservaciones.Dock = DockStyle.Fill;
                    txtObservaciones.Margin = new Padding(0);
                    txtObservaciones.Width = panelReceta.Width - 40; // compensar márgenes
                }

                contenedor.ResumeLayout();
                panelReceta.ResumeLayout();
            };

            return panelReceta;
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
            txtOptometro.Name = "txtOptómetro";
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
            Control[] txtMotivo = panelHorizontal.Controls.Find("txtMotivoConsulta", true);
            if (txtMotivo.Length > 0 && txtMotivo[0] is TextBox)
            {
                ((TextBox)txtMotivo[0]).Text = string.Empty;
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
    }
}
