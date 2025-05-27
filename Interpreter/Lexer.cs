using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class Lexer
    {
        private Storage storage;
        private string name;
        private int number;
        private string oper;
        private int line;
        private int character;
        int state;

                         //      б   ц   +   -   *   /   [   ]   (   )   {   }   <   >   !   _   =   ;  \n  \0   д
        int[,] state_table = { 
                         /*S*/ { 1,  2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,  5,  5,  4,  0,  3, -2,  0, -2, -3}, 
                         /*L*/ { 1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -3, -1, -1, -1, -2, -1, -1, -2, -3, -3},
                         /*N*/ {-3,  2, -1, -1, -1, -1, -3, -1, -3, -1, -3, -3, -1, -1, -1, -2, -1, -1, -3, -3, -3},
                         /*C*/ {-1, -1, -3, -1, -3, -3, -3, -3, -1, -3, -3, -3, -3, -3, -3, -2, -2, -3, -3, -3, -3},
                         /*E*/ {-3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -2, -3, -3, -3, -3},
                         /*I*/ {-1, -1, -3, -1, -3, -3, -3, -3, -1, -3, -3, -3, -3, -3, -3, -2, -2, -3, -3, -3, -3}
                             };

                         //      б   ц   +   -   *   /   [   ]   (   )   {   }   <   >   !   _   =   ;  \n  \0   д
        int[,] program_table = { 
                         /*S*/ { 0,  1,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 14, 14, 21, 14, 22, 20, -1, -1}, 
                         /*L*/ { 2,  2, 18, 18, 18, 18, 18, 18, 18, 18, 18, -1, 18, 18, 18, 18, 18, 18, 18, -1, -1},
                         /*N*/ {-1,  3, 19, 19, 19, 19, -1, 19, -1, 19, -1, -1, 19, 19, 19, 19, 19, 19, -1, -1, -1},
                         /*C*/ {17, 17, -1, 17, -1, -1, -1, -1, 17, -1, -1, -1, -1, -1, -1, 17, 15, -1, -1, -1, -1},
                         /*E*/ {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 15, -1, -1, -1, -1},
                         /*I*/ {16, 16, -1, 16, -1, -1, -1, -1, 16, -1, -1, -1, -1, -1, -1, 16, 15, -1, -1, -1, -1}
                             };

        public Lexer(Storage storage)
        {
            this.storage = storage;
        }

        private int SymbolToColumnNumber(char c)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                return 0;
            }

            if (c >= '0' && c <= '9')
            { 
                return 1; 
            }

            int column_number = "+-*/[](){}<>! =;\n\0".IndexOf(c) + 2;
            if (column_number != -1)
            {
                return column_number;
            }
            
            return 21;
        }

        private void SemanticProgram0(char c)
        {
            name = c.ToString();
        }
        private void SemanticProgram1(char c)
        {
            number = c - '0';
        }
        private void SemanticProgram2(char c)
        {
            name += c.ToString();
        }
        private void SemanticProgram3(char c)
        {
            number = number * 10 + (c - '0');
        }
        private void SemanticProgram4(char c)
        {
            storage.lexems.Add(new Lexem(3, "+", line, character));
        }
        private void SemanticProgram5(char c)
        {
            storage.lexems.Add(new Lexem(4, "-", line, character));
        }
        private void SemanticProgram6(char c)
        {
            storage.lexems.Add(new Lexem(5, "*", line, character));
        }
        private void SemanticProgram7(char c)
        {
            storage.lexems.Add(new Lexem(6, "/", line, character));
        }
        private void SemanticProgram8(char c)
        {
            storage.lexems.Add(new Lexem(15, "[", line, character));
        }
        private void SemanticProgram9(char c)
        {
            storage.lexems.Add(new Lexem(16, "]", line, character));
        }
        private void SemanticProgram10(char c)
        {
            storage.lexems.Add(new Lexem(13, "(", line, character));
        }
        private void SemanticProgram11(char c)
        {
            storage.lexems.Add(new Lexem(14, ")", line, character));
        }
        private void SemanticProgram12(char c)
        {
            storage.lexems.Add(new Lexem(17, "{", line, character));
        }
        private void SemanticProgram13(char c)
        {
            storage.lexems.Add(new Lexem(18, "}", line, character));
        }
        private void SemanticProgram14(char c)
        {
            oper = c.ToString();
        }
        private void SemanticProgram15(char c)
        {
            oper += c.ToString();
            
            if (oper == "==")
            {
                storage.lexems.Add(new Lexem(7, "==", line, character));
            }
            else if (oper == "!=")
            {
                storage.lexems.Add(new Lexem(8, "!=", line, character));
            }
            else if (oper == "<=")
            {
                storage.lexems.Add(new Lexem(11, "<=", line, character));
            }
            else if (oper == ">=")
            {
                storage.lexems.Add(new Lexem(12, ">=", line, character));
            }
        }
        private void SemanticProgram16(char c)
        {
            if (oper == "<")
            {
                storage.lexems.Add(new Lexem(9, "<", line, character));
            }
            else if (oper == ">")
            {
                storage.lexems.Add(new Lexem(10, ">", line, character));
            }
        }
        private void SemanticProgram17(char c)
        {
            storage.lexems.Add(new Lexem(2, "=", line, character));
        }
        private void SemanticProgram18(char c)
        {
            if (name == "read")
            {
                storage.lexems.Add(new Lexem(20, "read", line, character));
            }
            else if (name == "write")
            {
                storage.lexems.Add(new Lexem(21, "write", line, character));
            }
            else if (name == "if")
            {
                storage.lexems.Add(new Lexem(22, "if", line, character));
            }
            else if (name == "else")
            {
                storage.lexems.Add(new Lexem(23, "else", line, character));
            }
            else if (name == "while")
            {
                storage.lexems.Add(new Lexem(24, "while", line, character));
            }
            else if (name == "int")
            {
                storage.lexems.Add(new Lexem(25, "int", line, character));
            }
            else 
            {
                storage.lexems.Add(new Lexem(0, name, line, character));
            }
        }
        private void SemanticProgram19(char c)
        {
            storage.lexems.Add(new Lexem(1, number.ToString(), line, character));
            storage.constants.Enqueue(number);
        }
        private void SemanticProgram20(char c)
        {
            line++;
            character = -1;
        }
        private void SemanticProgram21(char c)
        {

        }
        private void SemanticProgram22(char c)
        {
            storage.lexems.Add(new Lexem(19, ";", line, character));
        }

        private void RunSemanticProgram(int program_num, char c)
        {
            switch (program_num)
            {
                case 0: SemanticProgram0(c); break;
                case 1: SemanticProgram1(c); break;
                case 2: SemanticProgram2(c); break;
                case 3: SemanticProgram3(c); break;
                case 4: SemanticProgram4(c); break;
                case 5: SemanticProgram5(c); break;
                case 6: SemanticProgram6(c); break;
                case 7: SemanticProgram7(c); break;
                case 8: SemanticProgram8(c); break;
                case 9: SemanticProgram9(c); break;
                case 10: SemanticProgram10(c); break;
                case 11: SemanticProgram11(c); break;
                case 12: SemanticProgram12(c); break;
                case 13: SemanticProgram13(c); break;
                case 14: SemanticProgram14(c); break;
                case 15: SemanticProgram15(c); break;
                case 16: SemanticProgram16(c); break;
                case 17: SemanticProgram17(c); break;
                case 18: SemanticProgram18(c); break;
                case 19: SemanticProgram19(c); break;
                case 20: SemanticProgram20(c); break;
                case 21: SemanticProgram21(c); break;
                case 22: SemanticProgram22(c); break;
            }
        }
        public void Analyze(string program)
        {
            state = 0;
            character = 0;
            line = 0;
            storage.lexems.Clear();

            for (int i = 0; i < program.Length; i++, character++)
            {
                char c = program[i];
                int column = SymbolToColumnNumber(c);

                RunSemanticProgram(program_table[state, column], c);
                state = state_table[state, column];

                if (state == -1)
                {
                    storage.lexems[storage.lexems.Count - 1].character--;
                    state = 0;
                    i--;
                    character--;
                }
                else if (state == -2)
                {
                    state = 0;
                }
                else if (state == -3)
                {
                    storage.lexems.Add(new Lexem(-1, "Error", line, character));
                    return;
                }
            }
        }
    }
}