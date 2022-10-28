using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sleekflow.Models;
using Sleekflow.DAO;

namespace Sleekflow.Services
{
    public class TodoRepository
    {
        public Todo[] GetTodo(string id, ref string returnMsg)
        {
            return TodoDAO.getTodo(id, ref returnMsg);
        }

        public string InsertTodo(ref Todo t, ref string returnMsg)
        {
            string returnValue = string.Empty;

            if (t == null)
            {
                returnMsg += "Invalid Todo item.";
                return returnValue;
            }

            return TodoDAO.insertTodo(ref t, ref returnMsg);
        }
        
        public bool DeleteTodo(string id, ref string returnMsg)
        {
            return TodoDAO.deleteTodo(id, ref returnMsg);
        }

        public bool UpdateTodo(ref Todo t, ref string returnMsg)
        {
            return TodoDAO.updateTodo(ref t, ref returnMsg);
        }

        public Todo[] FilterTodo(string filter_col, string filter_val, string is_date, string sort_by, ref string returnMsg)
        {
            return TodoDAO.filterTodo(filter_col, filter_val, is_date, sort_by, ref returnMsg);
        }
    }
}