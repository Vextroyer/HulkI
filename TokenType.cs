/*
Tipos de tokens en Hulk
*/

namespace Hulk;

enum TokenType
{
    //Tokens de un solo caracter
    LEFT_PAREN,RIGHT_PAREN,COMMA,MINUS,PLUS,SEMICOLON,SLASH,STAR,CARET,AT,AMPERSAND,PIPE,
    //Tokens de uno o dos caracteres
    BANG, BANG_EQUAL,
    EQUAL,EQUAL_EQUAL,
    LESS,LESS_EQUAL,
    GREATER,GREATER_EQUAL,
    ASSIGN,ARROW,
    //Literals
    IDENTIFIER,STRING,NUMBER,
    //Keywords
    ELSE,FALSE,FUNCTION,IF,PRINT,TRUE,LET,IN,PI,EULER,
    EOF
}

// ( ) , - + ; / * ^ @ & |
// ! !=
// = ==
// < <=
// > >=
// := =>