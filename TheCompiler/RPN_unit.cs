using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCompiler
{
    internal class RPN_unit
    {
        public string value;
        public string type;
        public int address;
        public int line;
        public int character;

        public RPN_unit(string value, string type, int address, int line, int character)
        {
            this.value = value;
            this.type = type;
            this.address = address;
            this.line = line;
            this.character = character;
        }
        public override string ToString()
        {
            return value.ToString() + " " + type.ToString();
        }
    }
}
