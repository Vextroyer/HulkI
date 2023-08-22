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
        }
        //If an exception is launched, means an error ocurred and so the processing of this expression is ended.
        catch(Exception e){
            return null;
        }
    }

    //Maneja la presentacion de los errores en pantalla
    private static void Error(string message, string errorType = "ERROR"){
        Console.Error.WriteLine("! "+ errorType + "    " + message);
    }
    internal static void ScannerError(ScannerException e){
        Error(e.Message,e.ErrorType());
        Console.Error.WriteLine("In: " + e.Contexto);
        for(int i=0;i < 4 + e.Ofsset;++i)Console.Error.Write(' ');
        Console.Error.Write('^');
        Console.Error.WriteLine();
    }

    internal static void ParserError(ParserException e){
        Error(e.Message,e.ErrorType());
    }

    internal static void InterpreterError(InterpreterException e){
        Error(e.Message,e.ErrorType());
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