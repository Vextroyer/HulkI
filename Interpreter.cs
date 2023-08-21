// /*
// Interprets hulks expressions.
// */

namespace Hulk;

class Interpreter : Visitor<object>{
    public Interpreter(){}

    public string Interpret(Expr expr){
        return Stringify(Evaluate(expr));
    }

    private string Stringify(object value){
        if(value == null)return "NULL";
        return value.ToString();
    }

    private object Evaluate(Expr expr){
        return expr.Accept(this);
    }

    public object VisitLiteralExpr(LiteralExpr expr){
        return expr.Value;
    }
    public object VisitUnaryExpr(UnaryExpr expr){
        object value = Evaluate(expr.Expression);
        switch(expr.Operation.Type){
            case TokenType.BANG:
                return !IsTrue(value);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operation,value);
                return -(float)value;
        }

        //Unreachable
        return null;
    }

    public object VisitBinaryExpr(BinaryExpr expr){
        //IMPORTANT: Evaluating expresions is done from left to right and before thr binary operation.
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);
        switch(expr.Operation.Type){
            case TokenType.PLUS://Adicion
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left + (float)right;

            case TokenType.MINUS://Substraccion
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left - (float)right;

            case TokenType.AT://Concatenacion de cadenas
                CheckStringOperand(expr.Operation,left);
                CheckStringOperand(expr.Operation,right);
                return (string)left + (string)right;

            case TokenType.STAR://Multiplicacion
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left * (float)right;
            
            case TokenType.SLASH://Division
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float)left / (float)right;

            case TokenType.CARET://Potenciacion
                CheckNumberOperand(expr.Operation,left);
                CheckNumberOperand(expr.Operation,right);
                return (float) Math.Pow((float)left,(float)right);//Cast the pow result to keep types consistent
        }
        return null;
    }

    //Una expresion es verdadera si no es falsa o no es null
    private bool IsTrue(object value){
        if(value == null)return false;
        if(value is bool)return (bool)value;
        return true;
    }
    //Comprueba que el objeto dado es un numero, en caso contrario lanza una excepcion
    private void CheckNumberOperand(Token operation, object value){
        if(!(value is float)){
            throw new RuntimeError("Incorrect operand type for "+operation.Lexema+" operator. Number expected and "+ value.GetType().Name  +" was provided.");
        }
    }
    private void CheckStringOperand(Token operation, object value){
        if(!(value is string)){
            throw new RuntimeError("Incorrect operand type for "+operation.Lexema+" operator. String expected and "+ value.GetType().Name  +" was provided.");
        }
    }   
}