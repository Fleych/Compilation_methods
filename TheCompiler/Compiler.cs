using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCompiler
{
    internal class Compiler
    {
        Storage storage;
        Lexer lexer;
        Parser parser;
        Interpreter interpreter;

        public Compiler()
        {
            storage = new Storage();
            lexer = new Lexer(storage);
            parser = new Parser(storage);
            interpreter = new Interpreter(storage);
        }

        public void Compile(string program)
        {
            storage.lexems.Clear();
            storage.RPN.Clear();
            storage.memory.Clear();

            lexer.Analyze(program);

            /*
            Console.WriteLine("Лексический анализатор\n");
            Console.WriteLine("Номер " + "Лексема " + "Строка:символ");
            for (int i = 0; i < storage.lexems.Count; i++)
            {
                Console.WriteLine(storage.lexems[i].ToString());
            }
            */

            parser.Analyze();

            /*
            Console.WriteLine("\nСинтаксический анализатор\n");
            Console.WriteLine("Номер: " + "ОПС: " + "     ТИП:");
            for (int i = 0; i < storage.RPN.Count; i++)
            {
                Console.WriteLine(i.ToString() + " " + storage.RPN[i].ToString());
            }
            */

            interpreter.Interpret();

            if (storage.error != "")
            {
                Console.WriteLine(storage.error);
                return;
            }
        }
    }
}
