/*
The scanner receives a Hulk source code and transform it into a token sequence.
*/
namespace Hulk;

using static TokenType;

class Scanner{
    private string source;// Source code to be scanned
    private List<Token> tokens = new List<Token>();// Token sequence to be produced.
    private int current = 0;//Current character to be procesed.
    private int start = 0;//Start of the current token.

    private static readonly Dictionary<string,TokenType> keywords = new Dictionary<string, TokenType>()
    {
        {"else",ELSE},
        {"E",EULER},
        {"false",FALSE},
        {"function",FUNCTION},
        {"if",IF},
        {"in",IN},
        {"let",LET},
        {"PI",PI},
        {"true",TRUE}
    };

    public Scanner(string source){
        this.source = source;
    }

    //Scan the source code and output a token sequence
    public List<Token> Scan(){
        try{
            while(!IsAtEnd()){
                start = current;
                ScanToken();
            }
            tokens.Add(new Token(EOF,"",null));//Its elegant
            return this.tokens;
        }catch(ScannerException e){
            e.HandleException();
            //Unreachable code
            return null;
        }
    }

    //Scan next token
    private void ScanToken(){
        char c = Advance();
        switch(c){
            //One char tokens
            case '(': AddToken(LEFT_PAREN);break;
            
            case ')': AddToken(RIGHT_PAREN);break;
            
            case ',': AddToken(COMMA);break;
            
            case '-': AddToken(MINUS);break;
            
            case '+': AddToken(PLUS);break;
            
            case ';': AddToken(SEMICOLON);break;
            
            case '/': AddToken(SLASH);break;
            
            case '*': AddToken(STAR);break;
            
            case '^': AddToken(CARET);break;
            
            case '@': AddToken(AT);break;
            
            case '&': AddToken(AMPERSAND);break;
            
            case '|': AddToken(PIPE);break;
            
            case '!': AddToken(Match('=')?BANG_EQUAL:BANG);break;
            
            //There are three operators that start with an '='. These are '=' '==' '=>'
            case '=':
                if(Match('='))AddToken(EQUAL_EQUAL);
                else if(Match('>'))AddToken(ARROW);
                else AddToken(EQUAL);
                break;
            //Two operators start with a '<' . These are '<' '<='
            case '<': AddToken(Match('=')?LESS_EQUAL:LESS);break;

            //Two operators start with a '>' . These are '>' '>='
            case '>': AddToken(Match('=')?GREATER_EQUAL:GREATER);break;
            
            //There is no colon operator in Hulk, but it is part of the destructive assignment operator ':='
            case ':': 
                if(Match('='))AddToken(ASSIGN);
                else throw new ScannerException("Invalid token ':' . Perhaps you mean ':=' .",source,start);
                break;

            //Strings literals
            case '"':
                ScanString();
                break;

            //Ignore whitespaces
            case ' '://Whitespace
            case '\t'://Tab
            case '\n'://Newline
                break;

            default:
                //Number literal
                if(IsDigit(c)){
                    ScanNumber();
                }
                //Identifier, can be a variable name, a function name or a keyword
                else if(IsAlpha(c)){
                    ScanIdentifier();
                }
                else{
                    throw new ScannerException("Invalid character.",source,current - 1);
                }
                break;
        }
    }
    //Scan an identifier. Can be a variable name, a function name or a keyword.
    private void ScanIdentifier(){
        while(IsAlphaNumeric(Peek()))Advance();//Consume every posible character (Maximal Munch principle)

        string lexema = source.Substring(start,current - start);
        TokenType type;
        try{
            //Is a keyword identifier
            type = keywords[lexema];
        }catch(KeyNotFoundException){
            type = IDENTIFIER;
        }
        AddToken(type);
    }
    //Scan a number literal
    private void ScanNumber(){
        while(IsDigit(Peek()))Advance();//Consume the leading digits
        
        //If there exist a dot an at least a digit after the dot its a real number.
        if(Peek() == '.' && IsDigit(PeekNext())){
            Advance();//Consume the '.'
            while(IsDigit(Peek()))Advance();//Consume the trailing digits
        }

        AddToken(NUMBER,float.Parse(source.Substring(start,current - start)));
    }
    //Scan a string literal, the previous character was a quote '"'
    //Consume characters until it hits another quote. If the end is reached then a quote is missing.
    private void ScanString(){
        //Stop at end or if a quote is found
        while(!IsAtEnd() && Peek() != '"'){
            char actual = Advance();//Last consumed character

            //If the actual character is '\' and next is '"' then the quote must not be interpreted
            //as an enclosing quote but as an escaped quote.
            if(actual == '\\' && !IsAtEnd() && Peek() == '"'){
                Advance();
                //A call to advance will consume the quote and thus it will not be seen by the while condition
                //After that it can be escaped below.
            }
        }

        //A quote is missing
        if(IsAtEnd()){
            throw new ScannerException("A quote is missing.",source,start);
        }

        //Consume the closing quote
        Advance();

        //The string content without the enclosing quotes
        string value = source.Substring(start + 1,current - start - 2);
        
        //Escape characters if any
        value = value.Replace("\\t","\t");//Transform the pattern \t into a tab character
        value = value.Replace("\\n","\n");//Transform the pattern \n into a newline character
        value = value.Replace("\\\"","\"");//Transform the \" pattern into a quote character

        AddToken(STRING,value);
    }

    //Add a token to the list
    private void AddToken(TokenType type){
        AddToken(type,null);
    }
    private void AddToken(TokenType type,object? literal){
        string lexema = source.Substring(start,current - start);
        tokens.Add(new Token(type,lexema,literal));
    }

    //Hit the end of the source code
    private bool IsAtEnd(){
        return current >= source.Length;
    }

    //Returns the character at current position and move current one position ahead. This is called consume the character.
    private char Advance(){
        ++current;
        return source[current - 1];
    }
    //Return the character at current position but do not move current. 
    private char Peek(){
        if(IsAtEnd())return '\0';//Guarantee that a call to this method does not throw an exception. 
        return source[current];
    }
    //Return the character one position ahead of current but do not move current. This is called lookahead.
    private char PeekNext(){
        if(current + 1 >= source.Length)return '\0';//Guarantee that a call to this method does not throw an exception.
        return source[current + 1];
    }
    //Conditional advance, si el caracter actual coincide con el dado retorna verdadero y consume el caracter actual,
    //si no retorna falso y se mantiene el caracter actual
    private bool Match(char expected){
        if(IsAtEnd() || source[current] != expected)return false;

        ++current;
        return true;
    }
    //Return true if the character is a decimal digit [0,9], otherwhise return false.
    private bool IsDigit(char c){
        if('0' <= c && c <= '9')return true;
        return false;
    }
    //Return true if the character is a letter or an underscore '_'
    //
    private bool IsAlpha(char c){
        return ('a' <= c && c <= 'z') ||
               ('A' <= c && c <= 'Z') ||
               (c == '_');
    }
    //Return true if the character is a letter, a digit or an underscore.
    private bool IsAlphaNumeric(char c){
        return IsDigit(c) || IsAlpha(c);
    }
}