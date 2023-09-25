Hulki is a Hulk interpreter.

Hulk is a programming language created at Habana University for
educational purposes. You can learn more about it at
https://matcom.in/hulk

This version of the interpreter offers support for one line expressions, wich I like to call Hulk one-liners.

As you may read on the specifications of the language (almost)everything in Hulk is an expression and thus has a return value.

This interpreter support inline functions(can be written on one line) and recursion.

Function declaration is a type of expression that dont return a value.

An example of a recursive function for computing the factorial of a non-negative integer:
Declaration:
	function Factorial(integer) => if(integer == 0) 1 else integer * Factorial(integer - 1);
Usage:
	Factorial(5);
Which yields 120.

Redeclaration of a function is not allowed in this version of the interpreter, but you can modify the source code
to support it, its just uncommenting some lines and commenting others.

Besides functions Hulki supports :
	-numbers, all numbers are considered real numbers.
	-strings, with the escaped characters '\t' for tabs, '\n' for newlines and '\"' for quotes.
	-Unary operators(! -), for logic negation and negative sign.
	-Real number arithmetic (+ - * / % ^), with the caret(^) for exponentiation.
	-String concatenation (@), "alpha"@"bet"; yields "alphabet".
	-Relations (< <= > >= != ==), as usual.
	-Logic operators (& |)a, for 'logical and' and 'logical or', without shortcircuit.
	-Conditional expressions of the form 'if(condition) expression1 else expression2' where condition must evaluate
	to a boolean value (true or false).
	-Declarations of variables on the form 'let assignments in expression' where assignments is a non empty list of comma(,)
	separated assignments. An assignment is of the form 'variableName = expression'.After the in keyword the variables can be used.
	For example 'let x = 5, y = -5 in cos(x + y)' yields 1.
	-Native functions : rand(), sin(number), cos(number), exp(number), sqrt(number), log(base,value), print(x)
	-Builtin-constants : E for Euler constant and PI for pi constant.


To run the interpreter use the Hulk.sh script.
A script for Windows will be added soon.