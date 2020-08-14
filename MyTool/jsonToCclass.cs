using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool
{
    class jsonToCclass
    {
        public class Employees
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
        }
        public class Manager
        {
            public string salary { get; set; }
            public string age { get; set; }
        }    
        public class RootObject
        {
            public string companyID { get; set; }
            public List<Employees> employees { get; set; }
            public List<Manager> manager { get; set; }
        }
    }
}
