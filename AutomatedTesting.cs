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
        new Test("3.14 - PI <= 0 | false","True")
    };

    public static  void Test(){
        Console.WriteLine("Testing\n");
        int testNumber = 1;
        foreach(Test test in tests){
            string result = Hulk.Run(test.source + ";");
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