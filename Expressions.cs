/*
This classes represents the nodes of the abstract syntax tree.
The tree is produced by the parser and then consumed by the interpreter.
*/

namespace Hulk;

abstract class Expr{
    abstract public string Print();//For debbugging purposes
}

//Represents string and number literals
class LiteralExpr : Expr{
    public object Value {get; private set;}

    public LiteralExpr(object value){
        this.Value = value;
    }

    override public string Print(){
        return AstPrinter.Accept(this);
    }
}

//Represents unary operators like (! -)
class UnaryExpr : Expr{
    public TokenType Operation{get; private set;}
    public Expr Expression{get; private set;}

    public UnaryExpr(TokenType operation,Expr expr){
        Operation = operation;
        Expression = expr;
    }

    override public string Print(){
        return AstPrinter.Accept(this);
    }
}