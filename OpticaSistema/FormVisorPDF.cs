using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using System.IO;
using System.Windows.Forms;

namespace OpticaSistema
{
    public partial class FormVisorPDF : Form
    {
        public FormVisorPDF(byte[] pdfBytes)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            MostrarPDF(pdfBytes);
        }

        private void MostrarPDF(byte[] pdfBytes)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            File.WriteAllBytes(tempPath, pdfBytes);

            WebBrowser visor = new WebBrowser();
            visor.Dock = DockStyle.Fill;
            visor.Navigate(tempPath);

            this.Controls.Add(visor);
        }
    }
}
