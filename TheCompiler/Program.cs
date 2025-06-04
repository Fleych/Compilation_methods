using TheCompiler;

Compiler compiler = new Compiler();

string program = File.ReadAllText("test1.txt");

compiler.Compile(program);