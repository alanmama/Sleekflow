using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using Sleekflow.Models;

namespace Sleekflow.DAO
{
    public static class TodoDAO
    {
        public static Todo[] getTodo(string id, ref string returnMsg)
        {
            SqlTransaction tranSQL = null;
            DataTable myDatatable = null;
            DataSet resultDataset = new DataSet();
            bool isDBOpened = false;
            SqlConnection conSQL = null;
            SqlCommand cmdSQL = null;
            Hashtable parameterValueTable = new Hashtable();
            List<Todo> listT = null;

            try
            {
                conSQL = new SqlConnection(DataAccessCommon.connectionString);
                conSQL.Open();
                isDBOpened = true;

                parameterValueTable.Add("id", id);
                
                if (!DataAccessCommon.executeSQLStoredProc("usp_get_todo", ref parameterValueTable, ref conSQL, ref cmdSQL, ref tranSQL, ref myDatatable, ref resultDataset, ref returnMsg))
                {
                    resultDataset = null;
                }
                else
                {
                    listT = new List<Todo>();

                    foreach (DataRow dr in resultDataset.Tables[0].Rows)
                    {
                        Todo t = new Todo();

                        t.Id = dr["id"].ToString();
                        t.Name = dr["name"].ToString();
                        t.Description = dr["description"].ToString();
                        t.DueDate = dr["duedate"].ToString();
                        t.Status = dr["status"].ToString();
                        listT.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                resultDataset = null;
                returnMsg = ex.Message.ToString();
            }
            finally
            {
                cmdSQL = null;
                parameterValueTable = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                }

                conSQL = null;
            }

            if (listT != null)
            {
                return listT.ToArray();
            }
            else
            {
                return null;
            }
        }
        
        public static string insertTodo(ref Todo t, ref string returnMsg)
        {
            SqlTransaction tranSQL = null;
            DataTable myDatatable = null;
            DataSet myDataset = null;
            string returnValue = string.Empty;
            bool isDBOpened = false;
            SqlConnection conSQL = null;
            SqlCommand cmdSQL = null;
            Hashtable parameterValueTable = new Hashtable();

            try
            {
                conSQL = new SqlConnection(DataAccessCommon.connectionString);
                conSQL.Open();
                isDBOpened = true;

                parameterValueTable.Add("name", t.Name.Trim());
                parameterValueTable.Add("description", t.Description.Trim());
                parameterValueTable.Add("status", t.Status.Trim());
                parameterValueTable.Add("duedate", t.DueDate);

                if (DataAccessCommon.executeSQLStoredProc("usp_insert_todo", ref parameterValueTable, ref conSQL, ref cmdSQL, ref tranSQL, ref myDatatable, ref myDataset, ref returnMsg))
                {
                    returnValue = cmdSQL.Parameters["@id"].SqlValue.ToString();
                }
            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
            }
            finally
            {
                cmdSQL = null;
                parameterValueTable = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                }

                conSQL = null;
            }

            return returnValue;
        }

        public static bool deleteTodo(string id, ref string returnMsg)
        {
            SqlTransaction tranSQL = null;
            DataTable myDatatable = null;
            DataSet myDataset = null;
            bool returnValue = false;
            bool isDBOpened = false;
            SqlConnection conSQL = null;
            SqlCommand cmdSQL = null;
            Hashtable parameterValueTable = new Hashtable();

            try
            {
                conSQL = new SqlConnection(DataAccessCommon.connectionString);
                conSQL.Open();
                isDBOpened = true;

                parameterValueTable.Add("id", id);

                if (DataAccessCommon.executeSQLStoredProc("usp_delete_todo", ref parameterValueTable, ref conSQL, ref cmdSQL, ref tranSQL, ref myDatatable, ref myDataset, ref returnMsg))
                {
                    returnValue = true;
                }                
            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
            }
            finally
            {
                cmdSQL = null;
                parameterValueTable = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                }

                conSQL = null;
            }

            return returnValue;
        }

        public static bool updateTodo(ref Todo t, ref string returnMsg)
        {
            SqlTransaction tranSQL = null;
            DataTable myDatatable = null;
            DataSet myDataset = null;
            bool returnValue = false;
            bool isDBOpened = false;
            SqlConnection conSQL = null;
            SqlCommand cmdSQL = null;
            Hashtable parameterValueTable = new Hashtable();

            try
            {
                conSQL = new SqlConnection(DataAccessCommon.connectionString);
                conSQL.Open();
                isDBOpened = true;

                parameterValueTable.Add("id", t.Id);
                parameterValueTable.Add("name", t.Name.Trim());
                parameterValueTable.Add("description", t.Description.Trim());
                parameterValueTable.Add("duedate", t.DueDate.Trim());
                parameterValueTable.Add("status", t.Status.Trim());

                if (DataAccessCommon.executeSQLStoredProc("usp_update_todo", ref parameterValueTable, ref conSQL, ref cmdSQL, ref tranSQL, ref myDatatable, ref myDataset, ref returnMsg))
                {
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
            }
            finally
            {
                cmdSQL = null;
                parameterValueTable = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                }

                conSQL = null;
            }

            return returnValue;
        }

        public static Todo[] filterTodo(string filter_col, string filter_val, string is_date, string sort_by, ref string returnMsg)
        {
            SqlTransaction tranSQL = null;
            DataTable myDatatable = null;
            DataSet resultDataset = new DataSet();
            bool isDBOpened = false;
            SqlConnection conSQL = null;
            SqlCommand cmdSQL = null;
            Hashtable parameterValueTable = new Hashtable();
            List<Todo> listT = null;

            try
            {
                conSQL = new SqlConnection(DataAccessCommon.connectionString);
                conSQL.Open();
                isDBOpened = true;

                parameterValueTable.Add("filter_col", filter_col);
                parameterValueTable.Add("filter_val", filter_val);
                parameterValueTable.Add("is_date", is_date);
                parameterValueTable.Add("sort_by", sort_by);

                if (!DataAccessCommon.executeSQLStoredProc("usp_filter_todo", ref parameterValueTable, ref conSQL, ref cmdSQL, ref tranSQL, ref myDatatable, ref resultDataset, ref returnMsg))
                {
                    resultDataset = null;
                }
                else
                {
                    listT = new List<Todo>();

                    foreach (DataRow dr in resultDataset.Tables[0].Rows)
                    {
                        Todo t = new Todo();

                        t.Id = dr["id"].ToString();
                        t.Name = dr["name"].ToString();
                        t.Description = dr["description"].ToString();
                        t.DueDate = dr["duedate"].ToString();
                        t.Status = dr["status"].ToString();
                        listT.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                resultDataset = null;
                returnMsg = ex.Message.ToString();
            }
            finally
            {
                cmdSQL = null;
                parameterValueTable = null;

                if (isDBOpened)
                {
                    conSQL.Close();
                }

                conSQL = null;
            }

            if (listT != null)
            {
                return listT.ToArray();
            }
            else
            {
                return null;
            }

        }
    }
}
