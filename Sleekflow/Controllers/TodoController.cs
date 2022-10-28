using Sleekflow.Models;
using Sleekflow.Services;
using System;
using System.Web.Http;
using Newtonsoft.Json;

namespace Sleekflow.Controllers
{
    public class TodoController : ApiController
    {
        private TodoRepository repoTodo;        

        public TodoController()
        {
            this.repoTodo = new TodoRepository();
        }
        
        [HttpGet]
        public Todo[] Get(string id)
        {
            string returnMsg = string.Empty;
            return this.repoTodo.GetTodo(id, ref returnMsg);            
        }

        public string Insert(Todo t)
        {
            string returnMsg = string.Empty;
            return this.repoTodo.InsertTodo(ref t, ref returnMsg);
        }

        public bool Update(Todo t)
        {
            string returnMsg = string.Empty;            
            return this.repoTodo.UpdateTodo(ref t, ref returnMsg);
        }

        public bool Delete(string id)
        {
            string returnMsg = string.Empty;
            return this.repoTodo.DeleteTodo(id, ref returnMsg);
        }

        public Todo[] Filter(string filtercol, string filterval, string isdate, string sortby)
        {
            string returnMsg = string.Empty;
            return this.repoTodo.FilterTodo(filtercol, filterval, isdate, sortby, ref returnMsg);
        }
    }
}
