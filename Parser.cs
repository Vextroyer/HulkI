/*
Parse a token list and produce an abstract syntax tree (AST).
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
            Expr expr = Expression();
            //Expressions must end in a ;
            if(Match(TokenType.SEMICOLON)){
                if(Match(TokenType.EOF))return expr;
                else throw new ParserException("Found tokens after ';'. Multiple expressions per line not allowed.");
            }
            else{
                //If doesnt exist a ';'
                if(!HasSemicolon())throw new ParserException("Expression must end in a ';'");
                
                //If exist a ';' then there are tokens before the ';' that where not parsed.

                //If the current token is a closing paren then the opening paren is missing.
                if(Match(TokenType.RIGHT_PAREN))throw new ParserException("Missing opening paren.");
                throw new ParserException("Malformed expresion.");
            }
        }
        catch(ParserException e){
            e.HandleException();
            //Unreachable code
            return null;
        }
        
    }
 
    //Here are the recursive descending methods for interpreting the expressions.

    private Expr Expression(){
        //Fall to the next
        return Or();
    }
    //Logical or '|'
    private Expr Or(){
        Expr expr = And();
        while(Match(TokenType.PIPE)){
            Token operation = Previous();
            Expr right = And();
            return new BinaryExpr(expr,operation,right);
        }
        return expr;
    }
    //Logical and '&'
    private Expr And(){
        Expr expr = Equality();
        while(Match(TokenType.AMPERSAND)){
            Token operation = Previous();
            Expr right = Equality();
            return new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Equality operators '==' '!='
    private Expr Equality(){
        Expr expr = Comparison();
        if(Match(TokenType.EQUAL_EQUAL,TokenType.BANG_EQUAL)){
            Token operation = Previous();
            Expr right = Comparison();
            return new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Comparison operators '<' '<=' '>' '>='
    private Expr Comparison(){
        Expr expr = Term();
        if(Match(TokenType.LESS,TokenType.LESS_EQUAL,TokenType.GREATER,TokenType.GREATER_EQUAL)){
            Token operation = Previous();
            Expr right = Term();
            return new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Substractions, sums and concatenations '-' '+' '@'
    private Expr Term(){
        Expr expr = Factor();
        while(Match(TokenType.MINUS,TokenType.PLUS,TokenType.AT)){
            Token operation = Previous();
            Expr right = Factor();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Divisions and multiplications '/' '*'
    private Expr Factor(){
        Expr expr = Power();
        while(Match(TokenType.SLASH,TokenType.STAR)){
            Token operation = Previous();
            Expr right = Power();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Exponentiation '^' (Rigth to left associative)
    private Expr Power(){
        Expr expr = Unary();
        while(Match(TokenType.CARET)){
            Token operation = Previous();
            Expr right = Power();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }
    //Unary operators '!' '-'
    private Expr Unary(){
        switch(Peek().Type){
            case TokenType.MINUS:
            case TokenType.BANG:
                Token operation = Advance();
                return new UnaryExpr(operation,Unary());
            default:
                return Grouping();
        }
    }

    //Evaluate parenthesis
    private Expr Grouping(){
        if(Match(TokenType.LEFT_PAREN)){
            Expr expr = Expression();
            if(!Match(TokenType.RIGHT_PAREN))throw new ParserException("Missing close paren.");
            return expr;
        }
        return Literal();
    }
    //Numbers, string literals, true, false, Euler constant and Pi constant
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
    //If the current token match any of the given tokens advance an return true, if not return false and keep current.
    //It is a conditional advance.
    private bool Match(params TokenType[] types){
        foreach(TokenType type in types){
            if(type == Peek().Type){
                Advance();
                return true;
            }
        }
        return false;
    }
    //Return the previous token.
    private Token Previous(){
        return tokens[current - 1];
    }

    //Returns true if there exist a semicolon on the remaining tokens.
    private bool HasSemicolon(){
        for(int i=current;i<tokens.Count;++i){
            if(tokens[i].Type == TokenType.SEMICOLON)return true;
        }
        return false;
    }
}