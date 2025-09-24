using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpticaSistema.Login;

namespace OpticaSistema
{
    public partial class FormAdministracionUsuario : Form
    {
        public FormAdministracionUsuario()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
        }

        private void FormAdministracionUsuario_Load(object sender, EventArgs e)
        {
            this.Text = "OpticaSistema - Usuario";
            this.Icon = new Icon("Imagenes/log.ico");

        }
        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }



     
}
