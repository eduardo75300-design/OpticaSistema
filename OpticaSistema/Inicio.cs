using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpticaSistema.Login;

namespace OpticaSistema
{
    public partial class Inicio : Form
    {
              public Inicio()
        {
            InitializeComponent();
            this.FormClosing += Inicio_FormClosing;
            CrearMenuSuperiorAdaptable();
            // Establece el formulario en pantalla completa
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            


        }

        private void CrearMenuSuperiorAdaptable()
        {
            // Panel contenedor del menú
            Panel barraNav = new Panel();
            barraNav.Dock = DockStyle.Top;
            barraNav.Height = 60;
            barraNav.BackColor = Color.SteelBlue;
            this.Controls.Add(barraNav);

            // TableLayoutPanel para distribución automática
            TableLayoutPanel menuLayout = new TableLayoutPanel();
            menuLayout.Dock = DockStyle.Fill;
            menuLayout.ColumnCount = 6;
            menuLayout.RowCount = 1;
            menuLayout.BackColor = Color.Transparent;
            menuLayout.ColumnStyles.Clear();

            // Distribución proporcional: 20% para nombre, 16% para cada opción
            menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Nombre del usuario
            for (int i = 1; i < 6; i++)
            {
                menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F)); // Opciones del menú
            }

            barraNav.Controls.Add(menuLayout);

            // 👤 Nombre del usuario
            Label lblNombre = new Label();
            lblNombre.Text = $"👤 {SesionUsuario.Nombre}";
            lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblNombre.ForeColor = Color.White;
            lblNombre.Dock = DockStyle.Fill;
            lblNombre.TextAlign = ContentAlignment.MiddleLeft;
            lblNombre.Margin = new Padding(10, 0, 0, 0);
            lblNombre.AutoSize = false;
            menuLayout.Controls.Add(lblNombre, 0, 0);

            // Opciones del menú
            string[] secciones = {
        "INICIO",
        "HISTORIAL CLÍNICO",
        "REGISTRO DE PACIENTE",
        "ADMINISTRACIÓN USUARIO",
        "CERRAR SECCIÓN"
    };

            for (int i = 0; i < secciones.Length; i++)
            {
                Label lbl = new Label();
                lbl.Text = secciones[i];
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lbl.ForeColor = Color.White;
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Cursor = Cursors.Hand;
                lbl.Margin = new Padding(0);
                lbl.AutoSize = false;

                lbl.Click += (s, e) =>
                {
                    Form destino = null;

                    switch (lbl.Text)
                    {
                        case "INICIO":
                            if (this is Inicio) return;
                            destino = new Inicio();
                            break;

                        /*case "HISTORIAL CLÍNICO":
                            if (this is FormHistorialClinico) return;
                            destino = new FormHistorialClinico();
                            break;

                        case "REGISTRO DE PACIENTE":
                            if (this is FormRegistroPaciente) return;
                            destino = new FormRegistroPaciente();
                            break;*/

                        case "ADMINISTRACIÓN USUARIO":
                            if (this is FormAdministracionUsuario) return;
                            destino = new FormAdministracionUsuario();
                            break;

                        case "CERRAR SECCIÓN":
                            Application.Exit();
                            return;
                    }

                    if (destino != null)
                    {
                        destino.WindowState = FormWindowState.Maximized;
                        destino.Show();
                        this.Hide();
                    }
                };

                menuLayout.Controls.Add(lbl, i + 1, 0); // +1 porque la columna 0 es para el nombre
            }


        }
        private void Inicio_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Menú";
            this.Icon = new Icon("Imagenes/log.ico");
        }


    }
}
