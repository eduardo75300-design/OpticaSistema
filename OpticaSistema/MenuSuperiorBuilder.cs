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

            List<string> secciones = new List<string>
            {
                "INICIO",
                "HISTORIAL CLÍNICO",
                "REGISTRO DE PACIENTE"
            };

            // Solo agregar si es administrador
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
                            if (formulario is Inicio) return;
                            destino = new Inicio();
                            break;

                        /*case "HISTORIAL CLÍNICO":
                            if (formulario  is FormHistorialClinico) return;
                            destino = new FormHistorialClinico();
                            break;

                        case "REGISTRO DE PACIENTE":
                            if (formulario  is FormRegistroPaciente) return;
                            destino = new FormRegistroPaciente();
                            break;*/

                        case "ADMINISTRACIÓN USUARIO":
                            if (formulario is FormAdministracionUsuario) return;
                            destino = new FormAdministracionUsuario();
                            break;

                        case "CERRAR SESIÓN":
                            Application.Exit();
                            return;
                    }

                    if (destino != null)
                    {
                        destino.WindowState = FormWindowState.Maximized;
                        destino.Show();
                        formulario.Hide();
                    }
                };

                menuLayout.Controls.Add(lbl, i + 1, 0); // +1 porque la columna 0 es para el nombre
            }
        }
    }
}

        
