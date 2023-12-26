using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyPlc2
{
    internal class PlcVar
    {
        //
        public string? name;
        public string? address;
        public string? type;
        public string? cycle;
        public string comment;
        public bool active;

        public PlcVar(string? name, string? address, string? type, string? cycle, string comment, bool active)
        {
            this.name = name;
            this.address = address;
            this.type = type;
            this.cycle = cycle;
            this.comment = comment;
            this.active = active;
        }
    }
}
