/*
Represents a token. 
*/

namespace Hulk;

class Token{
    //Initial position of the token in the line
    public int Offset {get; private set;}

    public TokenType Type {get; private set;}

    //Portion of text on the source code from wich this token was created.
    //For example, a PLUS token is generated always from a '+' in the source code
    //A number can be generated from '15.29' in the source code
    //A string from a '"This is a string literal"' in the source code.
    public string Lexeme {get; private set;}

    //Value, for example if the token is of type NUMBER the value can be 17
    public object? Literal {get; private set;}

    public Token(TokenType type,string lexema,object? literal,int offset){
        this.Type = type;
        this.Lexeme = lexema;
        this.Literal = literal;
        this.Offset = offset;
    }

    public override string ToString()
    {
        return this.Type + " " + this.Lexeme + " " + this.Literal;
    }
}