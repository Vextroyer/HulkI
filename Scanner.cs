/*
El Scanner recibe un codigo fuente escrito en Hulk y devuelve una secuencia de
tokens.
*/
namespace Hulk;

using static TokenType;

class Scanner{
    private string source;// Codigo fuente proveido al scanner
    private List<Token> tokens = new List<Token>();// Tokens que conforman el codigo fuente
    private int current = 0;//Caracter a ser procesado
    private int start = 0;//Start of the current token

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

    //Escanea el codigo fuente y produce una lista de tokens
    public List<Token> Scan(){
        try{
            while(!IsAtEnd()){
                start = current;
                ScanToken();
            }
            tokens.Add(new Token(EOF,"",null));
            return this.tokens;
        }catch(ScannerException e){
            Hulk.ScannerError(e);
            throw new HandledException();
        }
    }

    //Escanea el siguiente token
    private void ScanToken(){
        char c = Advance();
        switch(c){
            
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
            
            //Hay 3 operadores que comienzan con = : = , == , =>
            case '=':
                if(Match('='))AddToken(EQUAL_EQUAL);
                else if(Match('>'))AddToken(ARROW);
                else AddToken(EQUAL);
                break;

            case '<': AddToken(Match('=')?LESS_EQUAL:LESS);break;
            
            case '>': AddToken(Match('=')?GREATER_EQUAL:GREATER);break;
            
            case ':': 
                if(Match('='))AddToken(ASSIGN);
                else throw new ScannerException("Invalid token ':' . Perhaps you mean ':=' .",source,start);//No existe un token en Hulk compuesto por solo :
                break;

            //Strings literals
            case '"':
                ScanString();
                break;

            //Ignore whitespaces
            case ' ':
            case '\t':
            case '\n':
                break;

            default:
                //Number literal
                if(IsDigit(c)){
                    ScanNumber();
                }
                //Identifier, nombre de variable o funcion
                else if(IsAlpha(c)){
                    ScanIdentifier();
                }
                else{
                    throw new ScannerException("Invalid character.",source,current - 1);
                }
                break;
        }
    }
    //Escanea un identificador, que puede ser un nombre de variable, funcion o keyword
    private void ScanIdentifier(){
        while(IsAlphaNumeric(Peek()))Advance();//Consume todos los caracteres posibles (Maximal Munch)

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
    //Escanea un literal numerico
    private void ScanNumber(){
        while(IsDigit(Peek()))Advance();//Consume los digitos
        
        //Si hay un punto y caracteres decimales despues,este es un numero real
        if(Peek() == '.' && IsDigit(PeekNext())){
            Advance();//Consume el '.'
            while(IsDigit(Peek()))Advance();//Consume los digitos
        }

        AddToken(NUMBER,float.Parse(source.Substring(start,current - start)));
    }
    //Scan a strng literal, el ultimo caracter procesado fue un "
    //Lee caracteres hasta que encuentre el siguiente "
    //Si llega al final sin encontrarlo es un error
    private void ScanString(){
        //Este while para si se llego al final o se encontraron las comillas dobles
        while(!IsAtEnd() && Peek() != '"'){
            char actual = Advance();

            //If the actual character is \ and next is " then th quote must not be interpreted
            //as an enclosing quote but as an escaped quote.
            if(actual == '\\' && !IsAtEnd() && Peek() == '"'){
                Advance();
                //A call to advance will consume the quote and thus it will not be seen by the while condition
                //After that it can be escaped below.
            }
        }

        //No se cerraron las comillas dobles
        if(IsAtEnd()){
            throw new ScannerException("Closing quote is missing.",source,start);
        }

        //Consume las comillas de cierre
        Advance();

        //El valor del string incluye todo el string excepto las comillas dobles de apertura y cierre
        string value = source.Substring(start + 1,current - start - 2);
        
        //Escape characters if any
        value = value.Replace("\\t","\t");
        value = value.Replace("\\n","\n");
        value = value.Replace("\\\"","\"");

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

    //Quedan caracteres por procesar, llego al final del codigo fuente
    private bool IsAtEnd(){
        return current >= source.Length;
    }

    //Consume el caracter actual y mueve current al siguiente
    private char Advance(){
        ++current;
        return source[current - 1];
    }
    //Devuelve el caracter actual sin consumirlo, no mueve current
    private char Peek(){
        if(IsAtEnd())return '\0';//Garantiza que una llamada a Peek no lance una excepcion
        return source[current];
    }
    //Devuelve el caracter siguiente sin consumirlo, no mueve current
    private char PeekNext(){
        if(current + 1 >= source.Length)return '\0';//Garantiza que una llamada a Peek no lance una excepcion
        return source[current + 1];
    }
    //Conditional advance, si el caracter actual coincide con el dado retorna verdadero y consume el caracter actual,
    //si no retorna falso y se mantiene el caracter actual
    private bool Match(char expected){
        if(IsAtEnd() || source[current] != expected)return false;

        ++current;
        return true;
    }
    //Devuelve verdadero si el caracter es un digito entre [0,9]
    private bool IsDigit(char c){
        if('0' <= c && c <= '9')return true;
        return false;
    }
    //Devuelve verdadero si el caracter es una letra o un underscore _
    private bool IsAlpha(char c){
        return ('a' <= c && c <= 'z') ||
               ('A' <= c && c <= 'Z') ||
               (c == '_');
    }
    //Devuelve verdadero si es letra o digito
    private bool IsAlphaNumeric(char c){
        return IsDigit(c) || IsAlpha(c);
    }
}