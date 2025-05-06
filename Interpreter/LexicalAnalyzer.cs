using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class LexicalAnalyzer
    {   
        private string name;
        private int number;
        private string oper;
        private int line;
        private int character;

                         //      б   ц   +   -   *   /   [   ]   (   )   {   }   <   >   !   _   =   ;  \n  \0   д
        int[,] state_table = { 
                         /*S*/ { 1,  2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,  3,  3,  4,  0,  3, -2,  0, -2, -3}, 
                         /*L*/ { 1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -3, -3, -1, -1, -1, -2, -1, -1, -3, -3, -3},
                         /*N*/ {-3,  2, -1, -1, -1, -1, -3, -3, -3, -1, -3, -3, -1, -1, -1, -2, -1, -1, -3, -3, -3},
                         /*C*/ {-1, -1, -3, -1, -3, -3, -3, -3, -1, -3, -3, -3, -3, -3, -3, -2, -2, -3, -3, -3, -3},
                         /*E*/ {-3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -2, -3, -3, -3, -3},
                             };

                         //      б   ц   +   -   *   /   [   ]   (   )   {   }   <   >   !   _   =   ;  \n  \0   д
        int[,] program_table = { 
                         /*S*/ { 0,  1,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 14, 14, 20, 14, 21, 19, -1, -1}, 
                         /*L*/ { 2,  2, 17, 17, 17, 17, 17, 17, 17, 17, -1, -1, 17, 17, 17, 17, 17, 17, -1, -1, -1},
                         /*N*/ {-1,  3, 18, 18, 18, 18, -1, -1, -1, 18, -1, -1, 18, 18, 18, 18, 18, 18, -1, -1, -1},
                         /*C*/ {16, 16, -1, 16, -1, -1, -1, -1, 16, -1, -1, -1, -1, -1, -1, 16, 15, -1, -1, -1, -1},
                         /*E*/ {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 15, -1, -1, -1, -1},
                             };

        public List<int> lexeme_code = new List<int>();
        public List<string> lexeme_str = new List<string>();

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
            lexeme_code.Add(3); 
            lexeme_str.Add("+");
        }
        private void SemanticProgram5(char c)
        {
            lexeme_code.Add(4);
            lexeme_str.Add("-");
        }
        private void SemanticProgram6(char c)
        {
            lexeme_code.Add(5);
            lexeme_str.Add("*");
        }
        private void SemanticProgram7(char c)
        {
            lexeme_code.Add(6);
            lexeme_str.Add("/");
        }
        private void SemanticProgram8(char c)
        {
            lexeme_code.Add(15);
            lexeme_str.Add("[");
        }
        private void SemanticProgram9(char c)
        {
            lexeme_code.Add(16);
            lexeme_str.Add("]");
        }
        private void SemanticProgram10(char c)
        {
            lexeme_code.Add(13);
            lexeme_str.Add("(");
        }
        private void SemanticProgram11(char c)
        {
            lexeme_code.Add(14);
            lexeme_str.Add(")");
        }
        private void SemanticProgram12(char c)
        {
            lexeme_code.Add(17);
            lexeme_str.Add("{");
        }
        private void SemanticProgram13(char c)
        {
            lexeme_code.Add(18);
            lexeme_str.Add("}");
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
                lexeme_code.Add(7);
            }
            else if (oper == "!=")
            {
                lexeme_code.Add(8);
            }
            else if (oper == "<")
            { 
                lexeme_code.Add(9);
            }
            else if (oper == ">")
            {
                lexeme_code.Add(10);
            }
            else if (oper == "<=")
            {
                lexeme_code.Add(11);
            }
            else if (oper == ">=")
            {
                lexeme_code.Add(12);
            }

            lexeme_str.Add(oper);
        }
        private void SemanticProgram16(char c)
        {
            lexeme_code.Add(2);
            lexeme_str.Add(oper);
        }
        private void SemanticProgram17(char c)
        {
            if (name == "read")
            {
                lexeme_code.Add(20);
            }
            else if (name == "write")
            {
                lexeme_code.Add(21);
            }
            else if (name == "if")
            {
                lexeme_code.Add(22);
            }
            else if (name == "else")
            {
                lexeme_code.Add(23);
            }
            else if (name == "while")
            {
                lexeme_code.Add(24);
            }
            else if (name == "int")
            {
                lexeme_code.Add(25);
            }
            else 
            { 
                lexeme_code.Add(0); 
            }

            lexeme_str.Add(name);
        }
        private void SemanticProgram18(char c)
        {
            lexeme_code.Add(1);
            lexeme_str.Add(number.ToString());
        }
        private void SemanticProgram19(char c)
        {
            line++;
            character = 0;
        }
        private void SemanticProgram20(char c)
        {

        }
        private void SemanticProgram21(char c)
        {
            lexeme_code.Add(19);
            lexeme_str.Add(";");
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
            }
        }
        public void Analyze(string program)
        {
            int state = 0;
            character = 0;
            for (int i = 0; i < program.Length; i++, character++)
            {
                char c = program[i];
                int column = SymbolToColumnNumber(c);

                RunSemanticProgram(program_table[state, column], c);
                state = state_table[state, column];

                if (state == -1)
                {
                    state = 0;
                    i--;
                }
                else if (state == -2)
                {
                    state = 0;
                }
                else if (state == -3)
                {
                    lexeme_code.Add(-1);
                    lexeme_str.Add("Error at line: " + line + " character: " + character);
                    return;
                }
            }    
        }
    }
}