using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Models
{
    public class User
    {
        //Name of the profile
        public string Name { get; set; }
        public string Password { get; set; }
        public string Program { get; set; }

        public User(string name, string password, string program)
        {
            Name = name;
            Password = password;
            Program = program;
        }

        public void Save()
        {
            string path = string.Format(@"Data\Users\{0}", Name);

            // check folder exists
            if (Directory.Exists(path))
            {
                // ok, the directory exists
            }
            // if not then create
            else
            {
                Directory.CreateDirectory(path);
            }

        }
    }
}
    

