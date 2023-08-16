/*
Definir los diferentes tipos de errores que pueden ocurrir en la interpretacion de un
programa de Lox.
*/
namespace Hulk;

/*
Este tipo de excepciones ocurren durante el escaneo del codigo fuente
y representan los errores lexicos de Hulk
*/
class ScannerException : Exception{
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

class RuntimeError : Exception{
    public RuntimeError(string? message) : base(message){

    }
}