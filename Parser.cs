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
        return Unary();
    }

    private Expr Unary(){
        switch(Peek().Type){
            case TokenType.MINUS:
            case TokenType.BANG:
                TokenType operation = Consume().Type;
                return new UnaryExpr(operation,Expression());
            default:
                return Literal();
        }
    }

    private Expr Literal(){
        switch(Peek().Type){
            case TokenType.STRING:
            case TokenType.NUMBER:
                return new LiteralExpr(Peek().Literal);
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
    private Token Consume(){
        ++current;
        return tokens[current - 1];
    }
}