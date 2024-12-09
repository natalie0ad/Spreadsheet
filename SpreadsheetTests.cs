namespace SpreadsheetTests;

using CS3500.Spreadsheet;
using CS3500.Formula;
using System.Security.Cryptography;
using System.Diagnostics;

/// <summary>
///   This is a test class for Spreadsheet and is intended
///   to contain all Spreadsheet Unit Tests.
/// </summary>
/// <authors> [Rishabh Saini] </authors>
/// <date> [18th October, 2024] </date>

[TestClass]
public class SpreadsheetTests
{
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_SetFiveCells_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("B1", "6");
        s.SetContentsOfCell("C1", "7");
        s.SetContentsOfCell("D1", "8");
        s.SetContentsOfCell("E1", "hello");

        ISet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("C1");
        expected.Add("D1");
        expected.Add("E1");

        Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void GetNamesOfAllNonemptyCells_SetCellsWithReassignment_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("B1", "6");
        s.SetContentsOfCell("A1", "7");

        ISet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B1");

        Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void GetNamesOfAllNonemptyCells_SetOneCellFiveTimes_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("A1", "8");
        s.SetContentsOfCell("A1", "9");
        s.SetContentsOfCell("A1", "7");


        ISet<string> expected = new HashSet<string>();
        expected.Add("A1");

        Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void GetNamesOfAllNonemptyCells_SetNoCells_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<string> expected = new HashSet<string>();

        Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void GetNamesOfAllNonemptyCells_SetCellsWithNotNormalizedNames_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
        s.SetContentsOfCell("b1", "6");
        s.SetContentsOfCell("c1", "7");

