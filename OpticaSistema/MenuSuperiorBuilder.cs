using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpticaSistema.Login;

namespace OpticaSistema
{
    public static class MenuSuperiorBuilder
    {
        public static void CrearMenuSuperiorAdaptable(Form formulario)
        {
            // Panel contenedor del menú
            Panel barraNav = new Panel();
            barraNav.Dock = DockStyle.Top;
            barraNav.Height = 60;
            barraNav.BackColor = Color.SteelBlue;
            formulario.Controls.Add(barraNav);

            // TableLayoutPanel para distribución automática
            TableLayoutPanel menuLayout = new TableLayoutPanel();
            menuLayout.Dock = DockStyle.Fill;
            menuLayout.ColumnCount = 6;
            menuLayout.RowCount = 1;
            menuLayout.BackColor = Color.Transparent;
            menuLayout.ColumnStyles.Clear();

            // Distribución proporcional
            menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Nombre
            for (int i = 1; i < 6; i++)
            {
                menuLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F)); // Opciones
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

            // Submenú para HISTORIAL CLÍNICO
            Panel subMenuHistorial = new Panel();
            subMenuHistorial.Size = new Size(160, 80);
            subMenuHistorial.BackColor = Color.SteelBlue;
            subMenuHistorial.Visible = false;
            subMenuHistorial.BorderStyle = BorderStyle.None;
            formulario.Controls.Add(subMenuHistorial);

            // Subopción: Registrar Historial
            Label lblRegistrar = new Label();
            lblRegistrar.Text = "REGISTRAR";
            lblRegistrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblRegistrar.ForeColor = Color.White;
            lblRegistrar.BackColor = Color.SteelBlue;
            lblRegistrar.Dock = DockStyle.Top;
            lblRegistrar.Height = 40;
            lblRegistrar.TextAlign = ContentAlignment.MiddleCenter;
            lblRegistrar.Cursor = Cursors.Hand;
            lblRegistrar.MouseEnter += (s, e) => lblRegistrar.BackColor = Color.LightSteelBlue;
            lblRegistrar.MouseLeave += (s, e) => lblRegistrar.BackColor = Color.SteelBlue;
            lblRegistrar.Click += (s, e) =>
            {
                if (formulario is FormRegistrarHistorial) return;
                FormRegistrarHistorial registrar = new FormRegistrarHistorial();
                registrar.WindowState = FormWindowState.Maximized;
                registrar.Show();
                formulario.Hide();
            };
            subMenuHistorial.Controls.Add(lblRegistrar);

            // Subopción: Buscar Historial
            Label lblBuscar = new Label();
            lblBuscar.Text = "BUSCAR";
            lblBuscar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblBuscar.ForeColor = Color.White;
            lblBuscar.BackColor = Color.SteelBlue;
            lblBuscar.Dock = DockStyle.Top;
            lblBuscar.Height = 40;
            lblBuscar.TextAlign = ContentAlignment.MiddleCenter;
            lblBuscar.Cursor = Cursors.Hand;
            lblBuscar.MouseEnter += (s, e) => lblBuscar.BackColor = Color.LightSteelBlue;
            lblBuscar.MouseLeave += (s, e) => lblBuscar.BackColor = Color.SteelBlue;
            lblBuscar.Click += (s, e) =>
            {
                if (formulario is FormBuscarHistorial) return;
                FormBuscarHistorial buscar = new FormBuscarHistorial();
                buscar.WindowState = FormWindowState.Maximized;
                buscar.Show();
                formulario.Hide();
            };
            subMenuHistorial.Controls.Add(lblBuscar);

            // Opciones del menú principal
            List<string> secciones = new List<string>
{
    "INICIO",
    "HISTORIAL CLÍNICO",
    "REGISTRO DE PACIENTE"
};

            if (SesionUsuario.TipoUsuario == "A")
            {
                secciones.Add("ADMINISTRACIÓN USUARIO");
            }

            secciones.Add("CERRAR SESIÓN");

            for (int i = 0; i < secciones.Count; i++)
            {
                Label lbl = new Label();
                lbl.Text = secciones[i];
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lbl.ForeColor = Color.White;
                lbl.BackColor = Color.SteelBlue;
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Cursor = Cursors.Hand;
                lbl.Margin = new Padding(0);
                lbl.AutoSize = false;

                // Hover visual
                lbl.MouseEnter += (s, e) => lbl.BackColor = Color.LightSteelBlue;
                lbl.MouseLeave += (s, e) => lbl.BackColor = Color.SteelBlue;

                // Submenú por clic
                if (secciones[i] == "HISTORIAL CLÍNICO")
                {
                    lbl.Click += (s, e) =>
                    {
                        Point posicionGlobal = lbl.PointToScreen(Point.Empty);
                        Point posicionLocal = formulario.PointToClient(new Point(
                            posicionGlobal.X + lbl.Width / 2 - subMenuHistorial.Width / 2,
                            posicionGlobal.Y + lbl.Height
                        ));

                        subMenuHistorial.Location = posicionLocal;
                        subMenuHistorial.Visible = !subMenuHistorial.Visible;
                    };
                }
                else
                {
                    lbl.Click += (s, e) =>
                    {
                        Form destino = null;

                        switch (lbl.Text)
                        {
                            case "INICIO":
                                if (formulario is Inicio) return;
                                destino = new Inicio();
                                break;

                            case "REGISTRO DE PACIENTE":
                                if (formulario is FormRegistroPaciente) return;
                                destino = new FormRegistroPaciente();
                                break;

                            case "ADMINISTRACIÓN USUARIO":
                                if (formulario is FormAdministracionUsuario) return;
                                destino = new FormAdministracionUsuario();
                                break;

                            case "CERRAR SESIÓN":
                                System.Windows.Forms.Application.Exit();
                                return;
                        }

                        if (destino != null)
                        {
                            destino.WindowState = FormWindowState.Maximized;
                            destino.Show();
                            formulario.Hide();
                        }
                    };
                }

                menuLayout.Controls.Add(lbl, i + 1, 0);
            }
        }
    }
}

        
