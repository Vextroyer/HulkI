/*
Interprets hulks expressions.
*/

namespace Hulk;

class Interpreter : Visitor<object>{
    public Interpreter(){}

    //A global excecution environment.
    private Environment environment = new Environment();

    private int NestedCallCount = 0;//Amount of nested function calls. 
    private int MaxNestedCallCount = 3500;//Limit to 3500 nested function calls. To avoid stack overflows, which cant be captured during runtime.

    public string? Interpret(Expr expr){
        NestedCallCount = 0;//Resets NestedCallCount for every new interpreted line.
        try{
            return Stringify(Evaluate(expr));
        }
        catch(InterpreterException e){
            e.HandleException();
            //Unreachable code
            return null;
        }
    }

    private string? Stringify(object value){
        if(value == null)return null;
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
        //This code is prune to the following error:
        //Some assignments are computed, an error ocurred and the excecution was halted, the computed assignments
        //were never unassigned, so there are variables on the environment that should not be there.
        //It must be guaranteed that the environment state if something fails returns to its previous state,
        //and do not hang on a intermediate state.

        List<Token> guarantee = new List<Token>();

        try{
            //Compute the assigments.
            foreach(AssignmentExpr asignment in expr.Assignments){
                environment.Set(asignment.Identifier,Evaluate(asignment.RValue));
                //If the above fails, the token was not binded thus it doesnt need to be removed.
                //If the above doesnt fails the token was binded and thus it needs to be removed.
                guarantee.Add(asignment.Identifier);
            }

            //Evaluate the expression.
            object value = Evaluate(expr.InBranchExpr);

            //Uncompute the assignments.
            foreach(AssignmentExpr asignment in expr.Assignments){
                environment.Remove(asignment.Identifier);
            }

            return value;
        }catch(Exception e){
            //Return the environment to its previous state.
            //by uncomputing the computed assignments.
            foreach(Token identifier in guarantee)environment.Remove(identifier);
            //Rethrow
            throw e;
        }
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
    public object VisitFunctionExpr(FunctionExpr fun){
        environment.Register(fun);
        //Nothing to return
        return null;
    }
    //Evaluates a function call
    public object VisitCallExpr(CallExpr expr){
        if(NestedCallCount > MaxNestedCallCount){
            Console.WriteLine("Limit of nested function calls exceded, aborting excecution, this can take a minute.");
            throw new InterpreterException($"Stack overflow on call to function '{expr.Identifier.Lexeme}'");
        }

        //Its a builtin function
        if(environment.IsBuiltin(expr.Identifier.Lexeme,expr.Arity)){
            //Retrieve the value of the parameters
            List<object> _parameters = new List<object>();
            foreach(Expr paramExpr in expr.Parameters)_parameters.Add(Evaluate(paramExpr));
            switch(expr.Identifier.Lexeme){
                case "rand":
                    Random random = new Random();
                    return random.NextSingle();
                case "cos":
                    return (float) Math.Cos((float)_parameters[0]);
                case "exp":
                    return (float) Math.Exp((float)_parameters[0]);
                case "print":
                    Console.WriteLine(_parameters[0]);//Print echoes its content before returning it for debugging purposes.
                    return _parameters[0];
                case "sin":
                    return (float) Math.Sin((float)_parameters[0]);
                case "sqrt":
                    return (float) Math.Sqrt((float)_parameters[0]);
                case "log":
                    return (float) Math.Log((float)_parameters[1],(float)_parameters[0]);
                default:
                    throw new NotImplementedException();
            }
        }
        //Its not a function.
        if(!environment.IsFunction(expr.Identifier))throw new InterpreterException("'" + expr.Identifier.Lexeme + "' is not a function.",expr.Identifier.Offset);
        //The function has the incorrect number of parameters.
        if(!environment.IsFunction(expr.Identifier,expr.Arity)){
            string message = "'" + expr.Identifier.Lexeme + "' has the incorrect number of parameters. Expected : ";
            List<int> aritys = environment.GetAritys(expr.Identifier.Lexeme);
            aritys.Sort();
            for(int i=0;i<aritys.Count;++i){
                message += aritys[i].ToString();
                if(i == aritys.Count - 1)continue;
                if(i == aritys.Count - 2)message += " or ";
                else message += ", ";
            }
            message += $" parameters, but {expr.Arity} were passed.";
            throw new InterpreterException(message,expr.Identifier.Offset);
        }

        //Uses a let-in expression as an auxiliary for computing the function value
        
        //First create the assignments associating the arguments with the parameters
        List<AssignmentExpr> assignments = new List<AssignmentExpr>();
        List<Token> arguments = environment.GetArguments(expr.Identifier.Lexeme,expr.Arity);
        List<Expr> parameters = expr.Parameters;
        for(int i=0;i<expr.Arity;++i){
            assignments.Add(new AssignmentExpr(arguments[i],parameters[i]));
        }

        //Now create a let-in expression from the assigments created and the function body.
        LetInExpr functionCall = new LetInExpr(assignments,environment.GetBody(expr.Identifier.Lexeme,expr.Arity));

        //Different exceptions may arise during the call to the function.
        //Stack overflows will be handled

        ++NestedCallCount;//The function is called.
        object returnValue = Evaluate(functionCall);
        --NestedCallCount;//The function succesfully returns.
        return returnValue;
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