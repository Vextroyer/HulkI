namespace Hulk;

/*
The backbone of the Hulk interpreter.
*/
class Hulk{
    public static void Main(string[] args){
        //Testing Mode
        if(args.Length > 0 && args[0] == "test")
        {
            Tester.Test();
            Environment.Exit(0);
        }


        //Interactive mode
        while(true){
            Console.Write("> ");
            try{
                string? source = Console.ReadLine();
                Console.WriteLine(Run(source));
            }catch(ArgumentException){
                //En el caso de que la entrada sea nula o vacia no hacer nada
            }
        }
    }

    //Interpret an run the content of of the source 
    public static string Run(string? source){
        if(string.IsNullOrEmpty(source))throw new ArgumentException();
        try{
            //Scan the input
            Scanner scanner = new Scanner(source);
            List<Token> tokens;
            tokens = scanner.Scan();
            // DebugScanner(tokens); // Comentar o descomentar para debugear

            //Parse the input
            Parser parser = new Parser(tokens);
            Expr expr = parser.Parse();
            // DebugParser(expr); // Comentar o descomentar para debugear

            // Interpret the input
            Interpreter interpreter = new Interpreter();
            return interpreter.Interpret(expr);
        }catch(ScannerException e){
            Error(e.Message,e.Contexto,e.Ofsset);
            return "";
        }catch(RuntimeError e){
            Error(e.Message);
            return "";
        }catch(Exception e){
            Error(e.Message);
            return "";
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