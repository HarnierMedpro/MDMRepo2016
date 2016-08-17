using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.DAL
{
    public class _DALvmPosTab
    {
        //Metodo para Leer la vista vmPosTab
        public DataTable Read()
        {
            DataTable dtResultado = new DataTable("vmPosTab");
            SqlConnection sqlCon = new SqlConnection();
            try
            {
                sqlCon.ConnectionString = Conexion.Cnx1;
                sqlCon.Open(); //Como la cadena de conexion esta cerrada necesitamos abrirla para eso esta linea

                SqlCommand sqlCmd = new SqlCommand
                {
                    Connection = sqlCon,
                    CommandText = "pa_read_view",
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataAdapter sqlData;
                using (sqlData = new SqlDataAdapter(sqlCmd))
                {
                    sqlData.Fill(dtResultado);
                }
            }
            catch (Exception )
            {
                dtResultado = null;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            return dtResultado;
        }
    }
}