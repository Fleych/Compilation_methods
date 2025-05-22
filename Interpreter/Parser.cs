using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Interpreter
{
    internal class Parser
    {
        private Storage storage;
        private Stack<int> magazine = new Stack<int>();
        private Stack<int> generator = new Stack<int>();
        private Stack<int> mark = new Stack<int>();
        private int i = 0;
        private int memory_use = 0;

        public Parser(Storage storage)
        {
            this.storage = storage;
        }

        //                      v               c           =          +                 -            *            /          ==          !=           <            >           <=           >=                      (      )             [      ]           {      }      ;                    read                write                             if     else                       while               int
        int[,][] rule_table = { 
                       /*P*/ {[0,-10,2,-4,19,-1],             [],         [],        [],               [],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],           [],    [], [17,-1,18],    [],    [], [20,13,0,-10,14,19,-1], [21,13,-4,14,19,-1], [22,13,-12,14,17,-1,18,-2,-1],      [], [24,13,-12,14,17,-1,18,-1], [25,-11,0,19,-1]},
                       /*E*/ {                [],             [],         [],        [],               [],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],           [],    [],         [],    [],    [],                     [],                  [],                            [], [23,-3],                         [],               []},
                       /*F*/ {             [-18],          [-18],      [-18],     [-18],            [-18],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],                 [-18], [-18],        [-18], [-18], [17,-1,18], [-18], [-18],                  [-18],               [-18],    [22,13,-12,14,17,-1,18,-2],   [-18],                      [-18],            [-18]},
                       /*X*/ {     [0,-10,-7,-5],      [1,-7,-5],      [-18],     [-18],     [4,-9,-7,-5],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],      [13,-4,14,-7,-5], [-18],        [-18], [-18],      [-18], [-18], [-18],                  [-18],               [-18],                         [-18],   [-18],                      [-18],            [-18]},
                       /*N*/ {                [],             [],         [], [3,-6,-5],        [4,-6,-5],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],           [],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []},
                       /*Y*/ {        [0,-10,-7],        [1,-10],      [-18],     [-18],        [4,-9,-7],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],         [13,-4,14,-7], [-18],        [-18], [-18],      [-18], [-18], [-18],                  [-18],               [-18],                         [-18],   [-18],                      [-18],            [-18]},
                       /*M*/ {                [],             [],         [],        [],               [],   [5,-8,-7],   [6,-8,-7],         [],         [],         [],          [],          [],          [],                    [],    [],           [],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []},
                       /*U*/ {           [0,-10],            [1],      [-18],     [-18],       [4,-9,-14],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],            [13,-4,14], [-18],        [-18], [-18],      [-18], [-18], [-18],                  [-18],               [-18],                         [-18],   [-18],                      [-18],            [-18]},
                       /*Z*/ {           [0,-10],            [1],      [-18],     [-18],            [-18],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],            [13,-4,14], [-18],        [-18], [-18],      [-18], [-18], [-18],                  [-18],               [-18],                         [-18],   [-18],                      [-18],            [-18]},
                       /*A*/ {                [],             [],         [],        [],               [],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],   [15,-4,16],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []},
                       /*L*/ {                [],             [],         [],        [],               [],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],    [15,1,16],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []},
                       /*C*/ { [0,-10,-7,-5,-13],  [1,-7,-5,-13],      [-18],     [-18], [4,-9,-7,-5,-13],       [-18],       [-18],      [-18],      [-18],      [-18],       [-18],       [-18],       [-18],  [13,-4,14,-7,-5,-13], [-18],        [-18], [-18],      [-18], [-18], [-18],                  [-18],               [-18],                         [-18],   [-18],                      [-18],            [-18]},
                       /*H*/ {                [],             [],         [],        [],               [],          [],          [], [7,-4,-14], [8,-4,-14], [9,-4,-14], [10,-4,-14], [11,-4,-14], [12,-4,-14],                    [],    [],           [],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []},
                       /*Q*/ {                [],             [],         [],        [],               [],          [],          [],         [],         [],         [],          [],          [],          [],                    [],    [],           [],    [],         [],    [],    [],                     [],                  [],                            [],      [],                         [],               []}
                           };

                       //                     v             c          =          +                 -          *          /         ==         !=          <           >          <=          >=                    (   )             [   ]           {   }   ;                   read                write                            if     else                      while               int
        int[,][] action_table = { 
                       /*P*/ {[0,-1,-1,-1,-1,2],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],           [], [], [-1,-1,-1], [], [], [-1,-1,0,-1,14,-1,-1], [-1,-1,-1,15,-1,-1], [-1,-1,-1,21,-1,-1,-1,-1,23],      [], [24,-1,-1,-1,21,-1,-1,25], [28,-1,26,-1,-1]},
                       /*E*/ {               [],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],           [], [],         [], [], [],                    [],                  [],                           [], [22,-1],                        [],               []},
                       /*F*/ {               [],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],           [], [], [-1,-1,-1], [], [],                    [],                  [], [-1,-1,-1,21,-1,-1,-1,-1,23],      [],                        [],               []},
                       /*X*/ {     [0,-1,-1,-1],    [1,-1,-1],        [],        [],    [-1,-1,-1,13],        [],        [],        [],        [],        [],         [],         [],         [],    [-1,-1,-1,-1,-1], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*N*/ {               [],           [],        [], [-1,-1,3],        [-1,-1,4],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*Y*/ {        [0,-1,-1],       [1,-1],        [],        [],       [-1,-1,13],        [],        [],        [],        [],        [],         [],         [],         [],       [-1,-1,-1,-1], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*M*/ {               [],           [],        [],        [],               [], [-1,-1,5], [-1,-1,6],        [],        [],        [],         [],         [],         [],                  [], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*U*/ {           [0,-1],          [1],        [],        [],       [-1,-1,13],        [],        [],        [],        [],        [],         [],         [],         [],          [-1,-1,-1], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*Z*/ {           [0,-1],          [1],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],          [-1,-1,-1], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*A*/ {               [],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],   [-1,-1,16], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*L*/ {               [],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],   [-1,27,-1], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*C*/ {  [0,-1,-1,-1,-1], [1,-1,-1,-1],        [],        [], [-1,-1,-1,-1,13],        [],        [],        [],        [],        [],         [],         [],         [], [-1,-1,-1,-1,-1,-1], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*H*/ {               [],           [],        [],        [],               [],        [],        [], [-1,-1,7], [-1,-1,8], [-1,-1,9], [-1,-1,10], [-1,-1,11], [-1,-1,12],                  [], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []},
                       /*Q*/ {               [],           [],        [],        [],               [],        [],        [],        [],        [],        [],         [],         [],         [],                  [], [],           [], [],         [], [], [],                    [],                  [],                           [],      [],                        [],               []}
                           };

        private void SemanticProgram0(string l)
        {
            storage.RPN.Add(l);
            storage.RPN_types.Add("name");
        }
        private void SemanticProgram1(string l)
        {
            storage.RPN.Add(l);
            storage.RPN_types.Add("constant");
        }
        private void SemanticProgram2(string l)
        {
            storage.RPN.Add("=");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram3(string l)
        {
            storage.RPN.Add("+");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram4(string l)
        {
            storage.RPN.Add("-");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram5(string l)
        {
            storage.RPN.Add("*");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram6(string l)
        {
            storage.RPN.Add("/");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram7(string l)
        {
            storage.RPN.Add("==");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram8(string l)
        {
            storage.RPN.Add("!=");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram9(string l)
        {
            storage.RPN.Add("<");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram10(string l)
        {
            storage.RPN.Add(">");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram11(string l)
        {
            storage.RPN.Add("<=");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram12(string l)
        {
            storage.RPN.Add(">=");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram13(string l)
        {
            storage.RPN.Add("-'");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram14(string l)
        {
            storage.RPN.Add("r");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram15(string l)
        {
            storage.RPN.Add("w");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram16(string l)
        {
            storage.RPN.Add("i");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram21(string l)
        {
            storage.RPN.Add("");
            storage.RPN_types.Add("mark");

            mark.Push(storage.RPN.Count() - 1);
            
            storage.RPN.Add("jf");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram22(string l)
        {
            storage.RPN[mark.Pop()] = (storage.RPN.Count() + 2).ToString();
            
            storage.RPN.Add("");
            storage.RPN_types.Add("mark");
            
            mark.Push(storage.RPN.Count() - 1);
            
            storage.RPN.Add("j");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram23(string l)
        {
            storage.RPN[mark.Pop()] = (storage.RPN.Count()).ToString();
        }
        private void SemanticProgram24(string l)
        {
            mark.Push(storage.RPN.Count());
        }
        private void SemanticProgram25(string l)
        {
            storage.RPN[mark.Pop()] = (storage.RPN.Count() + 2).ToString();
            
            storage.RPN.Add(mark.Pop().ToString());
            storage.RPN_types.Add("mark");

            storage.RPN.Add("j");
            storage.RPN_types.Add("operation");
        }
        private void SemanticProgram26(string l)
        {
            storage.vars.Add(l, new int[memory_use]);
        }
        private void SemanticProgram27(string l)
        {
            memory_use = storage.constants.Dequeue();
        }
        private void SemanticProgram28(string l)
        {
            memory_use = 1;
        }
        private void RunSemanticProgram(int program_num, string l)
        {
            switch (program_num)
            {
                case 0: SemanticProgram0(l); break;
                case 1: SemanticProgram1(l); break;
                case 2: SemanticProgram2(l); break;
                case 3: SemanticProgram3(l); break;
                case 4: SemanticProgram4(l); break;
                case 5: SemanticProgram5(l); break;
                case 6: SemanticProgram6(l); break;
                case 7: SemanticProgram7(l); break;
                case 8: SemanticProgram8(l); break;
                case 9: SemanticProgram9(l); break;
                case 10: SemanticProgram10(l); break;
                case 11: SemanticProgram11(l); break;
                case 12: SemanticProgram12(l); break;
                case 13: SemanticProgram13(l); break;
                case 14: SemanticProgram14(l); break;
                case 15: SemanticProgram15(l); break;
                case 16: SemanticProgram16(l); break;
                case 21: SemanticProgram21(l); break;
                case 22: SemanticProgram22(l); break;
                case 23: SemanticProgram23(l); break;
                case 24: SemanticProgram24(l); break;
                case 25: SemanticProgram25(l); break;
                case 26: SemanticProgram26(l); break;
                case 27: SemanticProgram27(l); break;
                case 28: SemanticProgram28(l); break;
            }
        }
        private string lexem_code_to_str_err(int code)
        {
            switch (code)
            {
                case 0: return "Имя";
                case 1: return "Константа";
                case 2: return "=";
                case 3: return "+";
                case 4: return "-";
                case 5: return "*";
                case 6: return "/";
                case 7: return "==";
                case 8: return "!=";
                case 9: return "<";
                case 10: return ">";
                case 11: return "<=";
                case 12: return ">=";
                case 13: return "(";
                case 14: return ")";
                case 15: return "[";
                case 16: return "]";
                case 17: return "{";
                case 18: return "}";
                case 19: return ";";
                case 20: return "read";
                case 21: return "write";
                case 22: return "if";
                case 23: return "else";
                case 24: return "while";
                case 25: return "int";
            }
            return "";
        }
        public void Analyze()
        {
            magazine.Clear();
            generator.Clear();

            magazine.Push(-1);
            generator.Push(-1);

            int symbol;
            int lexem;
            int action;

            while (i < storage.lexems.Count)
            {
                symbol = magazine.Pop();
                lexem = storage.lexems[i].code;

                if (symbol < 0)
                {
                    if (rule_table[symbol * -1 - 1, lexem].Length != 0)
                    {
                        if (rule_table[symbol * -1 - 1, lexem][0] == -18)
                        {
                            //error
                            Console.WriteLine("1");
                            return;
                        }
                    }

                    for (int j = rule_table[symbol * -1 - 1, lexem].Length - 1; j >= 0; j--)
                    {
                        magazine.Push(rule_table[symbol * -1 - 1, lexem][j]);
                    }

                    action = generator.Pop();

                    RunSemanticProgram(action, storage.lexems[i].str);

                    for (int j = action_table[symbol * -1 - 1, lexem].Length - 1; j >= 0; j--)
                    {
                        generator.Push(action_table[symbol * -1 - 1, lexem][j]);
                    }
                }
                else if (symbol >= 0)
                {
                    if (symbol == storage.lexems[i].code)
                    {
                        action = generator.Pop();

                        RunSemanticProgram(action, storage.lexems[i].str);

                        i++;
                    }
                    else
                    {
                        storage.RPN.Add("Ошибка в строке: " + storage.lexems[i].line + " Символ: " + storage.lexems[i].character + " Втречено: " + storage.lexems[i].str + ", ожидалось: " + lexem_code_to_str_err(symbol));
                        storage.RPN_types.Add("Error");
                        return;
                    }
                }
            }
        }
    }
}
