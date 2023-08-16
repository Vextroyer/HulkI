namespace Hulk;

/*
The backbone of the Hulk interpreter.
*/
class Hulk{
    public static void Main(string[] args){
        //Interactive mode
        while(true){
            Console.Write("> ");
            try{
                string? source = Console.ReadLine();
                Run(source);
            }catch(ArgumentException){
                //En el caso de que la entrada sea nula o vacia no hacer nada
            }
        }
    }

    //Interpret an run the content of of the source 
    private static void Run(string? source){
        if(string.IsNullOrEmpty(source))throw new ArgumentException();
        try{
            //Scan the input
            Scanner scanner = new Scanner(source);
            List<Token> tokens;
            tokens = scanner.Scan();
            //DebugScanner(tokens);

            //Parse the input
            Parser parser = new Parser(tokens);
            Expr expr = parser.Parse();
            //DebugParser(expr);

            // Interpret the input
            Interpreter interpreter = new Interpreter();
            interpreter.Interpret(expr);
        }catch(ScannerException e){
            Error(e.Message,e.Contexto,e.Ofsset);
            return;
        }catch(RuntimeError e){
            Error(e.Message);
        }catch(Exception e){
            Error(e.Message);
            return;
        }
    }
    //Maneja la presentacion de los errores en pantalla
    private static void Error(string message){
        Console.Error.WriteLine("!ERROR " + message);
    }
    private static void Error(string message,string contexto,int offset){
        Error(message);
        Console.Error.WriteLine("In: " + contexto);
        for(int i=0;i < 4 + offset;++i)Console.Error.Write(' ');
        Console.Error.Write('^');
        Console.Error.WriteLine();
    }

    private static void DebugScanner(List<Token> tokens){
        //Print tokens
        foreach(Token token in tokens){
             Console.WriteLine(token);
        }
    }

    private static void DebugParser(Expr expr){
        //Print the ast
        AstPrinter printer = new AstPrinter();
        Console.WriteLine(printer.Print(expr));
    }
}