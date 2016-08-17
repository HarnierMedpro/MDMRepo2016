using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.DAL
{
    public class _DALDBList
    {
        //Metodo BuscarDB by DB#
        public DataTable BuscarDB(string term)
        {
            DataTable DtResultado = new DataTable("DBList");
            SqlConnection SqlCon = new SqlConnection();
            try
            {
                SqlCon.ConnectionString = Conexion.Cnx1;
                SqlCon.Open(); //Como la cadena de conexion esta cerrada necesitamos abrirla para eso esta linea

                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = SqlCon;
                SqlCmd.CommandText = "hsfindDB";
                SqlCmd.CommandType = CommandType.StoredProcedure;

                SqlParameter ParTextoBuscar = new SqlParameter();
                ParTextoBuscar.ParameterName = "@DB"; //Parametro que se pasa en el procedimiento almacenado
                ParTextoBuscar.SqlDbType = SqlDbType.VarChar;
                ParTextoBuscar.Size = 3;
                ParTextoBuscar.Value = term;
                SqlCmd.Parameters.Add(ParTextoBuscar);

                SqlDataAdapter SqlData = new SqlDataAdapter(SqlCmd); /*Objeto que permite ejecutar el comando y llenar el datatable*/
                SqlData.Fill(DtResultado);
            }
            catch (Exception )
            {
                DtResultado = null;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return DtResultado;
        }
    }
}