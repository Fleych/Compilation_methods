using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheCompiler
{
    internal class Storage
    {
        public List<Lexem> lexems = new List<Lexem>();
        public List<RPN_unit> RPN = new List<RPN_unit>();
        public Dictionary<string, int[]> memory = new Dictionary<string, int[]>();

        public string error = "";
    }
}
