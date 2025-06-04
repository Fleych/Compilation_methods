using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TheCompiler
{
    internal class Interpreter
    {
        private Storage storage;
        private Stack<RPN_unit> magazine = new Stack<RPN_unit>();
        private int i;

        public Interpreter(Storage storage)
        {
            this.storage = storage;
        }
        private void Assign()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            if (oper_2.type != "name")
            {
                storage.error = "Ошибка в строке: " + (oper_2.line + 1) + " символ: " + (oper_2.character + 1) + " недопустимый тип операнда для данной операции"; 
                return;
            }
            else
            {
                storage.memory[oper_2.value][oper_2.address] = storage.memory[oper_1.value][oper_1.address];
            }
        }
        private void Add()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            int res_num = storage.memory[oper_2.value][oper_2.address] + storage.memory[oper_1.value][oper_1.address];

            if (!storage.memory.ContainsKey(res_num.ToString()))
            {
                storage.memory.Add(res_num.ToString(), new int[1] {res_num});
            }
            
            RPN_unit res = new RPN_unit(res_num.ToString(), "constant", 0, oper_2.line, oper_2.character);
            magazine.Push(res);
        }
        private void Sub()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            int res_num = storage.memory[oper_2.value][oper_2.address] - storage.memory[oper_1.value][oper_1.address];

            if (!storage.memory.ContainsKey(res_num.ToString()))
            {
                storage.memory.Add(res_num.ToString(), new int[1] { res_num });
            }

            RPN_unit res = new RPN_unit(res_num.ToString(), "constant", 0, oper_2.line, oper_2.character);
            magazine.Push(res);
        }
        private void Mult()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            int res_num = storage.memory[oper_2.value][oper_2.address] * storage.memory[oper_1.value][oper_1.address];

            if (!storage.memory.ContainsKey(res_num.ToString()))
            {
                storage.memory.Add(res_num.ToString(), new int[1] { res_num });
            }

            RPN_unit res = new RPN_unit(res_num.ToString(), "constant", 0, oper_2.line, oper_2.character);
            magazine.Push(res);
        }
        private void Div()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            if (storage.memory[oper_1.value][oper_1.address] == 0)
            {
                storage.error = "Ошибка в строке: " + (oper_2.line + 1) + " символ: " + (oper_2.character + 1) + " деление на ноль";
                return;
            }

            int res_num = storage.memory[oper_2.value][oper_2.address] / storage.memory[oper_1.value][oper_1.address];

            if (!storage.memory.ContainsKey(res_num.ToString()))
            {
                storage.memory.Add(res_num.ToString(), new int[1] { res_num });
            }

            RPN_unit res = new RPN_unit(res_num.ToString(), "constant", 0, oper_2.line, oper_2.character);
            magazine.Push(res);
        }
        private void Equal()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] == storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void UnEqual()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] != storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void Less()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] < storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void Greater()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] > storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void LessEqual()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] <= storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void GreaterEqual()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            RPN_unit res;
            if (storage.memory[oper_2.value][oper_2.address] >= storage.memory[oper_1.value][oper_1.address])
            {
                res = new RPN_unit("true", "bool", -1, oper_2.line, oper_2.character);
            }
            else
            {
                res = new RPN_unit("false", "bool", -1, oper_2.line, oper_2.character);
            }
            magazine.Push(res);
        }
        private void SetIndex()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            if (storage.memory[oper_1.value][oper_1.address] > storage.memory[oper_2.value].Length - 1)
            {
                storage.error = "Ошибка в строке: " + (oper_2.line + 1) + " символ: " + (oper_2.line + 1) + " индекс выходит за пределы массива";
                return;
            }
            
            RPN_unit res = new RPN_unit(oper_2.value, oper_2.type, storage.memory[oper_1.value][oper_1.address], oper_2.line, oper_2.character);
            magazine.Push(res);
        }
        private void Negative()
        {
            RPN_unit oper_1 = magazine.Pop();

            int res_num = storage.memory[oper_1.value][oper_1.address] * -1;

            if (!storage.memory.ContainsKey(res_num.ToString()))
            {
                storage.memory.Add(res_num.ToString(), new int[1] { res_num });
            }
            
            RPN_unit res = new RPN_unit(res_num.ToString(), "constant", 0, oper_1.line, oper_1.character);
            magazine.Push(res);
        }
        private void Read()
        {
            int oper_1 = Convert.ToInt32(Console.ReadLine());
            RPN_unit oper_2 = magazine.Pop();

            if (oper_2.type != "name")
            {
                storage.error = "Ошибка в строке: " + (oper_2.line + 1) + " символ: " + (oper_2.character + 1) + " недопустимый тип операнда для данной операции";
                return;
            }
            else
            {
                storage.memory[oper_2.value][oper_2.address] = oper_1;
            }
        }
        private void Write()
        {
            RPN_unit oper_1 = magazine.Pop();

            Console.WriteLine(storage.memory[oper_1.value][oper_1.address].ToString());
        }
        private void Jump()
        {
            RPN_unit oper_1 = magazine.Pop();

            i = storage.memory[oper_1.value][oper_1.address] - 1;
        }
        private void JumpFalse()
        {
            RPN_unit oper_1 = magazine.Pop();
            RPN_unit oper_2 = magazine.Pop();

            if (oper_2.value == "false")
            {
                i = storage.memory[oper_1.value][oper_1.address] - 1;
            }
        }
        private void DoOperation(string operation)
        {
            switch (operation)
            {
                case "=": Assign(); break;
                case "+": Add(); break;
                case "-": Sub(); break;
                case "*": Mult(); break;
                case "/": Div(); break;
                case "==": Equal(); break;
                case "!=": UnEqual(); break;
                case "<": Less(); break;
                case ">": Greater(); break;
                case "<=": LessEqual(); break;
                case ">=": GreaterEqual(); break;
                case "i": SetIndex(); break;
                case "-'": Negative(); break;
                case "r": Read(); break;
                case "w": Write(); break;
                case "j": Jump(); break;
                case "jf": JumpFalse(); break;
            }
        }
        public void Interpret()
        {
            for (i = 0; i < storage.RPN.Count && storage.error == ""; i++)
            {
                if (storage.RPN[i].type != "operation")
                {
                    magazine.Push(storage.RPN[i]);
                }
                else
                {
                    DoOperation(storage.RPN[i].value);
                }
            }
        }
    }
}
