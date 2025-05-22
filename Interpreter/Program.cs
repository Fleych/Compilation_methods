using Interpreter;

Storage storage = new Storage();
Lexer lexer = new Lexer(storage);
Parser parser = new Parser(storage);

Console.WriteLine("Лексический анализатор\n");

/*string program = "{\n" +
                      "int[3] vec;\n" +
                      "int a;\n" +
                      "vec[0] = 2; vec[1] = 5; vec[2] = 7; a = 0;\n" +
                      "while (a < vec[2]) {a = a + 1;}\n" +
                      "if (a != vec[0])\n" +
                      "{\n" +
                      "    write(a);\n" +
                      "}\n" +
                      "else\n" +
                      "{\n" +
                      "    a = a * -1;\n"+
                      "}\n" + 
                  "}"; // <- ДЕНЧИК ЭТО РАБОТАЕТ*/

string program = "{int a;}";

lexer.Analyze(program);

Console.WriteLine("Номер " + "Лексема " + "Строка:символ");

for (int i = 0; i < storage.lexems.Count; i++)
{
    Console.WriteLine(storage.lexems[i].ToString());
}

parser.Analyze();

Console.WriteLine("\nСинтаксический анализатор\n");

Console.WriteLine("Номер: " + "ОПС: " + "     ТИП:");

for (int i = 0; i < storage.RPN.Count; i++)
{
    Console.WriteLine(i.ToString() + "      " + storage.RPN[i] + "      " + storage.RPN_types[i]);
}