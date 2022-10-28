using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;


namespace Sleekflow.DAO
{
    public static class DataAccessCommon
    {
               
        public static string connectionString 
        {
            get
            {
                return Properties.Settings.Default.ConnectionString;
            }
        }
        
        public static bool executeSQLStoredProc(string storedProcName, 
                                                ref Hashtable valueDataRow, 
                                                ref SqlConnection conSQL , 
                                                ref SqlCommand cmdSQL, 
                                                ref SqlTransaction trnSQL, 
                                                ref DataTable parameterTable,
                                                ref DataSet returnValueDataSet,
                                                ref String returnMsg)
        {
            Boolean returnValue= true;            
            Boolean isDBOpened = false;
            SqlDataAdapter adptSQL;
            String errorMsg = "";
            
            try
            {
                //if connection object is not passed, create a new connection
                if (conSQL == null)
                {
                    conSQL = new SqlConnection(Properties.Settings.Default.ConnectionString);
                    conSQL.Open();
                    isDBOpened = true;
                }

                //if stored proc parameters table is not passed, get parameters table
                if (parameterTable == null)
                {
                    parameterTable = getSQLStoredProcParametersList(storedProcName, ref conSQL, ref trnSQL, ref errorMsg);

                    if (errorMsg.Trim() != "")
                    {
                        returnMsg = errorMsg;
                        returnValue = false;
                    }
                }
                
                if (returnValue)
                {
                    //create command object for the stored proc
                    cmdSQL = new SqlCommand("SET ARITHABORT ON", conSQL);

                    if (trnSQL != null)
                    {
                        cmdSQL.Transaction = trnSQL;
                    }

                    cmdSQL.ExecuteNonQuery();
                    
                    cmdSQL = new SqlCommand(storedProcName, conSQL);                    
                    cmdSQL.CommandType = CommandType.StoredProcedure;
                    cmdSQL.CommandTimeout = 10000;

                    if (trnSQL != null)
                    {
                        cmdSQL.Transaction = trnSQL;
                    }

                        //create command parametes and fill with value
                    if (createSQLCommandParametersWithValue(ref cmdSQL, ref parameterTable, ref valueDataRow, ref returnMsg))
                    {
                        //create dataset object for saving return value
                        if (returnValueDataSet == null)
                        {
                            returnValueDataSet = new DataSet();
                        }

                        //execute stored proc
                        adptSQL = new SqlDataAdapter();
                        adptSQL.SelectCommand = cmdSQL;
                        adptSQL.Fill(returnValueDataSet);
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
                returnMsg = ex.Message;
            }
            finally
            {
                adptSQL = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                    conSQL = null;
                }
            }

            return returnValue;
        }

        public static DataTable getSQLStoredProcParametersList(string storedProcName,
                                                                ref SqlConnection conSQL,
                                                                ref SqlTransaction trnSQL,
                                                                ref string returnMsg)

        {
            DataTable returnValue = new DataTable();
            Boolean isDBOpened = false;
            SqlCommand cmdSQL;
            SqlDataAdapter adptSQL;

            try
            {
                //if connection object is not passed, create a new connection
                if (conSQL == null)
                {
                    conSQL = new SqlConnection(Properties.Settings.Default.ConnectionString);
                    conSQL.Open();
                    isDBOpened = true;
                }

                cmdSQL = new SqlCommand("usp_get_stored_proc_parameter_list", conSQL);                
                cmdSQL.CommandTimeout = 10000;
                cmdSQL.CommandType = CommandType.StoredProcedure;

                if (trnSQL != null)
                {
                    cmdSQL.Transaction = trnSQL;
                }

                cmdSQL.Parameters.AddWithValue("@sp_name", storedProcName.Trim());
                
                adptSQL = new SqlDataAdapter();
                adptSQL.SelectCommand = cmdSQL;
                adptSQL.Fill(returnValue);
            }
            catch (Exception ex)
            {
                returnValue = null;
                returnMsg = ex.Message;
            }
            finally
            {
                adptSQL = null;
                cmdSQL = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                    conSQL = null;
                }
            }

            return returnValue;
        }

