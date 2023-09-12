/*
Performs automated testing
*/

namespace Hulk;

/*
The source executed by the interpreter and the value expected.
*/
struct Test
{
    public Test(string source,string value){
        this.source = source;
        this.value = value;
    }
    public string source;
    public string value;
}

static class Tester{
    private static Test[] tests = new Test[] {
        //Literals
        new Test("2.141527","2.141527"),//Floating point number
        new Test("12","12"),//Integer
        new Test("\"Pedrito fue a la escuela.\"","Pedrito fue a la escuela."),//String literal
        new Test("\"\\\"This is a quoted string\\\"\"","\"This is a quoted string\""),//Escaped quote
        new Test("\"This is on line 1.\\nThis is on line 2.\"","This is on line 1.\nThis is on line 2."),//Escaped newline
        new Test("\"Iam\\ttabbed\"","Iam\ttabbed"),//Escaped tab
        // Boolean literals
        new Test("true","True"),
        new Test("false","False"),
        //Language constants
        new Test("PI",Math.PI.ToString()),
        new Test("E",Math.E.ToString()),

        // //String concatenation
        new Test("\"alfa\" @ \"beta\" @ \"ganma\"", "alfabetaganma"),

        //Unary
        new Test("-15","-15"),
        new Test("-12.25","-12.25"),
        new Test("!true","False"),
        new Test("!false","True"),
        new Test("!!true","True"),
        new Test("!!false","False"),
        new Test("--1","1"),
        new Test("---15.37","-15.37"),

        //Integer arithmetic
        new Test("2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10","54"),//Sum
        new Test("2 - 3 - 4 - 5 - 6","-16"),//Substraction
        new Test("2 * 3 * 4 * 5","120"),//Multiplication
        new Test("4 / 2","2"),//Division
        new Test("4^3^2","262144"),//Power
        new Test("15 % 4 % 2","1"),//Modulus
        new Test("16%4","0"),
        
        //Real numbers arithmetic
        new Test("2.14 - 3.14 + 0.86","-0.14"),//Rouding Error because of floating point representation imprecisions
        new Test("3 + 10 / 5","5"),
        new Test("10 / 5 + 3","5"),
        new Test("2 * 2.28 - 4.56","0"),
        new Test("-4.56 + 2.28 * 2","0"),
        new Test("2^4*3+1.15","49.15"),
        new Test("3*2^4+1.15","49.15"),
        new Test("1.15+3*2^4","49.15"),
        new Test("1.15+2^4*3","49.15"),
        new Test("4^0.5","2"),
        new Test("2+3*4","14"),
        new Test("2*3+4","10"),
        new Test("2-3*4","-10"),
        new Test("3+4/2","5"),
        new Test("4/2+3","5"),
        new Test("4/2+3","5"),
        new Test("3-4/2","1"),
        new Test("4/2-3","-1"),
        new Test("2 + 3 ^ 2 / 11",(9.0/11.0+2.0).ToString()),
        new Test("2^2 +3","7"),
        new Test("12 * -5","-60"),
        new Test("-5 * 12","-60"),
        new Test("-5 * -12","60"),
        new Test("-12 * -5","60"),
        new Test("2 ^ 3 + 4 ^ 2 * 3 - 15 + 12 * -5 / 60","40"),
        new Test("2 * PI",((float)2.0 * (float)Math.PI).ToString()),
        new Test("E ^ 2",(Math.Pow(Math.E,2)).ToString()),
        new Test("2 ^ E",((float)Math.Pow(2,Math.E)).ToString()),
        new Test("PI * 2 + 1",((float)Math.PI * 2.0 + 1).ToString()),
        new Test("E ^ 2 / E * PI",(Math.E * Math.E / Math.E * Math.PI).ToString()),
        new Test("2 * PI - PI - PI","0"),
        //Parenthesis allowed
        new Test("28*(1+1)","56"),
        new Test("28*(1-1)","0"),
        new Test("-32 * (-1) + 2^(2+3)","64"),
        new Test("-32 + (-1) * 2^(2+3)","-64"),
        new Test("-32 - (-1) * 2^(2+3)","0"),
        new Test("-32 * (-1) - 2^(2+3)","0"),
        new Test("-(2 - 3)","1"),
        new Test("10 / (5 / 5)","10"),
        new Test("(10 / 5) / 4","0.5"),
        new Test("\"alfa\" @ (\"omega\" @ \"beta\")","alfaomegabeta"),
        new Test("2 - 3 - 4","-5"),
        new Test("2 - (3 - 4)","3"),
        new Test("(((((\"hello world!\")))))","hello world!"),
        new Test("1 + (3 - 4 * (12 - 574) + 3.14 - (-5 / 2.5))",(1 + (3 - 4 * (12 - 574) + 3.14 - (-5 / 2.5))).ToString()),

        //Comparison
        new Test("2 > 3","False"),
        new Test("3 > 2","True"),
        new Test("2 + 3 < 3 + 2","False"),
        new Test("2 + 3 <= 3 + 2","True"),
        new Test("2 * -1 <= 0 + 1","True"),
        new Test("-3 ^ 4 < 0","False"),
        new Test("2 + 2 < 2 * 2","False"),
        new Test("14 * 21 ^ 3 - 5.16 > (-5.16 + (-(13 ^ 4))) * 1.75","True"),
        new Test("12 * 10 / 2 / 4 >= 15","True"),
        new Test("!(3 > 2)","False"),
        new Test("!(2 > 3)","True"),
        new Test("2 ^ 15 > -2 ^ 15","True"),

        //Equality
        new Test("2 > 3 == false","True"),
        new Test("3 == 4","False"),
        new Test("\"Alfa numeric\" != \"Alfa Numeric\"","True"),
        new Test("2 + 3 == 5 * 12","False"),
        new Test("!(2 * 2 ^(2 + 1) != 32 / 2 + 0)","True"),
        new Test("(2 == 3) != (2 > 3)","False"),
        new Test("true != true","False"),
        new Test("true == true","True"),
        new Test("false != false","False"),
        new Test("false == false","True"),
        new Test("true == false","False"),
        new Test("false == true","False"),
        new Test("true != false","True"),
        new Test("false != true","True"),
        new Test("\"Pedrito\" == \"Pedrito\"","True"),

        //Logical operators
        new Test("true & true","True"),
        new Test("true & false","False"),
        new Test("false & true","False"),
        new Test("false & false","False"),
        new Test("true | true","True"),
        new Test("true | false","True"),
        new Test("false | true","True"),
        new Test("false | false","False"),
        new Test("\"negro\" == \"blanco\" & 2 + 2 == 4","False"),
        new Test("(false & false) | true","True"),
        new Test("false & (false | true)","False"),
        new Test("false & false | true","True"),//El operador & se ejecuta antes que el operador |
        new Test("!(true | false)","False"),
        new Test("3 >= 2 & 3 <= 2","False"),
        new Test("3 >= 2 & 2 >= 3","False"),
        new Test("3.14 - PI <= 0 | false","True"),
        //if-else
        new Test("if (true) 5 else 6","5"),
        new Test("if (false) 5 else 6","6"),
        new Test("2 + (if (2 <= 3) 5 ^ 3 else 24 / 14.3)","127"),
        new Test("if( 2 * 8 <= 15 | !( if (\"test\" @ \"ear\" == \"testear\") 4 <= 4 ^ 1 / 4 else false ) ) \"Entre aqui\" else -1 ","Entre aqui"),
        new Test("(if(\"Pedrito\" == \"Calvo\") 54 else 27 ^ (2 + 1) - 1) % 27","26"),
        new Test("if (3.14 - PI < 0) \"Is under my Pi\" else if (3.14 - PI == 0) \"Is on my Pi\" else \"Is over my Pi\"","Is under my Pi"),
        new Test("if (3.14 - PI > 0) \"Is under my Pi\" else if (3.14 - PI == 0) \"Is on my Pi\" else \"Is over my Pi\"","Is over my Pi"),
        new Test("if (3.14 - PI > 0) \"Is under my Pi\" else if (3.14 - PI < 0) \"Is on my Pi\" else \"Is over my Pi\"","Is on my Pi"),
        new Test ("if ( \"moves\" == \"e4 & d5\") \"Scandinavian defense\" else \"Cant figure it out\"","Cant figure it out"),
        //Let - in expressions
        new Test("let a = 1, b = 2 in a + b","3"),
        new Test("let var = 5 in 6","6"),
        new Test("let var = 5 in var","5"),
        new Test("let var = 3 in let var = 4 in var","4"),
        new Test("2 + (let var1 = 5 , var2 = 7 in (var1 * var2) ^ 2 )",(35 * 35 + 2).ToString()),
        new Test("let a = \"number\", a = a @ \"two\" in a","numbertwo"),
        new Test("let a = 2 in let a = 3 in let a = 4 in a","4"),
        new Test("let a = 2 in a + (let a = 3 in a + (let a = 4 in a))","9"),
        new Test("let moves = \"e4xd5\" in if(moves == \"e4xd5\") \"Scandinavian defense\" else \"Cant figure it out\"","Scandinavian defense"),
        new Test("let moves = \"e3xd5\" in if(moves == \"e4xd5\") \"Scandinavian defense\" else \"Cant figure it out\"","Cant figure it out"),
        //Function declaration and usage.
        new Test("function Max(a,b) => if(a >= b) a else b","NULL"),
        new Test("Max(1,2)","2"),
        new Test("function Fibonaci(a) => if(a <= 1) 1 else Fibonaci(a - 1) + Fibonaci(a - 2)","NULL"),
        new Test("Fibonaci(0)","1"),
        new Test("Fibonaci(1)","1"),
        new Test("Fibonaci(2)","2"),
        new Test("Fibonaci(3)","3"),
        new Test("Fibonaci(4)","5"),
        new Test("Fibonaci(5)","8"),
        new Test("let n = 15, m = 21 in Max(Fibonaci(m),Fibonaci(n))","17711"),
        //A function with a parameter named like another function yet to be declared.
        new Test("function thisIsFun(sum) => \"ja ja ja\"","NULL"),
        new Test("thisIsFun(2)","ja ja ja"),
        new Test("function sum(a,b) => a + b","NULL"),
        new Test("thisIsFun(2)","ja ja ja"),
        //Other tests
        new Test("true | false | true","True"),
        new Test("let x = 12 in x == 9 | x == 10 | x == 11 | x == 12","True"),
        new Test("true & false & true","False"),
        new Test("let x = 12 in x == 9 & x == 10 & x == 11 & x == 12","False"),
        new Test("let a = 2 , b = 4 , c = \"pedrito\" in (a == b) != c","True"),
        new Test("let a = 2 , b = 4 , c = \"pedrito\" in a == (b != c)","False")
    };

    public static  void Test(){
        Console.WriteLine("Testing\n");
        int testNumber = 1;
        foreach(Test test in tests){
            string? result = Hulk.Run(test.source + ";");
            if(result == null)result = "NULL";
            string expected = test.value;
            Console.Write("T "+testNumber+": ");
            if(result != expected){
                Console.WriteLine("WA in test \"" + test.source + "\" . Expected \"" + test.value + "\" and found \"" + result + "\"");
            }
            else Console.WriteLine("Ok");
            ++testNumber;
        }
        Console.WriteLine("\nDone");
    }
}