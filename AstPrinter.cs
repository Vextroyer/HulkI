/*
This class prints an abstract syntax tree in reverse polish notation
*/

namespace Hulk;

class AstPrinter : Visitor<string>{
    public AstPrinter(){}

    public string Print(Expr expr){
        return expr.Accept(this);
    }

    public string VisitBinaryExpr(BinaryExpr expr){
        
        return expr.Operation.Lexema + "(" + Print(expr.Left) + " " + Print(expr.Right) + ")";
    }

    public string VisitUnaryExpr(UnaryExpr expr){
        return expr.Operation.Lexema + " (" + Print(expr.Expression) + ")";
    }

    public string VisitLiteralExpr(LiteralExpr expr){
        return expr.Value.ToString();
    }
}