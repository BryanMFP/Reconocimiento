using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Reconocimiento
{
    class clsDA {

        private static OleDbConnection cnx = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = DBrostros.accdb;");

        public static string[] Nombre;
        private static byte[] Rostro;
        public static List<byte[]> ListadosRostros = new List<byte[]>();
        public static int TotalRostros;
        public static bool GuardarImagen(string Nombre, byte[] Imagen)
        {
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("INSERT INTO Rostros(Nombre, Imagen) VALUES ('"+Nombre+"',?)");
            OleDbParameter parImagen = new OleDbParameter("@Imagen",OleDbType.VarBinary,Imagen.Length);
            parImagen.Value = Imagen;
            cmd.Parameters.Add(Imagen);
            int Resultado=cmd.ExecuteNonQuery();
            cnx.Close();
            return Convert.ToBoolean(Resultado);
        }

        public static DataTable Consultar(DataGridView DATA)
        {
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Rostros;", cnx);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DATA.DataSource = dt;
            int cont = dt.Rows.Count;
            Nombre = new string[cont];

            for (int i= 0; i<cont; i++)
            {
                Nombre[i] = dt.Rows[i]["Nombre"].ToString();
                Rostro = (byte[])dt.Rows[i]["Imagen"];
                ListadosRostros.Add(Rostro);
            }

            try
            {
                DATA.Columns[0].Width = 60;
                DATA.Columns[1].Width = 160;
                DATA.Columns[2].Width = 160;
                for (int i = 0; i < cont; i++)
                {
                    DATA.Rows[i].Height = 110;
                }
            }
            catch 
            {
            }
            TotalRostros = cont;
            cnx.Close();
            return dt;
            
        }

        public static byte[] ConvertImgToBinary(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            MemoryStream memoria = new MemoryStream();
            bmp.Save(memoria, ImageFormat.Bmp);
            byte[] imagen = memoria.ToArray();
            return imagen;
        }

        public static Image ConvertBinaryToImg(int C)
        {
            Image Imagen;
            byte[] ima = ListadosRostros[C];
            MemoryStream memoria = new MemoryStream(ima);
            Imagen = Image.FromStream(memoria);
            memoria.Close();
            return Imagen;
        }
    }
}
