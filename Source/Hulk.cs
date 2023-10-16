namespace Hulk;

/*
The backbone of the Hulk interpreter.
*/
class Hulk{
    private static Interpreter interpreter = new Interpreter();//Establish an interpreter for the entire session.
    public static void Main(string[] args){
        SelectMode(args);
    }

    private static void SelectMode(string[] args){
        if(args.Length == 1){
            if(args[0] == "test"){
                TestingMode();
                return;
            }
            else if(args[0] == "error"){
                ErrorTestingMode();
                return;
            }
        }
        InteractiveMode();
    }
    //Test the interpreter with the provided test cases on the tester class
    private static void TestingMode(){
        Tester.Test();
    }
    //This is a provisional mode for evaluating expressions from a file
    private static void ErrorTestingMode(){
        //This part can fail on opening the file.
        try{
            //Open the file
            StreamReader errors = new StreamReader("Error.aux");
                
            //While is not at end
            while(!errors.EndOfStream){
                string? line = errors.ReadLine();
                if(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))continue;
                line.Trim();
                if(line.StartsWith('#'))continue;//A comment
                Console.WriteLine("> " + line);
                Run(line);//It will always throw an exception because its for cheking errors
            }

            //Close the file
            errors.Close();
        }catch(Exception){
            Console.WriteLine("An error occurred while loading Error.aux file.");
        }
    }
    private static void InteractiveMode(){
        while(true){
            Console.Write("> ");
            try{
                string? source = Console.ReadLine();
                string? output = Run(source);
                Console.WriteLine(output == null?"":output);
            }catch(ArgumentException){
                //If the input is null or empty someone just hit enter. Ignore it and continue the REPL mode.
            }
        }
    }

    //The last line of source code, for printing errors
    private static string lastLine = "";

    //Interpret and run the content of of the source.
    //Return the result of evaluate the expression or null if an error ocurred.
    //In the case of an error it will automatically report it before returning.
    public static string? Run(string? source){
        if(string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))throw new ArgumentException();
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
            Console.WriteLine(e.StackTrace);
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