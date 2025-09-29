using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpticaSistema.FormLogin;

namespace OpticaSistema
{
    public partial class FormInicio : Form
    {
        private Label lblTitulo;
        private Label lblSubtitulo;
        private PictureBox imagenPromocional;

        public FormInicio()
        {
            InitializeComponent();
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
            InicializarContenidoPromocional();
            this.FormClosing += Inicio_FormClosing;
            this.Resize += AjustarFuenteDinamicamente;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
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

        private void InicializarContenidoPromocional()
        {
            Panel panelPromocional = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(panelPromocional);

            int anchoMaximo = 600;

            // Crear título
            lblTitulo = new Label
            {
                Text = "PREOCUPADOS POR MEJORAR TU VISIÓN",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,                      // Deja que se acomode solo
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 250, 0, 10)     // Espacio abajo
            };

            // --- Subtítulo ---
            lblSubtitulo = new Label
            {
                Text = " Más de 25 años al Cuidado de tu Visión",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,                      // Igual, que crezca en alto
                TextAlign = ContentAlignment.TopLeft,
                MaximumSize = new Size(600, 0),       // Para que haga salto de línea
                Margin = new Padding(0, 20, 0, 10)     // Espacio arriba
            };

            // Imagen
            string rutaImagen = "Imagenes/imagen-prueba.png";
            imagenPromocional = new PictureBox
            {
                Dock = DockStyle.Fill,                  // Ocupa todo el espacio disponible
                SizeMode = PictureBoxSizeMode.Zoom,     // Se adapta manteniendo proporción
                Margin = new Padding(0, 100, 0, 10),
                BackColor = Color.Transparent
            };

            if (File.Exists(rutaImagen))
            {
                Image original = Image.FromFile(rutaImagen);
                imagenPromocional.Image = HacerCircular(original);

                // Hacerla redonda
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, imagenPromocional.Width, imagenPromocional.Height);
                imagenPromocional.Region = new Region(path);

                // Redondear también cuando cambie de tamaño
                imagenPromocional.Resize += (s, e) =>
                {
                    int lado = Math.Min(imagenPromocional.Width, imagenPromocional.Height);
                    imagenPromocional.Width = lado;
                    imagenPromocional.Height = lado;
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, lado, lado);
                    imagenPromocional.Region = new Region(path);
                };
            }
            else
            {
                MessageBox.Show("No se encontró la imagen en la ruta: " + rutaImagen);
            }

            // Contenedor de texto
            TableLayoutPanel contenedorTexto = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(40, 0, 0, 40)
            };
            contenedorTexto.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenedorTexto.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contenedorTexto.Controls.Add(lblTitulo, 0, 0);
            contenedorTexto.Controls.Add(lblSubtitulo, 0, 1);

            // Layout principal
            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.Controls.Add(contenedorTexto, 0, 0);
            layout.Controls.Add(imagenPromocional, 1, 0);

            panelPromocional.Controls.Add(layout);
        }


        private void AjustarFuenteDinamicamente(object sender, EventArgs e)
        {
            if (lblTitulo == null || lblSubtitulo == null) return;

            // Calcular ancho disponible en el lado izquierdo (texto)
            int anchoDisponible = this.ClientSize.Width / 2 - 80;

            // Ajustar límites dinámicos
            lblTitulo.MaximumSize = new Size(anchoDisponible, 0);
            lblSubtitulo.MaximumSize = new Size(anchoDisponible, 0);

            // Escalar fuentes
            float tamañoTitulo = Math.Max(20, anchoDisponible / 20f);
            float tamañoSubtitulo = Math.Max(12, anchoDisponible / 40f);

            lblTitulo.Font = new Font("Segoe UI", tamañoTitulo, FontStyle.Bold);
            lblSubtitulo.Font = new Font("Segoe UI", tamañoSubtitulo, FontStyle.Regular);

            // Forzar recalculo del layout
            lblTitulo.Parent?.PerformLayout();
        }

        private Image HacerCircular(Image img)
        {
            int lado = Math.Min(img.Width, img.Height);
            Bitmap cuadrada = new Bitmap(lado, lado);

            using (Graphics g = Graphics.FromImage(cuadrada))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Centrar imagen dentro del cuadrado
                int x = (lado - img.Width) / 2;
                int y = (lado - img.Height) / 2;
                g.DrawImage(img, x, y, img.Width, img.Height);
            }

            // Recortar en círculo
            Bitmap circular = new Bitmap(lado, lado);
            using (Graphics g = Graphics.FromImage(circular))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, lado, lado);
                    g.SetClip(path);
                    g.DrawImage(cuadrada, 0, 0);
                }
            }

            return circular;
        }



    }
}
