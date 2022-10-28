using System;

namespace Sleekflow.Models
{
    public class Todo
    {
        /*public Todo()
        {
            Id = Guid.NewGuid();
        }*/

        //public Guid Id { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
    }
}