        public static bool createSQLCommandParametersWithValue(ref SqlCommand mySQLCommand, 
                                                                ref DataTable parameterTable, 
                                                                ref Hashtable valueDataRow,
                                                                ref string returnMsg)
        {
            bool returnValue  = true;            
            
            try
            {                
                    foreach (DataRow myDataRow in parameterTable.Rows)
                    {
                        if (myDataRow["isoutparam"].ToString().Trim() == "0")
                        {
                            //input parameter
                            if (!valueDataRow.ContainsKey(myDataRow["name"].ToString().Trim().Substring(1)))
                            {
                                //if no value is found, set default value for parameter
                                switch ((SqlDbType)(SqlDbType.Parse(typeof(SqlDbType), myDataRow["typename"].ToString().Trim(), true)))
                                {
                                    case SqlDbType.BigInt:
                                    case SqlDbType.Bit:
                                    case SqlDbType.Decimal:
                                    case SqlDbType.Float:
                                    case SqlDbType.Int:
                                    case SqlDbType.Money:
                                    case SqlDbType.Real:
                                    case SqlDbType.TinyInt:
                                    case SqlDbType.SmallInt:
                                    case SqlDbType.SmallMoney:
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), 0);
                                        break;

                                    case SqlDbType.Char:
                                    case SqlDbType.NChar:
                                    case SqlDbType.NText:
                                    case SqlDbType.NVarChar:
                                    case SqlDbType.Text:
                                    case SqlDbType.VarChar:
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), "");
                                        break;

                                    case SqlDbType.DateTime: 
                                    case SqlDbType.SmallDateTime:
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), DateTime.Parse("1900/01/01"));
                                        break;
                                        
                                    default:
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), "");
                                        break;
                                }                                
                            }
                            else
                            {
                                switch ((SqlDbType)(SqlDbType.Parse(typeof(SqlDbType), myDataRow["typename"].ToString().Trim(), true)))
                                {
                                    case SqlDbType.Char:
                                    case SqlDbType.NChar:
                                    case SqlDbType.NText:
                                    case SqlDbType.NVarChar:
                                    case SqlDbType.Text:
                                    case SqlDbType.VarChar:
                                        //for char type, passs empty string instead of null value
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), valueDataRow[myDataRow["name"].ToString().Trim().Substring(1)].ToString().Trim());
                                        break;

                                    default:
                                        mySQLCommand.Parameters.AddWithValue(myDataRow["name"].ToString().Trim(), valueDataRow[myDataRow["name"].ToString().Trim().Substring(1)]);
                                        break;
                                }                                
                            }
                        }
                        else
                        {
                            //output paremeter
                            SqlParameter sparaOut = new SqlParameter(myDataRow["name"].ToString().Trim(), (SqlDbType)(SqlDbType.Parse(typeof(SqlDbType), myDataRow["typename"].ToString().Trim(), true)), Int32.Parse(myDataRow["prec"].ToString().Trim()));

                            if ((SqlDbType)(SqlDbType.Parse(typeof(SqlDbType), myDataRow["typename"].ToString().Trim(), true)) == SqlDbType.Decimal)
                            {
                                sparaOut.Precision = (byte)(Int32.Parse(myDataRow["xprec"].ToString().Trim()));
                                sparaOut.Scale = (byte)(Int32.Parse(myDataRow["xscale"].ToString().Trim()));
                            }

                            sparaOut.Direction = ParameterDirection.Output;
                            mySQLCommand.Parameters.Add(sparaOut);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                returnValue = false;
                returnMsg = ex.Message;
            }
            finally
            {
                //myDataRow = null;
            }

            return returnValue;
        }

        public static DataTable ExeSQLGetResult(string sngSQL, ref string sngMsg)
        {
            SqlDataAdapter sdaData = new SqlDataAdapter();
            DataTable dtblReturn = new DataTable();
            SqlCommand sqlcData;            

            // Declare SQLCommand            
            sqlcData = new SqlCommand(sngSQL, new SqlConnection(Properties.Settings.Default.ConnectionString));
            
            // Getting Data from Server            
            try
            {
                sdaData.SelectCommand = sqlcData;
                sdaData.Fill(dtblReturn);
            }
            catch (Exception ex)
            {
                sngMsg += ex.Message;
            }
            finally
            {
                if (sdaData.SelectCommand != null)
                {
                    if (sdaData.SelectCommand.Connection != null)
                    {
                        sdaData.SelectCommand.Connection.Dispose();
                    }
                    sdaData.SelectCommand.Dispose();
                }
                sdaData.Dispose();
            }

            return dtblReturn;            
        }


    }    
}
