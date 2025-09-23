using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace OpticaSistema
{
    public class ConexionDB
    {
        private string cadena = "Server=localhost;Database=Optica;User Id=sad;Password=12345;";
        
        public SqlConnection Conectar()
        {
            SqlConnection con = new SqlConnection(cadena);
            /*try
            ///{
                con.Open();
                System.Windows.Forms.MessageBox.Show("Conexión exitosa");
            }
            catch(SqlException ex) {
                System.Windows.Forms.MessageBox.Show("Error al conectar: "+ ex.Message);    
            }*/
            return con;
        }

    }
}
