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
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);

            // Establece el formulario en pantalla completa
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


    }
}