        ISet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("C1");

        Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_InvalidName_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.GetCellContents("1A");

    }

    [TestMethod]
    public void GetCellContents_EmptyCell_ReturnEmptyString()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.AreEqual(string.Empty, s.GetCellContents("B1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_NameWithWhitespace_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A 1", "5");
    }

    [TestMethod]
    public void GetCellContents_SetAndGetNumber_SuccessfullySet()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");

        Assert.AreEqual(5.0, s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContents_SetAndGetNumberThrice_SuccessfullySet()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("A1", "10");
        s.SetContentsOfCell("A1", "78");

        Assert.AreEqual(78.0, s.GetCellContents("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_SetAndNumberInvalidName_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "5");
    }

    [TestMethod]
    public void SetCellContents_SetLowercaseNameWithNumber_NoException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
    }

    [TestMethod]
    public void GetCellContents_SetAndGetText_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "dog");

        Assert.AreEqual("dog", s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContents_SetAndGetTextThrice_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "dog");
        s.SetContentsOfCell("A1", "cat");
        s.SetContentsOfCell("A1", "ball");

        Assert.AreEqual("ball", s.GetCellContents("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_SetTextWithInvalidName_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "okay");
    }

    [TestMethod]
    public void SetCellContents_SetLowercaseNameWithText_NoException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "cs3500");
    }

    [TestMethod]
    public void GetCellContents_SetAndGetFormula_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        Formula formula = new Formula("5+4");
        s.SetContentsOfCell("A1", "=5+4");

        Assert.AreEqual(formula, s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContents_SetFormulaTwice_SuccessfullySet()
    {
        Spreadsheet s = new Spreadsheet();
        Formula formula1 = new Formula("5+4");
        Formula formula2 = new Formula("5+9");
        s.SetContentsOfCell("A1", "=5+4");
        s.SetContentsOfCell("A1", "=5+9");

        Assert.AreEqual(formula2, s.GetCellContents("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_SetFormulaWithInvalidName_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "=5+5");
    }

    [TestMethod]
    public void SetCellContents_SetLowercaseNameWithFormula_NoException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("b1", "=5*9");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_SetFormulaWithCircularDependency_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=B1");
        s.SetContentsOfCell("B1", "=A1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_SetLongerFormulaeWithCircularDependency_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=5+C1");
        s.SetContentsOfCell("B1", "=5+A1");
        s.SetContentsOfCell("C1", "=5+B1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_SetVeryLongFormulaWithCircularDependency_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=C1");
        s.SetContentsOfCell("B1", "=5");
        s.SetContentsOfCell("C1", "=5+B1");
        s.SetContentsOfCell("D1", "=5+A1+B1+C1");
        s.SetContentsOfCell("E1", "=G1");
        s.SetContentsOfCell("F1", "=E1+5");
        s.SetContentsOfCell("G1", "=F1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_SetFormulaContainingItself_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A1");
    }

    [TestMethod]
    public void SetCellContents_CircularDependencyDoesNotChangeGraph_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=C1");
            s.SetContentsOfCell("C1", "=D1");
            s.SetContentsOfCell("C1", "=A1");
        }
        catch (CircularException)
        {
            Assert.AreEqual(new Formula("D1"), s.GetCellContents("C1"));
        }
    }

    [TestMethod]
    public void SetCellContents_SetNumber_ReturnEmptyList()
    {
        Spreadsheet s = new Spreadsheet();

        //Ensures that the methods return a List with only one element which is the cell name.
        Assert.AreEqual("A1", s.SetContentsOfCell("A1", "5")[0]);
        Assert.AreEqual(1, s.SetContentsOfCell("A1", "=5").Count);
    }

    [TestMethod]
    public void SetCellContents_SetText_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();

        //Ensures that the methods return a List with only one element which is the cell name.
        Assert.AreEqual("A1", s.SetContentsOfCell("A1", "dog")[0]);
        Assert.AreEqual(1, s.SetContentsOfCell("A1", "dog").Count);
    }

    [TestMethod]
    public void SetCellContents_SetFormula_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1*2");
        s.SetContentsOfCell("C1", "=B1+A1");

        IList<string> expected = new List<string>();
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("C1");

        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("A1", "5")));
    }

    [TestMethod]
    public void SetCellContents_SetFormulaWithLowercaseCellNames_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("b1", "=a1*2");
        s.SetContentsOfCell("c1", "=b1+2");

        IList<string> expected = new List<string>();
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("C1");

        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("a1", "6")));
    }

    [TestMethod]
    public void SetCellContents_SetManyCells_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=a1+56");
        s.SetContentsOfCell("c1", "dog");
        s.SetContentsOfCell("D1", "=c1");
        s.SetContentsOfCell("e1", "=B1+2");
        s.SetContentsOfCell("F1", "=b1+76");
        s.SetContentsOfCell("g1", "=F1+2");

        IList<string> expected = new List<string>();
        //We expect E1 to be last, as it has no dependents.
        //The order of evaluation goes from most dependents to least dependents
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("F1");
        expected.Add("G1");
        expected.Add("E1");

        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("a1", "6")));
    }

    //SetContentsOfCell TESTS
    [TestMethod]
    public void SetContentsOfCell_SetNumber_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<string> expected = new HashSet<string>();
        expected.Add("B1");
        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("B1", "6")));
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void SetContentsOfCell_SetContent_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<string> expected = new HashSet<string>();
        expected.Add("B1");
        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("B1", "cat")));
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void SetContentsOfCell_SetEmptyString_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<string> expected = new HashSet<string>();
        expected.Add("B1");
        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("B1", "")));
        Assert.AreEqual("", s.GetCellValue("B1"));
        Assert.IsFalse(s.Changed);
    }

    [TestMethod]
    public void SetContentsOfCell_SetFormulaWithoutEqualsSign_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<string> expected = new HashSet<string>();
        expected.Add("B1");
        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("B1", "A1+B1")));
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_SetInvalidFormula_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=+A1+B1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_SetCircularFormula_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1+5");
        s.SetContentsOfCell("A1", "=B1+5");
    }

    [TestMethod]
    public void SetContentsOfCell_SetCircularFormula_StatusUnchanged()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("B1", "=A1+5");
            s.SetContentsOfCell("A1", "=B1+5");
        }
        catch (CircularException)
        {
            Assert.AreEqual(string.Empty, s.GetCellContents("A1"));
        }
    }

    [TestMethod]
    public void SetContentsOfCell_SetFormulaContainingItself_StatusUnchanged()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("B1", "=B1");
        }
        catch (CircularException)
        {
            Assert.IsFalse(s.Changed);
        }
    }

    [TestMethod]
    public void SetContentsOfCell_SetValidFormula_ReturnCorrectDependents()
    {
        Spreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("B1", "=A1+5");
        s.SetContentsOfCell("C1", "=B1+5");
        s.SetContentsOfCell("D1", "=C1+5");

        ISet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B1");
        expected.Add("C1");
        expected.Add("D1");

        Assert.IsTrue(expected.SequenceEqual(s.SetContentsOfCell("A1", "=9*7")));
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_SetInvalidNameForNumber_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "5");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_SetInvalidNameForText_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "dog");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_SetInvalidNameForFormula_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "=A1+B1");
    }


    [TestMethod]
    public void SetContentsOfCell_ChangingACellWithManyDependents_DependentsAreUpdated()
    {
        Spreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("B1", "=A1+5");
        s.SetContentsOfCell("C1", "=A1+6");
        s.SetContentsOfCell("D1", "=C1+7");

        ISet<string> expected1 = new HashSet<string>();
        expected1.Add("A1");
        expected1.Add("B1");
        expected1.Add("C1");
        expected1.Add("D1");
        Assert.IsTrue(expected1.SetEquals(s.SetContentsOfCell("A1", "5")));
        Assert.AreEqual((double)18, s.GetCellValue("D1"));

        s.SetContentsOfCell("A1", "7");
        Assert.AreEqual((double)20, s.GetCellValue("D1"));

        s.SetContentsOfCell("C1", "5");
        ISet<string> expected2 = new HashSet<string>();
        expected2.Add("A1");
        expected2.Add("B1");
        Assert.IsTrue(expected2.SetEquals(s.SetContentsOfCell("A1", "7")));
        Assert.AreEqual((double)12, s.GetCellValue("D1"));
    }

    [TestMethod]
    public void SetContentsOfCell_ManyChangesToCells_DependentsAreUpdated()
    {
        Spreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("D1", "10");
        s.SetContentsOfCell("B1", "=A1");
        s.SetContentsOfCell("C1", "=B1");

        s.SetContentsOfCell("A1", "7");
        Assert.AreEqual((double)7, s.GetCellValue("B1"));

        s.SetContentsOfCell("B1", "=D1");
        Assert.AreEqual((double)10, s.GetCellValue("C1"));
    }

    //GetCellValue TESTS

    [TestMethod]
    public void GetCellValue_NumberInCell_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "5");
        Assert.AreEqual((double)5, s.GetCellValue("B1"));
    }

    [TestMethod]
    public void GetCellValue_UseIndexer_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=5/5 + 2*10");
        Assert.AreEqual((double)21, s["B1"]);
    }

    [TestMethod]
    public void GetCellValue_TextInCell_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "dog");
        Assert.AreEqual("dog", s.GetCellValue("B1"));
    }

    [TestMethod]
    public void GetCellValue_FormulaInCell_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=5+6*7");
        Assert.AreEqual((double)47, s.GetCellValue("B1"));
    }

    [TestMethod]
    public void GetCellValue_MultipleFormulasInCell_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=5+6*7");
        s.SetContentsOfCell("C1", "=B1+3");
        Assert.AreEqual((double)50, s.GetCellValue("C1"));
    }
    [TestMethod]
    public void GetCellValue_DivisionByZero_ReturnsFormulaError()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=5/0");
        Assert.IsTrue(s.GetCellValue("A1") is FormulaError);
    }

    [TestMethod]
    public void GetCellValue_InvalidFormula_ReturnsFormulaError()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=B1+1");
        Assert.IsTrue(s.GetCellValue("A1") is FormulaError);
    }

    [TestMethod]
    public void GetCellValue_EvaluateAString_ReturnsFormulaError()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "dog");
        s.SetContentsOfCell("B1", "=A1+1");
        Assert.IsTrue(s.GetCellValue("B1") is FormulaError);
    }

    [TestMethod]
    public void GetCellValue_LotsOfEvaluation_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("A2", "=A1+6");
        s.SetContentsOfCell("A3", "=A2/11");
        s.SetContentsOfCell("A4", "=A3*100");
        s.SetContentsOfCell("A5", "=(A4+A3)*2");
        s.SetContentsOfCell("A6", "=(A5+1)/203");
        Assert.AreEqual((double)1, s.GetCellValue("A6"));
    }

    [TestMethod]
    public void GetCellValue_EmptyCell_ReturnsEmptyString()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.AreEqual(string.Empty, s.GetCellValue("C1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValue_InvalidName_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.GetCellValue("1A");
    }

    //SAVE Tests

    [TestMethod]
    public void Save_SaveToANewFile_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B3", "=A1+2");
        s.SetContentsOfCell("C4", "hello");
        s.Save("SaveToANewFile.txt");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_PassAEmptyString_ThrowException()
    {
        Spreadsheet s = new Spreadsheet();
        s.Save("");
    }

    [TestMethod]
    public void Save_SaveWithCircularDependency_NoChangeMade()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("A1", "=A1");
        }
        catch (CircularException)
        {
            s.Save("CircularDependency.txt");
        }
    }

    [TestMethod]
    public void Save_SaveToAlreadyExisitingFile_OverwriteSucessfully()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B3", "=A1+2");
        s.SetContentsOfCell("C4", "hello");
        s.SetContentsOfCell("D4", "4");
        s.Save("SaveToANewFile.txt");
    }

    [TestMethod]
    public void Save_SaveEmptySpreadsheet_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.Save("EmptySpreadsheet.txt");
    }

    [TestMethod]
    public void Save_SaveSpreadSheetTwice_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.IsTrue(s.Changed);
        s.Save("TwiceSave.txt");
        Assert.IsFalse(s.Changed);
        s.SetContentsOfCell("A1", "6");
        Assert.IsTrue(s.Changed);
        s.Save("TwiceSave.txt");
        Assert.IsFalse(s.Changed);
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_SaveToNonExistentPath_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.Save("/some/nonsense/path.txt");
    }
    //LOAD Tests

    [TestMethod]
    public void Load_LoadASpreadsheetFromAFile_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B3", "=A1+2");
        s.SetContentsOfCell("C4", "hello");
        s.Save("LoadASpreadsheetFromAFile.txt");

        string contents = File.ReadAllText("LoadASpreadsheetFromAFile.txt");
        File.WriteAllText("SavedFile.txt", contents);
        Spreadsheet s2 = new Spreadsheet("SavedFile.txt");
        Assert.AreEqual((double)6, s2.GetCellContents("A1"));
        Assert.AreEqual(new Formula("A1+2"), s2.GetCellContents("B3"));
        Assert.AreEqual("hello", s2.GetCellContents("C4"));
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_LoadEmptyFile_Successful()
    {
        string contents = "";
        File.WriteAllText("EmptyFile.txt", contents);
        Spreadsheet s2 = new Spreadsheet("EmptyFile.txt");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_LoadNullFile_ThrowExceptions()
    {
        string contents = "null";
        File.WriteAllText("NullFile.txt", contents);
        Spreadsheet s2 = new Spreadsheet("NullFile.txt");
    }

    [TestMethod]
    public void Load_LoadValidJSON_Successful()
    {
        string contents = "{\"Cells\": { \"A1\": { \"StringForm\": \"6\" } } }";
        Debug.WriteLine(contents);
        File.WriteAllText("ValidJSON.txt", contents);
        Spreadsheet s2 = new Spreadsheet("ValidJSON.txt");
        Assert.AreEqual((double)6, s2.GetCellContents("A1"));
    }


    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_JSONWithoutCorrectCurlyBraces_ThrowException()
    {
        string contents = "{\"Cells\": { \"A1\": \"StringForm\": \"6\" } ";
        File.WriteAllText("InvalidJSON.txt", contents);
        Spreadsheet s2 = new Spreadsheet("InvalidJSON.txt");
    }

    [TestMethod]
    public void Load_MutipleSavesAndLoads_Successful()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B3", "=A1+2");
        s.SetContentsOfCell("C4", "hello");
        s.Save("MutipleSavesAndLoads.txt");

        string contents = File.ReadAllText("LoadASpreadsheetFromAFile.txt");
        File.WriteAllText("MutipleSavesAndLoads2.txt", contents);
        Spreadsheet s2 = new Spreadsheet("SavedFile.txt");
        s2.SetContentsOfCell("A1", "");
        s2.SetContentsOfCell("A2", "9000");

        s2.Save("MutipleSavesAndLoads.txt");
    }

    [TestMethod]
    public void StressTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A0", "0");
        for (int i = 1; i < 500; i++)
            s.SetContentsOfCell("A" + i, "=A" + (i - 1) + " + 1");

        s.Save("StressTest.txt");
        for (int i = 0; i < 250; i++)
            s.SetContentsOfCell("A" + i, "");

        string contents = File.ReadAllText("StressTest.txt");
        File.WriteAllText("StressTest2.txt", contents);

        Spreadsheet s1 = new Spreadsheet("StressTest.txt");
        for (int i = 250; i < 500; i++)
        {
            Assert.AreEqual((double)i, s1.GetCellValue("A" + i));
        }
    }
}
