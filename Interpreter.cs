/*
Interprets hulks expressions.
*/

namespace Hulk;

class Interpreter : Visitor<object>{
    public Interpreter(){}

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
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left + (float)right;

            case TokenType.MINUS:// -
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left - (float)right;

            case TokenType.AT:// @
                CheckStringOperand(expr.Operation,left);
                CheckStringOperand(expr.Operation,right);
                return (string)left + (string)right;

            case TokenType.STAR:// *
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left * (float)right;
            
            case TokenType.SLASH:// /
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left / (float)right;

            case TokenType.CARET:// ^
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float) Math.Pow((float)left,(float)right);//Cast the pow result to keep types consistent
            
            case TokenType.LESS:// <
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left < (float)right;

            case TokenType.LESS_EQUAL:// <=
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left <= (float)right;

            case TokenType.GREATER:// >
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left > (float)right;
            
            case TokenType.GREATER_EQUAL:// >=
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left >= (float)right;

            case TokenType.EQUAL_EQUAL:// ==
                return IsEqual(left,right);
            
            case TokenType.BANG_EQUAL:// !=
                return !IsEqual(left,right);

            case TokenType.AMPERSAND:// &
                CheckBoolOperand(expr.Operation,left);
                CheckBoolOperand(expr.Operation,right);
                return (bool)left && (bool)right;

            case TokenType.PIPE:// |
                CheckBoolOperand(expr.Operation,left);
                CheckBoolOperand(expr.Operation,right);
                return (bool)left || (bool)right;
        }
        return null;
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
    //Check if the object is a number, throw if not
    private void CheckNumberOperand(Token operation, object value){
        if(!(value is float)){
            throw new InterpreterException("Incorrect operand type for "+operation.Lexeme+" operator. Number expected and "+ value.GetType().Name  +" was found.");
        }
    }
    private void CheckStringOperand(Token operation, object value){
        if(!(value is string)){
            throw new InterpreterException("Incorrect operand type for "+operation.Lexeme+" operator. String expected and "+ value.GetType().Name  +" was found.");
        }
    }
    private void CheckBoolOperand(Token operation, object value){
        if(!(value is bool)){
            throw new InterpreterException("Incorrect operand type for "+operation.Lexeme+" operator. Boolean expected and "+ value.GetType().Name  +" was found.");
        }
    }
}