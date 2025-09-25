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
    public partial class FormBuscarHistorial : Form
    {
        public FormBuscarHistorial()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
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
    }
}
