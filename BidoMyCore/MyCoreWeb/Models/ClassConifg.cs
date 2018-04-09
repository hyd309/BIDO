using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace MyCoreWeb.Models
{
    public class ClassConifg
    {
        public string Name { get; set; }
        public string Info { get; set; }

        public List<Student> Students { get; set; }
    }

    public class Student
    {
        public string name { get; set; }
        public string age { get; set; }
    }
}
