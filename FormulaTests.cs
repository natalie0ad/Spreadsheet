// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>

// <summary
// Class <c>FormulaSyntaxTests/c> Contains tests for the formula class.
// </summary>
// <authors> [Rishabh Saini] </authors>
// <date> [20th September, 2024] </date>

namespace CS3500.FormulaTests;

using CS3500.Formula; // Change this using statement to use different formula implementations.

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    [TestMethod]
    public void FormulaConstructor_TestOneToken_Valid()
    {
        _ = new Formula("5");
    }


    // --- Tests for Valid Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestParenthesis_Valid()
    {
        _ = new Formula("(9)");
    }

    [TestMethod]
    public void FormulaConstructor_TestPositiveScientificNotation_Valid()
    {
        _ = new Formula("9e5");
    }

    [TestMethod]
    public void FormulaConstructor_TestNegativeScientificNotation_Valid()
    {
        _ = new Formula("9e-5");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNegativeNumber_Invalid()
    {
        _ = new Formula("-5");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLetter_Invalid()
    {
        _ = new Formula("c");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidTokens_Invalid()
    {
        _ = new Formula("@ + $");
    }

    [TestMethod]
    public void FormulaConstructor_TestAdditionSymbol_Valid()
    {
        _ = new Formula("5+5");
    }

    [TestMethod]
    public void FormulaConstructor_TestSubtractionSymbol_Valid()
    {
        _ = new Formula("5-5");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultiplicationSymbol_Valid()
    {
        _ = new Formula("5*5");
    }

    [TestMethod]
    public void FormulaConstructor_TestDivisionSymbol_Valid()
    {
        _ = new Formula("5/5");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariable_Valid()
    {
        _ = new Formula("a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLongerVariable_Valid()
    {
        _ = new Formula("abDfc156");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidLongerVariable_Invalid()
    {
        _ = new Formula("a2+a1+1a-a@+6z/5&");
    }

    // --- Tests for Closing Parenthesis Rule

    [TestMethod]
    public void FormulaConstructor_TestForClosingParenthesis_Valid()
    {
        _ = new Formula("(2+a3)-(a7/(3+b2))");
    }


    /// <summary>
    ///   <para>
    ///     Make sure a formula in which the number of closing parenthesis is greater than 
    ///     the number of opening parenthesis throws a FormulaFormatException.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "(2+a3)-(a7/(3+56+(a7))))" is an invalid formula which should throw an error.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestForExcessiveClosingParenthesis_Invalid()
    {
        _ = new Formula("(2+a3)-(a7/(3+56+(a7))))");
    }


    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestForOneClosingParenthesis_Invalid()
    {
        _ = new Formula(")");
    }


    // --- Tests for Balanced Parentheses Rule

    [TestMethod]
    public void FormulaConstructor_TestForBalancedParenthesis_Valid()
    {
        _ = new Formula("((x2-x1)+(y2-y1))");
    }

    /// <summary>
    ///   <para>
    ///     Make sure a formula in which the number of opening parenthesis is greater than 
    ///     the number of closing parenthesis throws a FormulaFormatException.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "(5+3)*8/(2*(45)" is an invalid formula which should throw an error.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestForUnbalancedParenthesis_Invalid()
    {
        _ = new Formula("(5+3)*8/(2*(45)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneOpeningParenthesis_Invalid()
    {
        _ = new Formula("(");
    }

    // --- Tests for First Token Rule

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenAsNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "(1+1)" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenAsOpeningParenthesis_Valid()
    {
        _ = new Formula("(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenAsVariable_Valid()
    {
        _ = new Formula("abc123/2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenAsOperator_Invalid()
    {
        _ = new Formula("+3");
    }

    // --- Tests for  Last Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestLastTokenAsNumber_Valid()
    {
        _ = new Formula("1+1");
    }
    [TestMethod]
    public void FormulaConstructor_TestLastTokenAsClosingParenthesis_Valid()
    {
        _ = new Formula("(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenAsVariable_Valid()
    {
        _ = new Formula("(4) + abc123");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenAsOperator_Invalid()
    {
        _ = new Formula("9+");
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestNumberAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("(4+4)");
    }

    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("((4+4))");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("(abc123+4)");
    }

    [TestMethod]
    public void FormulaConstructor_TestScientificNotationAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("(4.5e6+2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisAfterOpeningParenthesis_Invalid()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorAfterOpeningParenthesis_Invalid()
    {
        _ = new Formula("(*");
    }

    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisAfterOperator_Valid()
    {
        _ = new Formula("(4+(4))");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableAfterOperator_Valid()
    {
        _ = new Formula("(4+a8)");
    }

    [TestMethod]
    public void FormulaConstructor_TestScientificNotationAfterOperator_Valid()
    {
        _ = new Formula("(4+1e-4)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingParenthesis_Invalid()
    {
        _ = new Formula("(4+)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorAfterOperator_Invalid()
    {
        _ = new Formula("4++4");
    }

    // --- Tests for Extra Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterNumber_Valid()
    {
        _ = new Formula("4+a8");
    }

    [TestMethod]
    public void FormulaConstructor_TestNumberAfterNumber_Valid()
    {
        _ = new Formula("45");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterVariable_Valid()
    {
        _ = new Formula("a8-2");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterClosingParenthesis_Valid()
    {
        _ = new Formula("(p2)/4");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterNumber_Valid()
    {
        _ = new Formula("a8+(4)");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterVariable_Valid()
    {
        _ = new Formula("(a8)+2");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterClosingParenthesis_Valid()
    {
        _ = new Formula("((2-4))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterNumber_Invalid()
    {
        _ = new Formula("4(2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterVariable_Invalid()
    {
        _ = new Formula("a4(a2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterClosingParenthesis_Invalid()
    {
        _ = new Formula(")(");
    }

    [TestMethod]
    public void FormulaConstructor_TestToStringWithScientificNotation_Valid()
    {
        Assert.AreEqual("5+600000+0.0007-5", new Formula("5 + 6e5 + 7e-4 - 5.000").ToString());
    }

    [TestMethod]
    public void FormulaConstructor_TestToStringWithVariable_Valid()
    {
        Assert.AreEqual("5+ABC1+90", new Formula("5 + abc1 +    90.0").ToString());
    }

    [TestMethod]
    public void FormulaConstructor_TestGetVariablesWithOnlyValidVariables_Valid()
    {
        HashSet<string> variables = new HashSet<string>();
        variables.Add("ABC1");
        variables.Add("DEF2");
        variables.Add("GHI5");
        Assert.IsTrue(variables.SetEquals(new Formula("5+abc1+DEF2+gHi5").GetVariables()));
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestGetVariablesWithValidAndInvalidVariables_Invalid()
    {
        HashSet<string> variables = new HashSet<string>();
        variables.Add("ABC1");
        Assert.AreEqual(variables, new Formula("5+abc1+3F2+5hi").GetVariables());
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestGetVariablesWithInvalidVariables_Invalid()
    {
        HashSet<string> variables = new HashSet<string>();
        Assert.AreEqual(variables, new Formula("5+a+F+5h").GetVariables());
    }

    [TestMethod]
    public void Evaluate_TestAddition_CorrectSum()
    {
        Formula formula = new Formula("5+4");
        Assert.AreEqual((double)9, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestLongerAddition_CorrectSum()
    {
        Formula formula = new Formula("5+4+2");
        Assert.AreEqual((double)11, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestVeryLongAddition_CorrectSum()
    {
        Formula formula = new Formula("5+4+2+1+4+5+6+2+5");
        Assert.AreEqual((double)34, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestAdditionWithDecimals_CorrectSum()
    {
        Formula formula = new Formula("5.5+4.3");
        Assert.AreEqual((double)9.8, formula.Evaluate(s => 0));
    }
    [TestMethod]
    public void Evaluate_TestSubtraction_CorrectDifference()
    {
        Formula formula = new Formula("5-4");
        Assert.AreEqual((double)1, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestLongerSubtraction_CorrectDifference()
    {
        Formula formula = new Formula("5-4-1");
        Assert.AreEqual((double)0, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestVeryLongSubtraction_CorrectDifference()
    {
        Formula formula = new Formula("90-10-9-8-7-5-3-1");
        Assert.AreEqual((double)47, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionWithDecimals_CorrectSum()
    {
        Formula formula = new Formula("5.5-4.3");
        Assert.AreEqual(1.2, (double)formula.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_TestMultiplication_CorrectProduct()
    {
        Formula formula = new Formula("5*4");
        Assert.AreEqual((double)20, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestLongerMultiplication_CorrectProduct()
    {
        Formula formula = new Formula("5*4*2*1");
        Assert.AreEqual((double)40, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestMultiplicationWithDecimals_CorrectProduct()
    {
        Formula formula = new Formula("6.6*5.5*1.1");
        Assert.AreEqual(39.93, (double)formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestDivision_CorrectQuotient()
    {
        Formula formula = new Formula("10/5");
        Assert.AreEqual((double)2, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestLongerDivision_CorrectQuotient()
    {
        Formula formula = new Formula("10/5/2/1");
        Assert.AreEqual((double)1, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestDivisionWithDecimals_CorrectProduct()
    {
        Formula formula = new Formula("5.5/6.6");
        Assert.AreEqual(0.833333333333333, (double)formula.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_TestDivisionWithNumeratorZero_CorrectProduct()
    {
        Formula formula = new Formula("0/6.6");
        Assert.AreEqual(0, (double)formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestAdditionAndMutliplication_CorrectCalculation()
    {
        Formula formula = new Formula("5+4*2");
        Assert.AreEqual((double)13, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestAdditionAndDivisionInNumerator_CorrectCalculation()
    {
        Formula formula = new Formula("5+4/2");
        Assert.AreEqual((double)7, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestAdditionAndDivisionInDenominator_CorrectCalculation()
    {
        Formula formula = new Formula("5/5+5");
        Assert.AreEqual((double)6, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionAndMutliplication_CorrectCalculation()
    {
        Formula formula = new Formula("5-4*2");
        Assert.AreEqual((double)-3, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionAndDivisionInNumerator_CorrectCalculation()
    {
        Formula formula = new Formula("5-4/2");
        Assert.AreEqual((double)3, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionAndDivisionInDenominator_CorrectCalculation()
    {
        Formula formula = new Formula("5/5-5");
        Assert.AreEqual((double)-4, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestMultiplcationAndDivisionInNumerator_CorrectCalculation()
    {
        Formula formula = new Formula("5*5/5");
        Assert.AreEqual((double)5, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestMultiplcationAndDivisionInDenominator_CorrectCalculation()
    {
        Formula formula = new Formula("5/5*5");
        Assert.AreEqual((double)5, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParentheses_CorrectCalculation()
    {
        Formula formula = new Formula("(5+4)");
        Assert.AreEqual((double)9, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesWithAddition_CorrectCalculation()
    {
        Formula formula = new Formula("(5+4)+4");
        Assert.AreEqual((double)13, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesWithSubtraction_CorrectCalculation()
    {
        Formula formula = new Formula("9-(20+4)");
        Assert.AreEqual((double)-15, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesWithMultiplcation_CorrectCalculation()
    {
        Formula formula = new Formula("(5+4)*4");
        Assert.AreEqual((double)36, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesInNumeratorWithDivision_CorrectCalculation()
    {
        Formula formula = new Formula("(5-4)/4");
        Assert.AreEqual(0.25, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesInDenominatorWithDivision_CorrectCalculation()
    {
        Formula formula = new Formula("4/(4+4)");
        Assert.AreEqual(0.5, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestLayersOfParentheses_CorrectCalculation()
    {
        Formula formula = new Formula("((20*(4+5))+20)/20");
        Assert.AreEqual((double)10, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestDivideByZero_ReturnFormulaError()
    {
        Formula formula = new Formula("50/0");
        Assert.IsInstanceOfType(formula.Evaluate(s => 0), typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestDivideByCalculatedZero_ReturnFormulaError()
    {
        Formula formula = new Formula("50/(9+8-17)");
        Assert.IsInstanceOfType(formula.Evaluate(s => 0), typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestManyOperationsDividedByZero_ReturnFormulaError()
    {
        Formula formula = new Formula("(5+4)*2-3/(23-23)");
        Assert.IsInstanceOfType(formula.Evaluate(s => 0), typeof(FormulaError));
    }
    [TestMethod]
    public void Evaluate_TestDividingByZeroUsingDecimals_ReturnFormulaError()
    {
        Formula formula = new Formula("(5+4)*2-3/(6.6-6.6)");
        Assert.IsInstanceOfType(formula.Evaluate(s => 0), typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestAdditionWithVariable_CorrectCalculation()
    {
        Formula formula = new Formula("A1+5");
        Assert.AreEqual((double)10, formula.Evaluate(s => 5));
    }

    [TestMethod]
    public void Evaluate_TestAdditionWithOnlyVariables_CorrectCalculation()
    {
        Formula formula = new Formula("A1+B1+C1");

        //The lambda expression defines values for different varibales via 
        // if-else branching.
        Assert.AreEqual((double)22, formula.Evaluate(s =>
        {
            if (s == "A1")
                return 10;

            else if (s == "B1")
                return 5;

            else
                return 7;
        }
        ));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionWithVariable_CorrectCalculation()
    {
        Formula formula = new Formula("A1-9");
        Assert.AreEqual((double)0, formula.Evaluate(s => 9));
    }

    [TestMethod]
    public void Evaluate_TestSubtractionWithOnlyVariables_CorrectCalculation()
    {
        Formula formula = new Formula("A1-B1-C1");

        //The lambda expression defines values for different varibales via 
        // if-else branching.
        Assert.AreEqual((double)-2, formula.Evaluate(s =>
        {
            if (s == "A1")
                return 10;

            else if (s == "B1")
                return 5;

            else
                return 7;
        }
        ));
    }

    [TestMethod]
    public void Evaluate_TestMultiplicationWithVariable_CorrectCalculation()
    {
        Formula formula = new Formula("A1*9");
        Assert.AreEqual((double)81, formula.Evaluate(s => 9));
    }

    [TestMethod]
    public void Evaluate_TestMultiplicationWithOnlyVariables_CorrectCalculation()
    {
        Formula formula = new Formula("A1*B1*C1");

        //The lambda expression defines values for different varibales via 
        // if-else branching.
        Assert.AreEqual((double)350, formula.Evaluate(s =>
        {
            if (s == "A1")
                return 10;

            else if (s == "B1")
                return 5;

            else
                return 7;
        }
        ));
    }

    [TestMethod]
    public void Evaluate_TestDivisionWithVariable_CorrectCalculation()
    {
        Formula formula = new Formula("A1/9");
        Assert.AreEqual((double)1, formula.Evaluate(s => 9));
    }

    [TestMethod]
    public void Evaluate_TestDivisionWithOnlyVariables_CorrectCalculation()
    {
        Formula formula = new Formula("A1/B1/C1");

        //The lambda expression defines values for different varibales via 
        // if-else branching.
        Assert.AreEqual((double)1, formula.Evaluate(s =>
        {
            if (s == "A1")
                return 10;

            else if (s == "B1")
                return 5;

            else
                return 2;
        }
        ));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesWithVariable_CorrectCalculation()
    {
        Formula formula = new Formula("(A1+10)*10/5");
        Assert.AreEqual((double)38, formula.Evaluate(s => 9));
    }

    [TestMethod]
    public void Evaluate_TestParenthesesWithManyVariables_CorrectCalculation()
    {
        Formula formula = new Formula("(A1-B1)/5+(C1+D2)*6");
        Assert.AreEqual((double)25, formula.Evaluate(s =>
        {
            if (s == "A1")
                return 10;

            else if (s == "B1")
                return 5;

            else if (s == "C1")
                return 2;

            else
                return 2;
        }
        ));
    }

    [TestMethod]
    public void Evaluate_TestDivideByZeroWithVariableEqualToZero_ReturnFormulaError()
    {
        Formula formula = new Formula("A1/B1");
        Assert.IsInstanceOfType(formula.Evaluate(s =>
        {
            if (s == "A1")
                return 1;

            else
                return 0;

        })
        , typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestVariableNotFound_ReturnFormulaError()
    {
        Formula formula = new Formula("A1/5");
        Assert.IsInstanceOfType(formula.Evaluate(s => { throw new ArgumentException(); }), typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestManyVaribalesOnlyOneVariableNotFound_ReturnFormulaError()
    {
        Formula formula = new Formula("A1/B1 * C1");
        Assert.IsInstanceOfType(formula.Evaluate(s =>
        {
            if (s == "A1")
                return 5;

            else if (s == "B1")
                return 5;

            else
                throw new ArgumentException();
        }), typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_TestOnlyOneNumber_ReturnOneNumber()
    {
        Formula formula = new Formula("5");
        Assert.AreEqual((double)5, formula.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_TestOnlyOneVariable_ReturnOneNumber()
    {
        Formula formula = new Formula("A1");
        Assert.AreEqual((double)9, formula.Evaluate(s => 9));
    }

    [TestMethod]
    public void Equals_TestTwoEqualFormulae_ReturnTrue()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula1.Equals(formula2));
    }

    [TestMethod]
    public void Equals_TestTwoEqualFormulaeWithDifferentDefintion_ReturnTrue()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5.0");
        Assert.IsTrue(formula1.Equals(formula2));
    }

    [TestMethod]
    public void Equals_TestTwoUnequalFormulae_ReturnFalse()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+6");
        Assert.IsFalse(formula1.Equals(formula2));
    }

    [TestMethod]
    public void Equals_TestFormualWithDifferentObject_ReturnFalse()
    {
        Formula formula1 = new Formula("A1+5");
        string formula2 = "A1+6";
        Assert.IsFalse(formula1.Equals(formula2));
    }

    [TestMethod]
    public void OperatorDoubleEquals_TesEqualFormulae_ReturnTrue()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5");
        Assert.IsTrue(formula1 == formula2);
    }

    [TestMethod]
    public void OperatorDoubleEquals_TestEqualFormulaeWithDifferentDefinition_ReturnTrue()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5.0");
        Assert.IsTrue(formula1 == formula2);
    }

    [TestMethod]
    public void OperatorNotEquals_TestTwoUnequalFormulae_ReturnTrue()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+6");
        Assert.IsTrue(formula1 != formula2);
    }

    [TestMethod]
    public void GetHashCode_TestCorrectHashcode_AreEqual()
    {
        Formula formula = new Formula("A1+5");
        string stringFormula = "A1+5";
        Assert.AreEqual(stringFormula.GetHashCode(), formula.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_TestCorrectHashcodeWithUnformattedFormula_AreEqual()
    {
        Formula formula = new Formula("a1+5");
        string stringFormula = "A1+5";
        Assert.AreEqual(stringFormula.GetHashCode(), formula.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_TestCorrectHashcode_AreNotEqual()
    {
        Formula formula = new Formula("A1+5");
        string stringFormula = "a1+5";
        Assert.IsFalse(stringFormula.GetHashCode() == formula.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_TestCorrectHashcodeForTwoFormulae_AreEqual()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("A1+5"); ;
        Assert.AreEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_TestCorrectHashcodeForFormulaAndUnformattedFormula_AreEqual()
    {
        Formula formula1 = new Formula("A1+5");
        Formula formula2 = new Formula("a1+5.000"); ;
        Assert.AreEqual(formula1.GetHashCode(), formula2.GetHashCode());
    }
}