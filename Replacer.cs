/*
Replaces all the ocurrences of an identifier for another in a AST.

This helps addressing the following situation:
I declare a function 
    function thisIsFun(sum) => "ja ja ja";
This is not an error because there is no function named sum, yet.
I can call it
    thisIsFun(2);
But if i declare sum to be a function
    function sum(a,b) => a + b;
And i call thisIsFun again
    thisIsFun(2);
This is an error because the argument sum also collide with a function name.
Thus its a technique to guarantee that : No variable can be named as a function.
*/
namespace Hulk;
class Replacer : Visitor<Expr>{
    private  Token viejo;
    private Token nuevo;
    public Expr Replace(Expr expr,Token _viejo,Token _nuevo){
        viejo = _viejo;
        nuevo = _nuevo;
        return Replace(expr);
    }
    private Expr Replace(Expr expr){
        return expr.Accept(this);
    }
    //There is nothing to replace in here.
    public Expr VisitLiteralExpr(LiteralExpr expr){
        return expr;
    }
    //The replacement can occur in the right operand.
    public Expr VisitUnaryExpr(UnaryExpr expr){
        //Ignore the operation
        Expr expression = Replace(expr);
        return new UnaryExpr(expr.Operation,expression);
    }
    //The replacement can occur in any of the operands.
    public Expr VisitBinaryExpr(BinaryExpr expr){
        Expr left = Replace(expr.Left);
        //Ignore the operator
        Expr right = Replace(expr.Right);
        return new BinaryExpr(left,expr.Operation,right);
    }
    //The replacement can occur in the condition and any of the branches.
    public Expr VisitConditionalExpr(ConditionalExpr expr){
        Expr condition = Replace(expr.Condition);
        Expr ifBranch = Replace(expr.IfBranchExpr);
        Expr elseBranch = Replace(expr.ElseBranchExpr);
        return new ConditionalExpr(condition,ifBranch,elseBranch,expr.IfOffset,expr.ElseOffset);
    }
    //The replacement can occur in any of the assigments or the 'in' expression.
    public Expr VisitLetInExpr(LetInExpr expr){
        List<AssignmentExpr> assignments = new List<AssignmentExpr>();
        foreach(AssignmentExpr asign in expr.Assignments)assignments.Add((AssignmentExpr) Replace(asign));
        Expr inBranch = Replace(expr.InBranchExpr);
        return new LetInExpr(assignments,inBranch);
    }
    //The variable to be assigned can be replaced and can occur in the rValue expression.
    public Expr VisitAssignmentExpr(AssignmentExpr expr){
        Token identifier = Match(expr.Identifier);
        Expr rValue = Replace(expr.RValue);
        return new AssignmentExpr(identifier,rValue);
    }
    //This is the main target of the replacement
    public Expr VisitVariableExpr(VariableExpr expr){
        Token identifier = Match(expr.Identifier);
        return new VariableExpr(identifier);
    }
    //There cant be a function declaration inside a function declaration, thus this will never be called.
    public Expr VisitFunctionExpr(FunctionExpr expr){
        Token identifier = Match(expr.Identifier);
        Expr body = Replace(expr.Body);
        List<Token> arguments = new List<Token>();
        foreach(Token arg in expr.Args)arguments.Add(Match(arg));
        return new FunctionExpr(identifier,arguments,body,expr.Overwritable);
    }
    //The replacement can occur in the parameters because the identifier of the called function can not be the same as a parameter.
    public Expr VisitCallExpr(CallExpr expr){
        Token identifier = Match(expr.Identifier);
        List<Expr> parameters = new List<Expr>();
        foreach(Expr paramExpr in expr.Parameters)parameters.Add(Replace(paramExpr));
        return new CallExpr(identifier,parameters);
    }
    //If this token is the one to be replaced return a copy of the replacement but keeping the offset, if not return itself.
    //Offset of viejo and nuevo are the same. But the offset of viejo and actual can be different.
    //Consider : function abs(x) => if(x < 0) -x else x;
    //There are 4 'x' thus four different offsets.
    private Token Match(Token actual){
        if(actual.Lexeme == viejo.Lexeme)return new Token(nuevo.Type,nuevo.Lexeme,nuevo.Literal,actual.Offset);
        return actual;
    }
}