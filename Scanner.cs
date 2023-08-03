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

    public Scanner(string source){
        this.source = source;
    }

    //Escanea el codigo fuente y produce una lista de tokens
    public List<Token> Scan(){
        while(!IsAtEnd()){
            start = current;
            ScanToken();
        }
        tokens.Add(new Token(EOF,"",null));
        return this.tokens;
    }

    //escanea el siguiente token
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
            
            //case 'E': 
            //pi

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
                else throw new ScannerException("Invalid token ':' . Perhaps you mean ':=' .",source,start);
                //No existe un token en Hulk compuesto por solo :
                break;

            //Strings literals
            case '"':
                ScanString();
                break;
        }
    }

    //Scan a strng literal, el ultimo caracter procesado fue un "
    //Lee caracteres hasta que encuentre el siguiente "
    //Si llega al final sin encontrarlo es un error
    private void ScanString(){
        //Este while para si se llego al final o se encontraron las comillas dobles
        while(!IsAtEnd() && Peek() != '"'){
            Advance();
        }

        //No se cerraron las comillas dobles
        if(IsAtEnd()){
            throw new ScannerException("Closing quote is missing.",source,start);
        }

        //Consume las comillas de cierre
        Advance();

        //El valor del string incluye todo el string excepto las comillas dobles de apertura y cierre
        string value = source.Substring(start + 1,current - start - 2);
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
        return source[current];
    }
    //Conditional advance, si el caracter actual coincide con el dado retorna verdadero y consume el caracter actual,
    //si no retorna falso y se mantiene el caracter actual
    private bool Match(char expected){
        if(IsAtEnd() || source[current] != expected)return false;

        ++current;
        return true;
    }
}