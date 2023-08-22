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
        try{
            return Expression();
        }
        catch(ParserException e){
            Hulk.ParserError(e);
            throw new HandledException();
        }
        
    }
 
    //Here are the recursive descending methods for interpreting the expressions.

    private Expr Expression(){
        //Fall to the next
        Expr expr = Term();
        //Expressions must end in a ;
        if(Match(TokenType.SEMICOLON)){
            if(Match(TokenType.EOF))return expr;
            else throw new ParserException("Found tokens after ';'. Multiple expressions per line not allowed.");
        }
        else{
            //Si no existe ;
            if(!HasSemicolon())throw new ParserException("Expression must end in a ';'");
            //Si existe entonces hay tokens que no fueron interpretados
            throw new ParserException("Malformed expresion.");
        }
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
                return Grouping();
        }
    }

    //Evaluate parenthesis
    private Expr Grouping(){
        /*if(Match(TokenType.LEFT_PAREN)){
            Expr expr = Expression();
            if(!Match(TokenType.RIGHT_PAREN))throw new Exception("Missing close paren");
            return expr;
        }
        //What if you found a closing paren ? Then it has no opening paren because in case of existing the above code would be executed
        if(Match(TokenType.RIGHT_PAREN)){
            throw new Exception("Missing open paren");
        }
        */
        return Literal();
    }

    private Expr Literal(){
        switch(Peek().Type){
            //String or number literal
            case TokenType.STRING:
            case TokenType.NUMBER:
                return new LiteralExpr(Advance().Literal);
            //Boolean literals
            case TokenType.TRUE:
                Advance();
                return new LiteralExpr(true);
            case TokenType.FALSE:
                Advance();
                return new LiteralExpr(false);
            //Pi
            case TokenType.PI:
                Advance();
                return new LiteralExpr((float)Math.PI);
            //Euler
            case TokenType.EULER:
                Advance();
                return new LiteralExpr((float)Math.E);
            default:
                return Unrecognized();
        }
    }

    //Helper method that launch an exception
    private Expr Unrecognized(){
        throw new ParserException("Unrecognized Expresion.");
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

    //Returns true if there exist a semicolon on the remaining tokens
    private bool HasSemicolon(){
        for(int i=current;i<tokens.Count;++i){
            if(tokens[i].Type == TokenType.SEMICOLON)return true;
        }
        return false;
    }
}