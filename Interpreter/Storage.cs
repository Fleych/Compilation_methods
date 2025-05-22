using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class Storage
    {
        public List<Lexem> lexems = new List<Lexem>();
        public Queue<int> constants = new Queue<int>();

        public List<string> RPN = new List<string>();
        public List<string> RPN_types = new List<string>();
        public Dictionary<string, int[]> vars = new Dictionary<string, int[]>();
    }
}
