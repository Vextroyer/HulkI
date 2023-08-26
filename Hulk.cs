﻿namespace Hulk;

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
                //If the input is null or empty someone just hit enter. Ignore it and continue the REPL mode.
            }
        }
    }

    //Interpret and run the content of of the source.
    //Return the result of evaluate the expression or null if an error ocurred.
    //In the case of an error it will automatically report it before returning.
    public static string? Run(string? source){
        if(string.IsNullOrEmpty(source))throw new ArgumentException();
        try{
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
    private static void Error(string message, string errorType = "ERROR"){
        Console.Error.WriteLine("! "+ errorType + "    " + message);
    }
    //Handle specific error printing on the CLI. 
    internal static void Error(ScannerException e){
        Error(e.Message,e.ErrorType());
        Console.Error.WriteLine("In: " + e.Contexto);
        for(int i=0;i < 4 + e.Ofsset;++i)Console.Error.Write(' ');
        Console.Error.Write('^');
        Console.Error.WriteLine();
    }
    internal static void Error(ParserException e){
        Error(e.Message,e.ErrorType());
    }
    internal static void Error(InterpreterException e){
        Error(e.Message,e.ErrorType());
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