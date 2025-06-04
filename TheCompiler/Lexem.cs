using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCompiler
{
    internal class Lexem
    {
        public int code;
        public string str;
        public int line;
        public int character;

        public Lexem(int code, string str, int line, int character) 
        {
            this.code = code;
            this.str = str;
            this.line = line;
            this.character = character; 
        }
        public override string ToString()
        {
            return code.ToString() + " " + str + " " + line.ToString() + ":" + character.ToString();
        }
    }
}
