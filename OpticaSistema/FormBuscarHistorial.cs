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
    public partial class FormBuscarHistorial : Form
    {
        private ConexionDB conexionBD;
        private DataGridView tablaUsuarios;
        private int pageNumber = 1;   // Página actual
        private int pageSize = 20;    // Registros por página
        private int totalRecords = 0; // Total de registros
        private int totalPages = 0;

        private TableLayoutPanel layout;
        private TableLayoutPanel panelFiltros;
        private TableLayoutPanel panelPaginacion;

        private ComboBox comboTipoBusqueda;
        private TextBox txtDni;
        private DateTimePicker dtpFecha;

        private Button btnBuscar;
        private Button btnActualizar;
        private Label lblEstado;
        private ComboBox comboEstado;
        private Button btnAceptar;

        private Button btnAnterior;
        private Label lblPagina;
        private Button btnSiguiente;

        private Panel panelNavegacion;


        public FormBuscarHistorial()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            this.WindowState = FormWindowState.Maximized;
            conexionBD = new ConexionDB();
            this.BackColor = Color.White;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            // === CONTENEDOR PRINCIPAL ===
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.RowStyles.Clear();
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // Título
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // Panel de filtros
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // 👈 Tabla expansible
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // Paginación
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // Botón REGISTRAR
            layout.Padding = new Padding(20);
            this.Controls.Add(layout);

            // === TÍTULO ===
            Label titulo = new Label();
            titulo.Text = "BUSCAR HISTORIAL CLÍNICO";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.DarkBlue;
            titulo.Dock = DockStyle.Fill;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.AutoSize = true; // 👈 evita que se corte
            titulo.Margin = new Padding(0, 60, 0, 20);
            layout.Controls.Add(titulo, 0, 0);

            // === PANEL DE FILTROS ===
            TableLayoutPanel panelFiltros = new TableLayoutPanel();
            panelFiltros.Dock = DockStyle.Fill;
            panelFiltros.AutoSize = true;
            panelFiltros.ColumnCount = 7;
            panelFiltros.RowCount = 1;
            panelFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // altura uniforme
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // ComboBox tipo búsqueda
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));  // Entrada
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // Botón Buscar
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // Botón Actualizar
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // Label Estado
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));  // ComboBox Estado
            panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // Botón Aceptar
            layout.Controls.Add(panelFiltros, 0, 1);

            // ComboBox tipo búsqueda
            comboTipoBusqueda = new ComboBox();
            comboTipoBusqueda.Items.AddRange(new string[] { "DNI", "Fecha" });
            comboTipoBusqueda.SelectedIndex = 0;
            comboTipoBusqueda.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            comboTipoBusqueda.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTipoBusqueda.Anchor = AnchorStyles.Left;
            comboTipoBusqueda.Margin = new Padding(5, 5, 5, 5);
            comboTipoBusqueda.Height = 40;
            panelFiltros.Controls.Add(comboTipoBusqueda, 0, 0);

            // Panel dinámico para entrada
            Panel panelEntrada = new Panel();
            panelEntrada.Dock = DockStyle.Fill;
            txtDni = new TextBox { Font = new Font("Segoe UI", 11), Dock = DockStyle.Fill, Height = 40};
            dtpFecha = new DateTimePicker { Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill, Visible = false, Height = 40 };
            panelEntrada.Margin = new Padding(0, 8, 0, 0);
            panelEntrada.Controls.Add(txtDni);
            panelEntrada.Controls.Add(dtpFecha);
            panelFiltros.Controls.Add(panelEntrada, 1, 0);

            // Evento cambio de búsqueda
            comboTipoBusqueda.SelectedIndexChanged += (s, e) =>
            {
                txtDni.Visible = comboTipoBusqueda.SelectedItem.ToString() == "DNI";
                dtpFecha.Visible = !txtDni.Visible;
            };

            // Botón Buscar
            btnBuscar = new Button();
            btnBuscar.Text = "BUSCAR";
            btnBuscar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBuscar.BackColor = Color.SteelBlue;
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Anchor = AnchorStyles.Left;
            btnBuscar.Margin = new Padding(5, 5, 5, 5);
            btnBuscar.MinimumSize = new Size(110, 40);
            panelFiltros.Controls.Add(btnBuscar, 4, 0);

            // Botón Actualizar
            btnActualizar = new Button();
            btnActualizar.Text = "ACTUALIZAR";
            btnActualizar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnActualizar.BackColor = Color.SteelBlue;
            btnActualizar.ForeColor = Color.White;
            btnActualizar.FlatStyle = FlatStyle.Flat;
            btnActualizar.Anchor = AnchorStyles.Left;
            btnActualizar.Margin = new Padding(5, 5, 5, 5);
            btnActualizar.MinimumSize = new Size(150, 40);
            panelFiltros.Controls.Add(btnActualizar, 5, 0);

            // Label Estado
            lblEstado = new Label();
            lblEstado.Text = "Estado:";
            lblEstado.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEstado.Anchor = AnchorStyles.Left;
            lblEstado.TextAlign = ContentAlignment.MiddleCenter; // 👈 centrado vertical
            lblEstado.Margin = new Padding(5, 5, 5, 5);
            panelFiltros.Controls.Add(lblEstado, 2, 0);

            // ComboBox Estado
            comboEstado = new ComboBox();
            comboEstado.Items.AddRange(new string[] { "", "Registrado", "Falta Registrar" });
            comboEstado.SelectedIndex = 0;
            comboEstado.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            comboEstado.Anchor = AnchorStyles.Left;
            comboEstado.Margin = new Padding(0, 5, 5, 5);
            comboEstado.MinimumSize = new Size(150, 40);
            comboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            panelFiltros.Controls.Add(comboEstado, 3, 0);

            
            // === TABLA DE USUARIOS ===
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

            // Columnas
            tablaUsuarios.Columns.Add("DNI", "DNI");
            tablaUsuarios.Columns.Add("NOMBRE", "NOMBRE");
            tablaUsuarios.Columns.Add("FECHA", "FECHA");
            tablaUsuarios.Columns.Add("ESTADO", "ESTADO");

            // Botón Editar
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
            colEditar.HeaderText = "EDITAR";
            colEditar.Text = "✏️"; // Puedes usar texto o dejarlo vacío
            colEditar.UseColumnTextForButtonValue = true;
            colEditar.FlatStyle = FlatStyle.Flat;
            colEditar.DefaultCellStyle.BackColor = Color.White;
            colEditar.DefaultCellStyle.ForeColor = Color.Black;
            tablaUsuarios.Columns.Add(colEditar);

            // Botón Eliminar
            DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
            colEliminar.HeaderText = "ELIMINAR";
            colEliminar.Text = "🗑️"; // Puedes usar texto o dejarlo vacío
            colEliminar.UseColumnTextForButtonValue = true;
            colEliminar.FlatStyle = FlatStyle.Flat;
            colEliminar.DefaultCellStyle.BackColor = Color.White;
            colEliminar.DefaultCellStyle.ForeColor = Color.Black;
            tablaUsuarios.Columns.Add(colEliminar);

            layout.Controls.Add(tablaUsuarios, 0, 2);

            // === PANEL DE PAGINACIÓN ===
            panelPaginacion = new TableLayoutPanel();
            panelPaginacion.Dock = DockStyle.Fill;
            panelPaginacion.ColumnCount = 3;
            panelPaginacion.RowCount = 1;
            panelPaginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            panelPaginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            panelPaginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            // Botón Anterior
            btnAnterior = new Button();
            btnAnterior.Text = "Anterior";
            btnAnterior.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAnterior.BackColor = Color.SteelBlue;
            btnAnterior.ForeColor = Color.White;
            btnAnterior.Dock = DockStyle.Right;
            btnAnterior.MinimumSize = new Size(120, 35);
            btnAnterior.Click += btnAnterior_Click;
            panelPaginacion.Controls.Add(btnAnterior, 0, 0);

            // Label Página
            lblPagina = new Label();
            lblPagina.Text = "Página 1 de " + totalPages;
            lblPagina.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            lblPagina.Dock = DockStyle.Fill;
            panelPaginacion.Controls.Add(lblPagina, 1, 0);

            // Botón Siguiente
            btnSiguiente = new Button();
            btnSiguiente.Text = "Siguiente";
            btnSiguiente.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSiguiente.BackColor = Color.SteelBlue;
            btnSiguiente.ForeColor = Color.White;
            btnSiguiente.Dock = DockStyle.Left;
            btnSiguiente.MinimumSize = new Size(120, 35);
            btnSiguiente.Click += btnSiguiente_Click;
            panelPaginacion.Controls.Add(btnSiguiente, 2, 0);

            layout.Controls.Add(panelPaginacion, 0, 3);

            CargarUsuarios();

            btnBuscar.Click += (s, e) =>
            {
                pageNumber = 1; // reinicia a la primera página
                BuscarUsuarios();
            };

            btnActualizar.Click += (s, e) =>
            {
                // Reinicia filtros
                comboTipoBusqueda.SelectedIndex = 0;   // vuelve a "DNI"
                txtDni.Text = string.Empty;            // limpia el campo DNI
                dtpFecha.Value = DateTime.Today;       // resetea la fecha
                txtDni.Visible = true;                 // muestra el txtDni
                dtpFecha.Visible = false;              // oculta el DateTimePicker
                comboEstado.SelectedIndex = 0;         // estado vacío

                // Reinicia paginación
                pageNumber = 1;

                // Vuelve a listar todo
                BuscarUsuarios();
            };


            tablaUsuarios.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (tablaUsuarios.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    string dni = tablaUsuarios.Rows[e.RowIndex].Cells["DNI"].Value?.ToString();

                    if (tablaUsuarios.Columns[e.ColumnIndex].HeaderText == "ELIMINAR")
                    {
                        DialogResult result = MessageBox.Show(
                            $"¿Seguro que deseas eliminar el historial del paciente con DNI {dni}?",
                            "Confirmar eliminación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result == DialogResult.Yes)
                        {
                            EliminarHistorial(dni);
                            BuscarUsuarios(); // refresca la grilla
                        }
                    }
                }
            };




        }
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (pageNumber > 1)
            {
                pageNumber--;
                CargarUsuarios();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (pageNumber < totalPages)
            {
                pageNumber++;
                CargarUsuarios();
            }
        }

        private void FormBuscarHistorial_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - BuscarHistorial";
            this.Icon = new Icon("Imagenes/log.ico");
        }
        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void CargarUsuarios()
        {
            using(SqlConnection cn = conexionBD.Conectar())
            {
                cn.Open();

                // 1. Contar total de registros
                string countQuery = @"
            SELECT COUNT(*) 
            FROM HistorialClinicoBD h
            INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
            WHERE h.Estado = 1";

                SqlCommand countCmd = new SqlCommand(countQuery, cn);
                totalRecords = (int)countCmd.ExecuteScalar();
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // 2. Traer registros de la página actual
                string query = @"
            SELECT 
                p.Dni,
                (p.Nombres + ' ' + p.Apellidos) AS NombreCompleto,
                h.FechaConsulta,
                CASE WHEN h.EstadoHistorial = 1 THEN 'FALTA REGISTRAR' ELSE 'REGISTRADO' END AS Estado
            FROM HistorialClinicoBD h
            INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
            WHERE h.Estado = 1
            ORDER BY h.FechaConsulta DESC
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                tablaUsuarios.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    tablaUsuarios.Rows.Add(
                        row["Dni"].ToString(),
                        row["NombreCompleto"].ToString(),
                        Convert.ToDateTime(row["FechaConsulta"]).ToString("dd/MM/yyyy HH:mm"),
                        row["Estado"].ToString()
                    );
                }

                // 3. Actualizar label de estado
                lblPagina.Text = $"Página {pageNumber} de {totalPages}";
            }



        }

        private void BuscarUsuarios()
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                cn.Open();

                // === Construcción dinámica del WHERE ===
                string where = "WHERE h.Estado = 1"; // base

                // Filtro por tipo de búsqueda
                if (comboTipoBusqueda.SelectedItem.ToString() == "DNI" && !string.IsNullOrWhiteSpace(txtDni.Text))
                {
                    where += " AND p.Dni LIKE @Dni";
                }
                else if (comboTipoBusqueda.SelectedItem.ToString() == "Fecha")
                {
                    where += " AND CAST(h.FechaConsulta AS DATE) = @Fecha";
                }

                // Filtro por estado
                if (comboEstado.SelectedItem.ToString() == "Falta Registrar")
                {
                    where += " AND h.EstadoHistorial = 1";
                }
                else if (comboEstado.SelectedItem.ToString() == "Registrado")
                {
                    where += " AND h.EstadoHistorial = 0";
                }

                // === Contar total de registros ===
                string countQuery = $@"
            SELECT COUNT(*) 
            FROM HistorialClinicoBD h
            INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
            {where}";

                SqlCommand countCmd = new SqlCommand(countQuery, cn);

                // Parámetros
                if (where.Contains("@Dni"))
                    countCmd.Parameters.AddWithValue("@Dni", "%" + txtDni.Text.Trim() + "%");
                if (where.Contains("@Fecha"))
                    countCmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value.Date);

                totalRecords = (int)countCmd.ExecuteScalar();
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // === Consulta principal con paginación ===
                string query = $@"
            SELECT 
                p.Dni,
                (p.Nombres + ' ' + p.Apellidos) AS NombreCompleto,
                h.FechaConsulta,
                CASE WHEN h.EstadoHistorial = 1 THEN 'FALTA REGISTRAR' ELSE 'REGISTRADO' END AS Estado
            FROM HistorialClinicoBD h
            INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
            {where}
            ORDER BY h.FechaConsulta DESC
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                // Reusar parámetros
                if (where.Contains("@Dni"))
                    cmd.Parameters.AddWithValue("@Dni", "%" + txtDni.Text.Trim() + "%");
                if (where.Contains("@Fecha"))
                    cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // === Cargar en la grilla ===
                tablaUsuarios.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    tablaUsuarios.Rows.Add(
                        row["Dni"].ToString(),
                        row["NombreCompleto"].ToString(),
                        Convert.ToDateTime(row["FechaConsulta"]).ToString("dd/MM/yyyy HH:mm"),
                        row["Estado"].ToString()
                    );
                }

                lblPagina.Text = $"Página {pageNumber} de {totalPages}";
            }
        }

        private void EliminarHistorial(string dni)
        {
            using (SqlConnection cn = conexionBD.Conectar())
            {
                cn.Open();

                string query = @"
            UPDATE h
            SET h.Estado = 0
            FROM HistorialClinicoBD h
            INNER JOIN PacienteBD p ON h.IdPaciente = p.Id
            WHERE p.Dni = @Dni";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Dni", dni);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        MessageBox.Show("Historial eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se encontró el historial a eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void InicializarPaginacionUI()
        {
            // Panel de navegación
            panelNavegacion = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.SteelBlue
            };

            // Botón Anterior
            btnAnterior = new Button
            {
                Name = "btnAnterior",
                Text = "Anterior",
                Width = 100,
                Left = 10,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Top = 2
            };
            btnAnterior.Click += btnAnterior_Click;

            // Label Página (declarado a nivel de clase)
            lblPagina = new Label
            {
                Name = "lblPagina",
                Text = "Página 0 de 0",
                AutoSize = true,
                Top = 12
            };
            // Centrado relativo dentro del panel
            lblPagina.Left = (panelNavegacion.Width / 2) - 40;
            lblPagina.Anchor = AnchorStyles.Top;

            // Botón Siguiente
            btnSiguiente = new Button
            {
                Name = "btnSiguiente",
                Text = "Siguiente",
                Width = 100,
                Height = 35,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Top = 2
            };
            // Ubicación a la derecha
            btnSiguiente.Left = panelNavegacion.Width - 110;
            btnSiguiente.Anchor = AnchorStyles.Right;
            btnSiguiente.Click += btnSiguiente_Click;

            // Agregar controles al panel
            panelNavegacion.Controls.Add(btnAnterior);
            panelNavegacion.Controls.Add(lblPagina);
            panelNavegacion.Controls.Add(btnSiguiente);

            // Agregar panel al formulario
            this.Controls.Add(panelNavegacion);

            // Ajuste de reposicionamiento al cambiar tamaño
            panelNavegacion.Resize += (s, e) =>
            {
                lblPagina.Left = (panelNavegacion.Width - lblPagina.Width) / 2;
                btnSiguiente.Left = panelNavegacion.Width - btnSiguiente.Width - 10;
            };
        }
        

    }
}
