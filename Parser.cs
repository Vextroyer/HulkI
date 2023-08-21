/*
Parse a token list and produce an abstract syntax tree
*/

namespace Hulk;

class Parser{
    List<Token> tokens;
    int current = 0;//next token to be procesed

    public Parser(List<Token> tokens){
        this.tokens = tokens;
    }

    //Parse the content provided
    public Expr Parse(){
        return Expression();
    }
 
    //Here are the recursive descending methods for interpreting the expressions.

    private Expr Expression(){
        //Fall to the next
        return Term();
    }

    //Sumas, restas y concatenaciones de cadenas
    private Expr Term(){
        Expr expr = Factor();
        while(Match(TokenType.MINUS,TokenType.PLUS,TokenType.AT)){
            Token operation = Previous();
            Expr right = Factor();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Productos y cocientes
    private Expr Factor(){
        Expr expr = Power();
        while(Match(TokenType.SLASH,TokenType.STAR)){
            Token operation = Previous();
            Expr right = Power();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Potencias (Rigth to left associative)
    private Expr Power(){
        Expr expr = Unary();
        while(Match(TokenType.CARET)){
            Token operation = Previous();
            Expr right = Power();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    private Expr Unary(){
        switch(Peek().Type){
            case TokenType.MINUS:
            case TokenType.BANG:
                Token operation = Advance();
                return new UnaryExpr(operation,Grouping());
            default:
                return Literal();
        }
    }

    //Evaluate parenthesis
    private Expr Grouping(){
        return Literal();
    }

    private Expr Literal(){
        switch(Peek().Type){
            case TokenType.STRING:
            case TokenType.NUMBER:
                return new LiteralExpr(Advance().Literal);
            default:
                return Unrecognized();
        }
    }

    //Helper method that launch an exception
    private Expr Unrecognized(){
        throw new Exception("Unrecognized Expresion.");
    }

    //Return the next token to be procesed without advancing current
    private Token Peek(){
        return tokens[current];
    }
    //Return the current token and advance current
    private Token Advance(){
        ++current;
        return tokens[current - 1];
    }
    //Retorna verdadero si el token actual coincide con cualquiera de los tokens proporcionados y lo consume. De ser falso no lo consume.
    private bool Match(params TokenType[] types){
        foreach(TokenType type in types){
            if(type == Peek().Type){
                Advance();
                return true;
            }
        }
        return false;
    }
    //Retorna el token anterior
    private Token Previous(){
        return tokens[current - 1];
    }
}