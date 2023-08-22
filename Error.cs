/*
Definir los diferentes tipos de errores que pueden ocurrir en la interpretacion de un
programa de Lox.
*/
namespace Hulk;

interface HulkTypeError {
    public string ErrorType();
}

/*
Este tipo de excepciones ocurren durante el escaneo del codigo fuente
y representan los errores lexicos de Hulk
*/
class ScannerException : Exception,HulkTypeError{
    public string ErrorType(){
        return "LEXICAL ERROR";
    }
    public ScannerException():base(){
        this.Contexto = "";
        this.Ofsset = 0;
    }
    public ScannerException(string? message, string contexto, int offset):base(message){
        this.Contexto = contexto;
        this.Ofsset = offset;
    }

    //La linea donde ocurrio el error
    public string Contexto {get;}
    //Cantidad de caracteres desde el inicio de la linea donde se descubrio el error. Es una aproximacion de la ubicacion.
    public int Ofsset {get;}
}

/*
Este tipo de excepciones ocurren al parsear los tokens generados por el Scanner y representan los errores
sintacticos de Hulk.
*/
class ParserException : Exception,HulkTypeError{
    public ParserException(string message) : base(message){}

    public string ErrorType(){
        return "SYNTACTIC ERROR";
    }
}

/*
Este tipo de excepciones ocurren al interpretar las expresiones parseadas y representan los errores
semnticos de Hulk.
*/
class InterpreterException : Exception,HulkTypeError{
    public InterpreterException(string message) : base(message){}

    public string ErrorType(){
        return "SEMANTIC ERROR";
    }
}

/*
A resource used in handling errors. Expected exceptions throw this exception. Unexpected ones do not.
*/
class HandledException : Exception{}