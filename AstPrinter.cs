/*
This class prints an abstract syntax tree in reverse polish notation
*/

namespace Hulk;

class AstPrinter : Visitor<string>{
    public AstPrinter(){}

    public string Print(Expr expr){
        return expr.Accept(this);
    }

    public string VisitConditionalExpr(ConditionalExpr expr){
        return "if-else (" + Print(expr.Condition) + " , " + Print(expr.IfBranchExpr) + " , " + Print(expr.ElseBranchExpr) + ")";
    }

    public string VisitBinaryExpr(BinaryExpr expr){
        
        return expr.Operation.Lexeme + "(" + Print(expr.Left) + " " + Print(expr.Right) + ")";
    }

    public string VisitUnaryExpr(UnaryExpr expr){
        return expr.Operation.Lexeme + " (" + Print(expr.Expression) + ")";
    }

    public string VisitLiteralExpr(LiteralExpr expr){
        return expr.Value.ToString();
    }
}