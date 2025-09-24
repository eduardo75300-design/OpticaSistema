using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpticaSistema
{
    public partial class Bienvenido : Form
    {
        public Bienvenido()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Size = new Size(500, 150);
            this.TopMost = true;

            // Sombra o borde opcional
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Fondo decorativo
            //this.BackgroundImage = Image.FromFile("imagenes/fondo_bienvenida.png");
            //this.BackgroundImageLayout = ImageLayout.Stretch;

            // Icono o imagen decorativa
            PictureBox icono = new PictureBox();
            icono.Image = Image.FromFile("Imagenes/log.png");
            icono.Size = new Size(64, 64);
            icono.Location = new Point(20, 20);
            icono.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(icono);

            // Texto de bienvenida
            Label lbl = new Label();
            lbl.Text = "¡Bienvenido al sistema!";
            lbl.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lbl.ForeColor = Color.DarkRed;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point(100, 30);
            this.Controls.Add(lbl);

            // Subtítulo opcional
            Label sub = new Label();
            sub.Text = "Cargando entorno de trabajo...";
            sub.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            sub.ForeColor = Color.Gray;
            sub.AutoSize = true;
            sub.Location = new Point(100, 70);
            this.Controls.Add(sub);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await Task.Delay(400);
            this.Close();
        }

        private void Bienvenido_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema";
            this.Icon = new Icon("Imagenes/log.ico");

        }
    }
}
