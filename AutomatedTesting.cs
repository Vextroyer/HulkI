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
        new Test("2.141527","2.141527"),//Numero en coma flotante
        new Test("12","12"),//Numero entero
        new Test("\"Pedrito fue a la escuela.\"","Pedrito fue a la escuela."),//Literal de cadena
        new Test("\"\\\"This is a quoted string\\\"\"","\"This is a quoted string\""),//Escaped quote
        new Test("\"This is on line 1.\\nThis is on line 2.\"","This is on line 1.\nThis is on line 2."),//Escaped newline
        new Test("\"Iam\\ttabbed\"","Iam\ttabbed"),//Escaped tab
        // Boolean literals

        // //Concatenaciones de cadenas
        new Test("\"alfa\" @ \"beta\" @ \"ganma\"", "alfabetaganma"),

        //Operaciones aritmeticas con enteros
        new Test("2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10","54"),//Suma
        new Test("2 - 3 - 4 - 5 - 6","-16"),//Resta
        new Test("2 * 3 * 4 * 5","120"),//Multiplicacion
        new Test("4 / 2","2"),//Division
        new Test("4^3^2","262144"),//Potenciacion
        
        //Operaciones aritmeticas con numeros reales
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
        new Test("2 ^ 3 + 4 ^ 2 * 3 - 15 + 12 * -5 / 60","40")

    };

    public static  void Test(){
        Console.WriteLine("Testing\n");
        int testNumber = 1;
        foreach(Test test in tests){
            string result = Hulk.Run(test.source);
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