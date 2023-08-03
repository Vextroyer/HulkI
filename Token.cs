/*
Representa un token devuelto por el scanner
*/

namespace Hulk;

class Token{
    //El tipo de este token
    public TokenType Type {get; private set;}
    //Texto en el codigo fuente desde el que se genero el token
    public string Lexema {get; private set;}
    //Valor que contiene este token
    public object? Literal {get; private set;}

    public Token(TokenType type,string lexema,object? literal){
        this.Type = type;
        this.Lexema = lexema;
        this.Literal = literal;
    }

    public override string ToString()
    {
        return this.Type + " " + this.Lexema + " " + this.Literal;
    }
}