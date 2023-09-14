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
            Expr expr = HLExpression();
            //Expressions must end in a ;
            if(Match(TokenType.SEMICOLON)){
                if(Match(TokenType.EOF))return expr;
                else throw new ParserException("Found tokens after ';'. Multiple expressions per line not allowed.",GetOffset());
            }
            else{
                //If doesnt exist a ';'
                if(!HasSemicolon())throw new ParserException("Expression must end in a ';'");
                
                //If exist a ';' then there are tokens before the ';' that where not parsed.

                //If the current token is a closing paren then the opening paren is missing.
                if(Match(TokenType.RIGHT_PAREN))throw new ParserException("There is no '(' for this parenthesis.",GetOffset() - 1);
                if(Match(TokenType.EQUAL))throw new ParserException("Assignments can only be used on a 'let in' expression.",Previous().Offset);
                throw new ParserException("Malformed expresion.",GetOffset());
            }
        }
        catch(ParserException e){
            e.HandleException();
            //Unreachable code
            return null;
        }
        
    }
 
    //Here are the recursive descending methods for interpreting the expressions.

    //Divides expressions and function declarations.
    private Expr HLExpression(){
        if(Match(TokenType.FUNCTION)){
            if(!Match(TokenType.IDENTIFIER))throw new ParserException("Identifier expected but found '" + Peek().Lexeme + "'.",GetOffset());
            Token identifier = Previous();
            if(!Match(TokenType.LEFT_PAREN))throw new ParserException("Expected () after function name.",identifier.Offset + identifier.Lexeme.Length);
            List<Token> args = Arguments();
            if(!Match(TokenType.RIGHT_PAREN)){
                if(Peek().Type == TokenType.IDENTIFIER)throw new ParserException("')' expected but identifier found, are you missing a ','",GetOffset());
                if(Peek().Type == TokenType.EQUAL)throw new ParserException("Assigments not allowed as function parameters, just variable names.",GetOffset());
                throw new ParserException("Expected ')' but '" + Peek().Lexeme + "' found.",GetOffset());
            }
            if(!Match(TokenType.ARROW))throw new ParserException("Expected '=>' after function signature.",Previous().Offset + 1);
            Expr body = Expression();

            return new FunctionExpr(identifier,args,body);
        }
        return Expression();
    }
    //Returns the arguments of the function
    private List<Token> Arguments(){
        List<Token> args = new List<Token>();
        
        //A closing paren at this point means no args.
        if(Peek().Type == TokenType.RIGHT_PAREN)return args;

        do{
            if(!Match(TokenType.IDENTIFIER))throw new ParserException("Identifier expected.",GetOffset());
            args.Add(Previous());//Add the identifier as an argument.
        }while(Match(TokenType.COMMA));
        return args;
    }

    private Expr Expression(){
        //Fall to the next
        return Declaration();
    }

    //A let-in expression
    private Expr Declaration(){
        if(Match(TokenType.LET)){
            //Assigments part
            List<AssignmentExpr> assignments = Assignment();
            
            //'In' part
            if(!Match(TokenType.IN)){
                throw new ParserException($"Expected 'in' but '{Peek().Lexeme}' was found. Missing a ',' ? Had an error typing 'in' ?",GetOffset());
                // if(Peek().Type == TokenType.IDENTIFIER)throw new ParserException("Identifier found. Are you missing a ',' ?",GetOffset());
                // throw new ParserException("'in' keyword expected before expression.",GetOffset());
            }

            Expr inBranchExpr = Expression();

            return new LetInExpr(assignments,inBranchExpr);
        }
        return Conditional();
    }
    //An assignment expression. Can only occur in a let-in expression. Does not fall to other expressions.
    private List<AssignmentExpr> Assignment(){
        List<AssignmentExpr> assignments = new List<AssignmentExpr>();
        //Handle comma separated assignments. Empty declarations after a let keyword are an error.
        int round = -1;
        do{
            ++round;
            if(!Match(TokenType.IDENTIFIER)){
                if(Match(TokenType.PI,TokenType.EULER))throw new ParserException(Previous().Lexeme + " is a language constant and cannot be assigned a value.",Previous().Offset);
                if(Match(TokenType.NUMBER,TokenType.STRING,TokenType.TRUE,TokenType.FALSE))throw new ParserException("Cannot assign a value to a literal.",Previous().Offset);
                throw new ParserException("Assignment expected" + (round == 0 ? "." : " after ',' ."),Previous().Offset);
            }
            Token identifier = Previous();
            if(!Match(TokenType.EQUAL))throw new ParserException("Equal sign '=' expected after '" + Previous().Lexeme + "'." ,Previous().Offset + Previous().Lexeme.Length);
            Expr rvalue = Expression();
            assignments.Add(new AssignmentExpr(identifier,rvalue));
        }while(Match(TokenType.COMMA));

        return assignments;
    }

    // If - else expression
    private Expr Conditional(){
        int ifOffset = -1;
        int elseOffset = -1;
        if(Match(TokenType.IF)){
            //An if token has been consumed
            ifOffset = Previous().Offset;
            //There must be a parenthesized expression, so check for opening paren
            if(Peek().Type == TokenType.LEFT_PAREN){
                //If there is a closing paren right after the opening paren thats an error because there is no expression
                //to be evaluated.
                if(PeekNext().Type == TokenType.RIGHT_PAREN)throw new ParserException("Empty parenthesis not allowed after if. They must contain an expression.",GetOffset());
                
                //It is necesary to check if exist a paren before calling Grouping because
                //it also handles expressinons without parens and this could lead to confusing errors.
                Expr condition = Grouping();//This consumes both parens

                //An else right after the pares means an empty expression, thats not allowed
                if(Peek().Type == TokenType.ELSE)throw new ParserException("No expression found on the 'if' branch.",GetOffset());

                Expr ifBranchExpr = Expression();

                //After the if branch expression comes the else keyword
                if(!Match(TokenType.ELSE))throw new ParserException("Expected 'else' and '" + Peek().Lexeme + "' was found.",GetOffset());
                
                elseOffset = Previous().Offset;

                //An exception may arise if the else branch expression is empty
                Expr elseBranchExpr;
                try{
                    elseBranchExpr = Expression();
                }catch(ParserException){
                    throw new ParserException("No expression found on the 'else' branch.",elseOffset + 4);
                }
                

                return new ConditionalExpr(condition,ifBranchExpr,elseBranchExpr,ifOffset,elseOffset);
            }else{
                //No paren after an if is a syntactic error
                throw new ParserException("Expected '(' after if.",Previous().Offset + 2);
            }
        }
        //Fall to next
        return Or();
    }

    //Logical or '|'
    private Expr Or(){
        Expr expr = And();
        while(Match(TokenType.PIPE)){
            Token operation = Previous();
            Expr right = And();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }
    //Logical and '&'
    private Expr And(){
        Expr expr = Equality();
        while(Match(TokenType.AMPERSAND)){
            Token operation = Previous();
            Expr right = Equality();
            expr = new BinaryExpr(expr,operation,right);
        }
        return expr;
    }

    //Equality operators '==' '!='
    private Expr Equality(){
        Expr expr = Comparison();

        int counter = 0;//How many equalities are being used.
        
        //Multiple chained equalities are not supported by the grammar. So report an error if more than one is found.
        while(Match(TokenType.EQUAL_EQUAL,TokenType.BANG_EQUAL)){
            
            if(counter == 1)throw new ParserException($"Cannot use '{Previous().Lexeme}' after '{Previous().Lexeme}'. Consider using parenthesis and/or logical operators.",Previous().Offset);
            
            Token operation = Previous();
            Expr right = Comparison();
            expr = new BinaryExpr(expr,operation,right);
            
            ++counter;
        }
        return expr;
    }

    //Comparison operators '<' '<=' '>' '>='
    private Expr Comparison(){
        Expr expr = Term();

        int counter = 0;//How many comparisons are being used.

        //Multiple chained comparisons are not supported by the grammar. So report an error if more than one is found.
        while(Match(TokenType.LESS,TokenType.LESS_EQUAL,TokenType.GREATER,TokenType.GREATER_EQUAL)){
            
            if(counter == 1)throw new ParserException($"Cannot use '{Previous().Lexeme}' after '{Previous().Lexeme}'. Consider using parenthesis and/or logical operators.",Previous().Offset);

            Token operation = Previous();
            Expr right = Term();
            expr = new BinaryExpr(expr,operation,right);

            ++counter;
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

    //Divisions ,multiplications and modules '/' '*' '%'
    private Expr Factor(){
        Expr expr = Power();
        while(Match(TokenType.SLASH,TokenType.PERCENT,TokenType.STAR)){
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
        if(Match(TokenType.BANG,TokenType.MINUS)){
            Token operation = Previous();
            return new UnaryExpr(operation,Unary());
        }
        return Grouping();
    }

    //Evaluate parenthesis
    private Expr Grouping(){
        if(Match(TokenType.LEFT_PAREN)){
            Expr expr = Expression();
            if(!Match(TokenType.RIGHT_PAREN))throw new ParserException($"Expected ')' but '{Peek().Lexeme}' was found.",GetOffset());
            return expr;
        }
        return Call();
    }
    //Function call
    private Expr Call(){
        //If the next seems like a function call, its a function call.
        if(Peek().Type == TokenType.IDENTIFIER && PeekNext().Type == TokenType.LEFT_PAREN){
            Token identifier = Advance();//Consume the identifier
            Advance();//Consume the opening paren
            List<Expr> parameters = Parameters();
            if(!Match(TokenType.RIGHT_PAREN))throw new ParserException("')' expected but found '" + Peek().Lexeme + "' .",GetOffset());
            return new CallExpr(identifier,parameters);
        }
        return Literal();
    }
    //Return the parameters of the call. Similar to arguments.
    private List<Expr> Parameters(){
        List<Expr> parameters = new List<Expr>();
        
        //A closing paren at this point means no parameters.
        if(Peek().Type == TokenType.RIGHT_PAREN)return parameters;

        do{
            parameters.Add(Expression());
        }while(Match(TokenType.COMMA));
        return parameters;
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
            //Identifier
            case TokenType.IDENTIFIER:
                return new VariableExpr(Advance());
            default:
                return Unrecognized();
        }
    }

    //Helper method that launch an exception
    private Expr Unrecognized(){
        //If a function keyword is found here, its because its inside some other expression, wich its not allowed.
        if(Peek().Type == TokenType.FUNCTION)throw new ParserException("Function declaration not allowed as part of other expressions.",GetOffset());
        throw new ParserException("Unrecognized Expresion.",GetOffset());
    }

    //Return the next token to be procesed without advancing current
    private Token Peek(){
        return tokens[current];
    }
    //Return the token after current without advancing current
    private Token PeekNext(){
        return tokens[current + 1];
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
    //Get the initial position of the current token in the source code
    private int GetOffset(){
        return tokens[current].Offset;
    }
}