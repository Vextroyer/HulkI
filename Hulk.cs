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
            System.Environment.Exit(0);
        }

        //Error Mode
        //This is a provisional mode for evaluating expressions from a file
        if(args.Length > 0){
            //Open the file
            StreamReader errors = new StreamReader("Error.aux");
            
            //While is not at end
            while(!errors.EndOfStream){
                string line = errors.ReadLine();
                if(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))continue;
                line.Trim();
                if(line.StartsWith('#'))continue;//A comment
                Console.WriteLine("> " + line);
                Run(line);//It will always throw an exception because its for cheking errors
            }

            //Close it
            errors.Close();
            System.Environment.Exit(0);
        }

        //Interactive mode
        while(true){
            Console.Write("> ");
            try{
                string? source = Console.ReadLine();
                Console.WriteLine(Run(source));
            }catch(ArgumentException){
                //If the input is null or empty someone just hit enter. Ignore it and continue the REPL mode.
            }
        }
    }

    //The last line of source code, for printing errors
    private static string lastLine;

    //Interpret and run the content of of the source.
    //Return the result of evaluate the expression or null if an error ocurred.
    //In the case of an error it will automatically report it before returning.
    public static string? Run(string? source){
        if(string.IsNullOrEmpty(source))throw new ArgumentException();
        try{
            //Last line of source code.
            lastLine = source;

            //Scan the input
            Scanner scanner = new Scanner(source);
            List<Token> tokens;
            tokens = scanner.Scan();
            // DebugScanner(tokens); // Comment or uncomment for debuging

            //Parse the input
            Parser parser = new Parser(tokens);
            Expr expr = parser.Parse();
            // DebugParser(expr); // Comment or uncomment for debuging

            // Interpret the input
            Interpreter interpreter = new Interpreter();
            return interpreter.Interpret(expr);
        }
        //If an exception is launched, means an error ocurred ,so stop the process.
        catch(HulkException){
            return null;
        }
        //For unexpected exceptions
        catch(Exception e){
            Console.WriteLine("UNHANDLED EXCEPTION");
            Console.WriteLine(e.Message);
            return null;
        }
    }

    //Handle general error printing on the CLI.
    private static void Error(string message, int offset, string errorType = "ERROR"){
        Console.Error.WriteLine("! "+ errorType + "    " + message);
        if(offset <= -1)return;
        Console.Error.WriteLine("In: " + lastLine);
        for(int i=0;i < 4 + offset;++i)Console.Error.Write(' ');
        Console.Error.Write('^');
        Console.Error.WriteLine();
    }
    //Handle specific error printing on the CLI. 
    internal static void Error(ScannerException e){
        Error(e.Message,e.Ofsset,e.ErrorType());
    }
    internal static void Error(ParserException e){
        Error(e.Message,e.Ofsset,e.ErrorType());
    }
    internal static void Error(InterpreterException e){
        Error(e.Message,e.Ofsset,e.ErrorType());
    }

    //Auxiliary methods for debuging purposes.
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