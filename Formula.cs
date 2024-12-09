// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>

// <summary
// Class <c>Formula/c> 
// Represents a formula entered into a spreadsheet application. It ensures the formula
// has correct syntax and provides methods to evaluate an entered formula.
// </summary>
// <authors> [Rishabh Saini] </authors>
// <date> [20th September, 2024] </date>


namespace CS3500.Formula;

using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>  + 
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    /// StringBuilder that holds the given formula in normalized and formatted form.
    /// </summary>
    private StringBuilder FormattedFormula;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        FormattedFormula = new StringBuilder();
        List<string> tokens = GetTokens(formula);

        //Build the formula in the canonical form i.e. by normalizing and formatting the tokens in the foemula
        foreach (string token in tokens)
        {
            //If it is a number, it needs to be formatted.
            if (double.TryParse(token, out double num))
                FormattedFormula.Append(num.ToString());

            //If it not a number, specifically if it is a variable, it needs to be normalized.
            else
                FormattedFormula.Append(token.ToUpper());
        }

        //Throwing an exception is the formula does not contain any spaces (e.g.:- contains only spaces)
        if (tokens.Count == 0)
            throw new FormulaFormatException("Formula contains no tokens.");

        int ClosingParenthesesCount = 0;
        int OpeningParenthesesCount = 0;
        //Checks the validity of the tokens in the formula,and also checks if the closing parentheses seen so far
        //exceeds opening parnetheses seen so far.
        foreach (string token in tokens)
        {
            if (Double.TryParse(token, out double _) || IsVar(token) || IsOperator(token)
                || token.Equals("(") || token.Equals(")"))
            {
                if (token.Equals(")"))
                    ClosingParenthesesCount++;

                else if (token.Equals("("))
                    OpeningParenthesesCount++;

                if (ClosingParenthesesCount > OpeningParenthesesCount)
                    throw new FormulaFormatException("Number of closing parentheses exceeds number of opening parentheses.");
            }

            else
                throw new FormulaFormatException("Formula contains invalid tokens.");
        }

        //Checks if the number of opening parentheses exceeds that of closing parentheses.
        if (OpeningParenthesesCount > ClosingParenthesesCount)
            throw new FormulaFormatException("Number of opening parentheses exceeds number of closing parentheses.");

        //Checks validity of first token. It can only be a variable, number or opening parentheses.
        string firstToken = tokens[0];
        if (!IsVar(firstToken) && !Double.TryParse(firstToken, out double _) && !firstToken.Equals("("))
            throw new FormulaFormatException("First token must be a number, a variable, or an opening parenthesis");

        //Checks validity of last token. It can only be a variable, number or closing parentheses.
        string lastToken = tokens[tokens.Count - 1];
        if (!IsVar(lastToken) && !Double.TryParse(lastToken, out double _) && !lastToken.Equals(")"))
            throw new FormulaFormatException("Last token must be a number, a variable, or an closing parenthesis");

        for (int i = 0; i < tokens.Count; i++)
        {
            string CurrentToken = tokens[i];

            //Break the loop at the second to last iteration to prevent an IndexOutOfRangeException().
            if (i >= tokens.Count - 1)
                break;

            string NextToken = tokens[i + 1];
            //Checks that the token following an opening parentheses or operator is a variable, number, or opening parentheses.
            if (CurrentToken == "(" || IsOperator(CurrentToken))
            {
                if (IsVar(NextToken) || Double.TryParse(NextToken, out double _)
                    || NextToken.Equals("("))
                    continue;

                else
                    throw new FormulaFormatException("Token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis");

            }
            //Checks that the token following a variable, number, or closing parentheses is an operator or closing parentheses.
            else if (IsVar(CurrentToken) || Double.TryParse(CurrentToken, out double _)
                || CurrentToken.Equals(")"))
            {
                if (IsOperator(NextToken) || NextToken.Equals(")"))
                    continue;

                else
                    throw new FormulaFormatException("Token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
            }
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        List<string> tokens = GetTokens(ToString());
        HashSet<string> variables = new HashSet<string>();
        foreach (string token in tokens)
        {
            if (IsVar(token))
                variables.Add(token.ToUpper());
        }
        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return this.FormattedFormula.ToString();
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   Reports whether "token" is a an operator. In this method, "(" and ")" 
    ///   is not included, only operators that operate on two numbers such as
    ///   "+","-", "*", and "/" are considered.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true is the token is +, - , * or /. </returns>
    private static bool IsOperator(string token)
    {
        //Regex expression to define operators +,*/,-. 
        string OperatorsRegExPattern = "^[+*/-]$";

        return Regex.IsMatch(token, OperatorsRegExPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !(f1.Equals(f2));
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Formula)
            return false;

        return ToString().Equals(obj.ToString());
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a 
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect 
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        Stack<string> values = new();
        Stack<string> operators = new();

        foreach (string token in GetTokens(ToString()))
        {
            //Check if token is a number
            if (double.TryParse(token, out double _))
            {
                if (TopOperatorIsMultiplyingOrDividing(operators))
                {
                    double.TryParse(values.Pop(), out double value);
                    double.TryParse(token, out double TokenValue);

                    //if TokenValue is zero, MuliplyOrDivideTwoValues() returns FormulaError.
                    if (MuliplyOrDivideTwoValues(values, operators, value, TokenValue) is FormulaError)
                        return new FormulaError("Division by zero");
                }
                else
                    values.Push(token);
            }

            else if (IsVar(token))
            {
                double VariableValue;
                //If lookup() does not yield a variable value, return FormulaError.
                try
                {
                    VariableValue = lookup(token);
                }
                catch (ArgumentException)
                {
                    return new FormulaError("Value of variable was not found,");
                }
                if (TopOperatorIsMultiplyingOrDividing(operators))
                {
                    double.TryParse(values.Pop(), out double value);

                    //if VariableValue is zero, MuliplyOrDivideTwoValues() returns FormulaError.
                    if (MuliplyOrDivideTwoValues(values, operators, value, VariableValue) is FormulaError)
                        return new FormulaError("Division by zero");
                }
                else
                    values.Push(VariableValue.ToString());
            }

            else if (token == "+" || token == "-")
            {
                if (TopOperatorIsAddingOrSubtracting(operators))
                {
                    double.TryParse(values.Pop(), out double value1);
                    double.TryParse(values.Pop(), out double value2);

                    AddOrSubtractTwoValues(values, operators, value2, value1);
                }
                operators.Push(token);
            }

            else if (token == "*" || token == "/")
                operators.Push(token);

            else if (token == "(")
                operators.Push(token);

            else if (token == ")")
            {
                if (TopOperatorIsAddingOrSubtracting(operators))
                {
                    double.TryParse(values.Pop(), out double value1);
                    double.TryParse(values.Pop(), out double value2);

                    AddOrSubtractTwoValues(values, operators, value2, value1);
                }
                operators.Pop();
                if (TopOperatorIsMultiplyingOrDividing(operators))
                {
                    double.TryParse(values.Pop(), out double value1);
                    double.TryParse(values.Pop(), out double value2);

                    //if value2 is zero, MuliplyOrDivideTwoValues() returns FormulaError.
                    if (MuliplyOrDivideTwoValues(values, operators, value2, value1) is FormulaError)
                        return new FormulaError("Division by zero");
                }
            }
        }

        if (operators.Count == 0)
        {
            double.TryParse(values.Pop(), out double value);
            return value;
        }
        else
        {
            double.TryParse(values.Pop(), out double value1);
            double.TryParse(values.Pop(), out double value2);

            AddOrSubtractTwoValues(values, operators, value2, value1);
            double.TryParse(values.Pop(), out double value);
            return value;
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    ///   <para>
    ///     Adds or subtracts two given values. The decision of whether to add or to subtract depends on
    ///     what operator is present at the top of the operator stack.
    ///   </para>
    ///   <remarks>
    ///     The order of subtraction would be first passed value - second passed value.
    ///   </remarks>
    ///    </summary>
    ///  <param name="values">  Stack containing all values. </param>
    ///  <param name="operators">  Stack containing all operators.</param>
    ///  <param name="value1">  The first value to be operated. </param>
    ///  <param name="value2">  The second value to be operated.</param>
    private static void AddOrSubtractTwoValues(Stack<string> values, Stack<string> operators, double value1, double value2)
    {
        if (operators.Pop() == "+")
            values.Push((value1 + value2).ToString());

        else
            values.Push((value1 - value2).ToString());
    }

    /// <summary>
    ///   <para>
    ///     Multiplies or divides two given values. The decision of whether to multiply or to divide depends on
    ///     what operator is present at the top of the operator stack.
    ///   </para>
    ///   <remarks>
    ///     The order of division would be first passed value / second passed value.
    ///   </remarks>
    ///    </summary>
    ///  <param name="values">  Stack containing all values. </param>
    ///  <param name="operators">  Stack containing all operators.</param>
    ///  <param name="value1">  The first value to be operated. </param>
    ///  <param name="value2">  The second value to be operated.</param>
    ///  <returns>
    ///  Either an empty object or FormlaError. A Formula error would be returned when a divsion by zero would occur.
    ///  An empty object would be returned when the method executes successfully without any errors.
    ///  </returns>
    private static object MuliplyOrDivideTwoValues(Stack<string> values, Stack<string> operators, double value1, double value2)
    {
        if (operators.Pop() == "*")
            values.Push((value1 * value2).ToString());

        else
        {
            if (value2 == 0)
                return new FormulaError("Division by zero.");

            values.Push((value1 / value2).ToString());
        }

        return new object();
    }

    /// <summary>
    ///   <para>
    ///     Checks if the top operator is an additon or subtraction symbol.
    ///   </para>
    /// </summary>
    ///  <param name="operators">  Stack containing all operators.</param>
    ///  <returns>
    ///  True is top operator if is "+" or "-", false otherwise.
    ///  If the stack is empty, then too the method would return false.
    ///  </returns>
    private static bool TopOperatorIsAddingOrSubtracting(Stack<string> operators)
    {
        return (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"));
    }

    /// <summary>
    ///   <para>
    ///     Checks if the top operator is a multiplcation or division symbol.
    ///   </para>
    /// </summary>
    ///  <param name="operators">  Stack containing all operators.</param>
    ///  ///  <returns>
    ///  True is top operator if is "*" or "/", false otherwise.
    ///  If the stack is empty, then too the method would return false.
    ///  </returns>
    private static bool TopOperatorIsMultiplyingOrDividing(Stack<string> operators)
    {
        return (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"));
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }

}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);
