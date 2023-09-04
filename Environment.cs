/*
Environment for the interpreter.
*/
namespace Hulk;
class Environment{
    private Dictionary<string,List<object>> table;//Associates identifiers to their values. A list is used for support nested declarations.
    /*
    The function representation is as follows:
    A function is uniquely determined by its name and arity.
    Name comes first.
    Arity comes next.
    This is an example of the structure used for storing functions.
    Name                Arity               An example body
    
    Sum --------------- 2 ----------------- a + b
        \-------------- 3 ----------------- a + b + c
    
    Max --------------- 2 ----------------- if(a >= b) a else b
        \-------------- 3 ----------------- let m = Max(a,b) in if(m >= c) m else c
    
    This structure can be extended to support types because types comes after arity in the hieracy.
    */
    private Dictionary<string,Dictionary<int,FunctionExpr>> funcTable;//Represents the functions. 
    public Environment(){
        table = new Dictionary<string,List<object>>();
        funcTable = new Dictionary<string, Dictionary<int, FunctionExpr>>();
        //Put the primitive functions here.
    }

    //Retrieves the value associated with the given identifier.
    public object Get(Token identifier){
        try{
            return table[identifier.Lexeme].Last();
        }catch(KeyNotFoundException){
            throw new InterpreterException("Variable '" + identifier.Lexeme + "' is used but not declared.",identifier.Offset);
        }
    }
    //Associates an identifier with a value.
    public void Set(Token identifier,object value){
        if(!table.ContainsKey(identifier.Lexeme))table.Add(identifier.Lexeme,new List<object>());
        table[identifier.Lexeme].Add(value);
    }

    //Remove the last value associated with the identifier and delete it if no values remains associated to it.
    public void Remove(Token identifier){
        table[identifier.Lexeme].RemoveAt(table[identifier.Lexeme].Count - 1);//Removes the last element.
        if(table[identifier.Lexeme].Count == 0)table.Remove(identifier.Lexeme);//Remove the association if there are no more values.
    }
    //Register a function. Functions are globally scoped.
    public void Register(FunctionExpr fun){
        string name = fun.Identifier.Lexeme;
        int arity = fun.Arity;
        if(funcTable.ContainsKey(name)){
            //There is a function with this name
            Dictionary<int,FunctionExpr> arityTable = funcTable[name];
            if(arityTable.ContainsKey(arity)){
                //There is a function with this arity
                if(!IsOverwritable(arityTable[arity]))throw new InterpreterException("Function '" + fun.Identifier.Lexeme + "' can not be redefined.");
                else {
                    funcTable[name][arity] = fun;
                }
            }else{
                funcTable[name].Add(arity,fun);
            }
        }else{
            Dictionary<int,FunctionExpr> arityTable = new Dictionary<int, FunctionExpr>();
            arityTable.Add(arity,fun);
            funcTable.Add(name,arityTable);
        }
    }
    //Determines wheter a function is overwritable.
    private bool IsOverwritable(FunctionExpr fun){
        return fun.Overwritable;
    }
    //Determine if a given identifier can be used like a function.
    public bool IsFunction(Token identifier){
        if(funcTable.ContainsKey(identifier.Lexeme))return true;
        return false;
    }
    public bool IsFunction(Token identifier,int arity){
        if(IsFunction(identifier)){
            return funcTable[identifier.Lexeme].ContainsKey(arity);
        }
        return false;
    }
    //Returns the parameters of the corresponding function.
    public List<Token> GetArguments(string name,int arity){
        return funcTable[name][arity].Args;
    }
    //Returns the body of the corresponding function.
    public Expr GetBody(string name,int arity){
        return funcTable[name][arity].Body;
    }
}