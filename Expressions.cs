/*
This classes represents the nodes of the abstract syntax tree.
The tree is produced by the parser and then consumed by the interpreter.
*/

namespace Hulk;

abstract class Expr{
    public abstract R Accept<R> (Visitor<R> visitor);
}

interface Visitor<R>{
     R VisitBinaryExpr(BinaryExpr expr);
     R VisitLiteralExpr(LiteralExpr expr);
     R VisitUnaryExpr(UnaryExpr expr);
}

//Represents string and number literals
class LiteralExpr : Expr{
    public object Value {get; private set;}

    public LiteralExpr(object value){
        this.Value = value;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}

//Represents unary operators like (! -)
class UnaryExpr : Expr{
    public Token Operation{get; private set;}
    public Expr Expression{get; private set;}

    public UnaryExpr(Token operation,Expr expr){
        Operation = operation;
        Expression = expr;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}
//Represents Binary Operators like + - * / ^ @
class BinaryExpr : Expr{
    public Expr Left{get; private set;}
    public Expr Right{get; private set;}

    public Token Operation{get; private set;}

    public BinaryExpr(Expr left,Token operation,Expr right){
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}