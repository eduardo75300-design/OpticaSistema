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
    public partial class FormRegistroPaciente : Form
    {
        public FormRegistroPaciente()
        {
            InitializeComponent();
            this.FormClosing += FormAdministracionUsuario_FormClosing;
            MenuSuperiorBuilder.CrearMenuSuperiorAdaptable(this);
        }

        private void FormRegistroPaciente_Load(object sender, EventArgs e)
        {

            this.Text = "OpticaSistema - RegistrarPaciente";
            this.Icon = new Icon("Imagenes/log.ico");
        }

        private void FormAdministracionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
