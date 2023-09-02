/*
Interprets hulks expressions.
*/

namespace Hulk;

class Interpreter : Visitor<object>{
    public Interpreter(){}

    //A global excecution environment.
    private Environment environment = new Environment();

    public string Interpret(Expr expr){
        try{
            return Stringify(Evaluate(expr));
        }
        catch(InterpreterException e){
            e.HandleException();
            //Unreachable code
            return null;
        }
    }

    private string Stringify(object value){
        if(value == null)return "NULL";
        return value.ToString();
    }

    private object Evaluate(Expr expr){
        return expr.Accept(this);
    }
    /*
    Evaluates a literal expression.
    */
    public object VisitLiteralExpr(LiteralExpr expr){
        return expr.Value;
    }
    /*
    Evaluates an unary expression.
    */
    public object VisitUnaryExpr(UnaryExpr expr){
        object value = Evaluate(expr.Expression);
        switch(expr.Operation.Type){
            case TokenType.BANG:
                CheckBoolOperand(expr.Operation,value);
                return !IsTrue(value);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operation,value);
                return -(float)value;
        }

        //Unreachable
        return null;
    }
    /*
    Evaluates a binary expression.
    */
    public object VisitBinaryExpr(BinaryExpr expr){
        //IMPORTANT: Evaluating expresions is done with a post-order traversal of the ast. First evaluates the left child,
        //then the right child, then the current node.This is importan in case any of the childs change the state.
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);
        switch(expr.Operation.Type){
            case TokenType.PLUS:// +
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left + (float)right;

            case TokenType.MINUS:// -
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left - (float)right;

            case TokenType.AT:// @
                CheckStringOperand(expr.Operation,left,-1);
                CheckStringOperand(expr.Operation,right,1);
                return (string)left + (string)right;

            case TokenType.STAR:// *
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left * (float)right;

            case TokenType.PERCENT:// %
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left % (float)right;
            
            case TokenType.SLASH:// /
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left / (float)right;

            case TokenType.CARET:// ^
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float) Math.Pow((float)left,(float)right);//Cast the pow result to keep types consistent
            
            case TokenType.LESS:// <
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left < (float)right;

            case TokenType.LESS_EQUAL:// <=
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left <= (float)right;

            case TokenType.GREATER:// >
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left > (float)right;
            
            case TokenType.GREATER_EQUAL:// >=
                CheckNumberOperand(expr.Operation,left,-1);
                CheckNumberOperand(expr.Operation,right,1);
                return (float)left >= (float)right;

            case TokenType.EQUAL_EQUAL:// ==
                return IsEqual(left,right);
            
            case TokenType.BANG_EQUAL:// !=
                return !IsEqual(left,right);

            case TokenType.AMPERSAND:// &
                CheckBoolOperand(expr.Operation,left,-1);
                CheckBoolOperand(expr.Operation,right,1);
                return (bool)left && (bool)right;

            case TokenType.PIPE:// |
                CheckBoolOperand(expr.Operation,left,-1);
                CheckBoolOperand(expr.Operation,right,1);
                return (bool)left || (bool)right;
        }
        return null;
    }
    /*
    Evaluates a conditional expression. if-else
    */
    public object VisitConditionalExpr(ConditionalExpr expr){
        object condition = Evaluate(expr.Condition);
        if(condition is bool){
            //Only a branch of the if-else expression will be interpreted.
            if((bool)condition)return Evaluate(expr.IfBranchExpr);
            return Evaluate(expr.ElseBranchExpr);
        }
        throw new InterpreterException("Boolean expected but the 'if' condition is of type " + GetType(condition) + " and evaluates to '" + condition.ToString()+"'",expr.IfOffset + 1);
    }
    /*
    Evaluates a let-in expression.
    */
    public object VisitLetInExpr(LetInExpr expr){
        //Compute the assigments.
        foreach(AssignmentExpr asignment in expr.Assignments){
            environment.Set(asignment.Identifier,Evaluate(asignment.RValue));
        }

        //Evaluate the expression.
        object value = Evaluate(expr.InBranchExpr);

        //Uncompute the assignments.
        foreach(AssignmentExpr asignment in expr.Assignments){
            environment.Remove(asignment.Identifier);
        }

        return value;
    }
    /*
    Evaluates an assigment.
    */
    public object VisitAssignmentExpr(AssignmentExpr expr){
        throw new NotImplementedException();
    }
    /*
    Evaluates a variable. Returns the value associated to the variable.
    */
    public object VisitVariableExpr(VariableExpr expr){
        return environment.Get(expr.Identifier);
    }
    //Are this objects equal in terms of value
    private bool IsEqual(object left,object right){
        return Equals(left,right);
    }

    //An expression is true if it is not the false keyword or null 
    private bool IsTrue(object value){
        if(value == null)return false;
        if(value is bool)return (bool)value;
        return true;
    }
    //Check if the object is of the expected type, throw if not
    //Pos is an optional value to indicate if it is a left operand or a right operand.
    // Pos = -1 , left
    // Pos = 1  , right
    // Pos = 0  , default
    private void CheckNumberOperand(Token operation, object value, int pos = 0){
        if(!(value is float)){
            throw new InterpreterException("Incorrect" + SetPosString(pos) + "operand type for "+operation.Lexeme+" operator. Number expected and "+ GetType(value)  +" was found.",operation.Offset);
        }
    }
    private void CheckStringOperand(Token operation, object value, int pos = 0){
        if(!(value is string)){
            throw new InterpreterException("Incorrect" + SetPosString(pos) + "operand type for "+operation.Lexeme+" operator. String expected and "+ GetType(value) +" was found.",operation.Offset);
        }
    }
    private void CheckBoolOperand(Token operation, object value, int pos = 0){
        if(!(value is bool)){
            throw new InterpreterException("Incorrect" + SetPosString(pos) + "operand type for "+operation.Lexeme+" operator. Boolean expected and "+ GetType(value) +" was found.",operation.Offset);
        }
    }
    private string SetPosString(int pos){
        if(pos == -1)return " left ";
        if(pos == 1)return " right ";
        return " ";
    }
    private string GetType(object value){
        if(value is float)return "Number";
        return value.GetType().Name;
    }
}