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
    public partial class FormRegistrarHistorial : Form
    {
        public FormRegistrarHistorial()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
        }

        private void FormRegistrarHistorial_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - RegistrarHistorial";
            this.Icon = new Icon("Imagenes/log.ico");
        }
        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
