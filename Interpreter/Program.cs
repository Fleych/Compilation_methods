using Interpreter;

LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();

string program = "int a1 = 10;\nif (a1 >= 0) {write(a1 + 7);} else a1 = 0;";

lexicalAnalyzer.Analyze(program);

Console.WriteLine("Номер: " + "Лексема:");

for (int i = 0; i < lexicalAnalyzer.lexeme_code.Count; i++)
{
    Console.WriteLine(lexicalAnalyzer.lexeme_code[i].ToString() + " " + lexicalAnalyzer.lexeme_str[i]);
}