/*
Types of tokens in Hulk
*/

namespace Hulk;

enum TokenType
{
    //One char tokens
    LEFT_PAREN,RIGHT_PAREN,COMMA,MINUS,PLUS,SEMICOLON,SLASH,PERCENT,STAR,CARET,AT,AMPERSAND,PIPE,
    //One or two char tokens
    BANG, BANG_EQUAL,
    EQUAL,EQUAL_EQUAL,
    LESS,LESS_EQUAL,
    GREATER,GREATER_EQUAL,
    ARROW,
    //Literals
    IDENTIFIER,STRING,NUMBER,
    //Keywords
    ELSE,FALSE,FUNCTION,IF,PRINT,TRUE,LET,IN,PI,EULER,
    EOF
}

// ( ) , - + ; / % * ^ @ & |
// ! !=
// = ==
// < <=
// > >=
// =>