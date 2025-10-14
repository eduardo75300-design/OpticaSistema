using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpticaSistema.FormLogin;

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
            subMenuHistorial.BackColor = Color.SteelBlue;
            subMenuHistorial.Visible = false;
            subMenuHistorial.BorderStyle = BorderStyle.None;
            formulario.Controls.Add(subMenuHistorial);

            // Lista de opciones permitidas según tipo de usuario
            List<(string texto, Action accion)> opcionesHistorial = new List<(string, Action)>();

            // Solo permitir "REGISTRAR" a usuarios tipo S o A
            if (SesionUsuario.TipoUsuario == "S" || SesionUsuario.TipoUsuario == "A")
            {
                opcionesHistorial.Add(("REGISTRAR", () => {
                    if (formulario is FormRegistrarHistorial) return;
                    FormRegistrarHistorial registrar = new FormRegistrarHistorial();
                    registrar.WindowState = FormWindowState.Maximized;
                    registrar.Show();
                    formulario.Hide();
                }
                ));
            }

            // "BUSCAR" disponible para todos
            opcionesHistorial.Add(("REGISTRAR-EDITAR", () =>
            {
                if (formulario is FormBuscarHistorial) return;
                FormBuscarHistorial buscar = new FormBuscarHistorial();
                buscar.WindowState = FormWindowState.Maximized;
                buscar.Show();
                formulario.Hide();
            }
            ));

            // "GENERAR" solo para S y A (ya estaba bien)
            if (SesionUsuario.TipoUsuario == "S" || SesionUsuario.TipoUsuario == "A")
            {
                opcionesHistorial.Add(("GENERAR", () =>
                {
                    if (formulario is FormGenerarHistorial) return;
                    FormGenerarHistorial generar = new FormGenerarHistorial();
                    generar.WindowState = FormWindowState.Maximized;
                    generar.Show();
                    formulario.Hide();
                }
                ));
            }


            // Crear dinámicamente los labels según opciones permitidas
            foreach (var (texto, accion) in opcionesHistorial)
            {
                Label lblOpcion = new Label();
                lblOpcion.Text = texto;
                lblOpcion.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblOpcion.ForeColor = Color.White;
                lblOpcion.BackColor = Color.SteelBlue;
                lblOpcion.Dock = DockStyle.Top;
                lblOpcion.Height = 40;
                lblOpcion.TextAlign = ContentAlignment.MiddleCenter;
                lblOpcion.Cursor = Cursors.Hand;
                lblOpcion.MouseEnter += (s, e) => lblOpcion.BackColor = Color.LightSteelBlue;
                lblOpcion.MouseLeave += (s, e) => lblOpcion.BackColor = Color.SteelBlue;
                lblOpcion.Click += (s, e) => accion();

                subMenuHistorial.Controls.Add(lblOpcion);
            }

            // Ajustar altura del submenú según cantidad de opciones
            subMenuHistorial.Height = subMenuHistorial.Controls.Count * 40;


            // Opciones del menú principal
            List<string> secciones = new List<string>
{
    "INICIO",
    "HISTORIAL CLÍNICO"
};
            if (SesionUsuario.TipoUsuario == "S" || SesionUsuario.TipoUsuario == "A")
            {
                secciones.Add("REGISTRO DE PACIENTE");
            }
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
                            posicionGlobal.X,
                            posicionGlobal.Y + lbl.Height
                        ));

                        subMenuHistorial.Width = lbl.Width; // adaptar al ancho del botón
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
                                if (formulario is FormInicio) return;
                                destino = new FormInicio();
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
                                formulario.Close();
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

        
