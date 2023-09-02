/*
Environment for the interpreter.
*/
namespace Hulk;
class Environment{
    private Dictionary<string,List<object>> table;//Associates identifiers to their values. A list is used for support nested declarations.
    public Environment(){
        table = new Dictionary<string,List<object>>();
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
}