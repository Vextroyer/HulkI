/*
This class prints an abstract syntax tree in reverse polish notation
*/

namespace Hulk;

static class AstPrinter{
    // public static string Print(Expr expr){
    //     if(expr is LiteralExpr)return PrintLiteralExpr((LiteralExpr) expr);
    //     return "NULL";
    // }

    // private static string PrintLiteralExpr(LiteralExpr expr){
    //     return expr.Value.ToString();
    // }

    public static string Print(Expr expr){
        return "("+expr.Print()+")";
    }

    public static string Accept(UnaryExpr expr){
        string operation;
        if(expr.Operation == TokenType.MINUS)operation = "-";
        else operation = "!";

        return operation + " " + Print(expr.Expression);
    }

    public static string Accept(LiteralExpr expr){
        return expr.Value.ToString();
    }
}