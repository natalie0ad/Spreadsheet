// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation

/// <summary>
/// Class <c>Spreadsheet/c> 
/// Represents a spreadsheet and consists of functionality for a spreadsheet,
/// such as getting and setting cell values, getting names of non empty cells,
/// saving the Spreadsheet, loading it from a existing file.
/// </summary>
/// <authors> [Rishabh Saini] </authors>
/// <date> [18th October, 2024] </date>

namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Encodings.Web;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    /// Represents the connections between cells. A connection is estabilished 
    /// when a Cell's value depends on another cell.
    /// </summary>
    private DependencyGraph CellConnections;

    /// <summary>
    /// Represents the mapping for a Cell name to its Cell object, which contains 
    /// the Cell's contents and values.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cell> Cells;

    /// <summary>
    /// Contrcuts a Spreadsheet object by initializing the DependencyGraph and Dictionary.
    /// </summary>
    public Spreadsheet()
    {
        CellConnections = new DependencyGraph();
        Cells = new Dictionary<string, Cell>();
    }

    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        ISet<string> NonEmptyCellNames = new HashSet<string>();
        foreach (KeyValuePair<string, Cell> content in Cells)
            NonEmptyCellNames.Add(content.Key.ToUpper());

        return NonEmptyCellNames;
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();

        else if (!Cells.ContainsKey(name))
            return string.Empty;

        return Cells[name].content;
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        string NormalizedName = name.ToUpper();

        if (!IsValidName(NormalizedName))
            throw new InvalidNameException();

        if (!Cells.ContainsKey(NormalizedName))
            Cells.Add(NormalizedName.ToUpper(), new Cell(number));
        else
            Cells[NormalizedName] = new Cell(number);

        IEnumerable<string> newDependees = new HashSet<string>();
        newDependees.Append(number.ToString());
        CellConnections.ReplaceDependees(NormalizedName, newDependees);
        return GetCellsToRecalculate(NormalizedName).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        string NormalizedName = name.ToUpper();
        if (!IsValidName(NormalizedName))
            throw new InvalidNameException();

        if (text != string.Empty)
        {
            if (!Cells.ContainsKey(NormalizedName))
                Cells.Add(NormalizedName.ToUpper(), new Cell(text));
            else
                Cells[NormalizedName] = new Cell(text);
        }

        // If a pre-exisiting non-epty Cell is set to an empty string,
        // remove the cell from the Spreadsheet.
        else if (Cells.ContainsKey(NormalizedName))
                Cells.Remove(NormalizedName.ToUpper());
        
            
        IEnumerable<string> newDependees = new HashSet<string>();
        newDependees.Append(text);
        CellConnections.ReplaceDependees(NormalizedName, newDependees);
        return GetCellsToRecalculate(NormalizedName).ToList();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        string NormalizedName = name.ToUpper();

        //Adding name as a dependent for all the variables in formula.
        CellConnections.ReplaceDependees(NormalizedName, formula.GetVariables());

        //Will throw CircularException if a cicular dependency is found.
        IList<string> CellsToRecalculate;
        try
        {
            CellsToRecalculate = GetCellsToRecalculate(NormalizedName).ToList();
        }
        catch (CircularException)
        {
            //if CircularException is caught remove all the added dependencies
            foreach (string token in formula.GetVariables())
                CellConnections.RemoveDependency(token, NormalizedName);

            throw new CircularException();
        }

        if (!IsValidName(NormalizedName))
            throw new InvalidNameException();

        if (!Cells.ContainsKey(NormalizedName))
            Cells.Add(NormalizedName, new Cell(formula));
        else
            Cells[NormalizedName] = new Cell(formula);

        return CellsToRecalculate;
    }

    
    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return CellConnections.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;

    }

    /// <summary>
    /// Using the depth-first search algorithm to find the dependents of a Cell.
    /// For example, this method start from a cell A1, then recursively goes to its dependent B1, then
    /// again recursively goes to B1's dependent C1, and so on.
    /// </summary>
    /// <param name="start">The name of cell that we are finding the dependents of</param>
    /// <param name="name">The name of the current cell</param>
    /// <param name="visited">This Set contains names of all cells that have been visited.</param>
    /// <param name="changed">This LinkedList contains the names of start cells and its dependents 
    /// in correct order</param>
    /// <exception cref="CircularException">If the Cells depend on each other in an infinite pattern,
    /// throw a CircularException</exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        changed.AddFirst(name);
    }

    /// <summary>
    /// Determines if a cell name is a valid name.
    /// A valid name as one or more letters followed by one or more numbers.
    /// </summary>
    /// <param name="name">The cell name to be checked.</param>
    /// <returns>True if the cell name is valid, false otherwise.</returns>
    private bool IsValidName(string name)
    {
        return Regex.IsMatch(name, @"^[a-zA-Z]+\d+$");
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        get { return GetCellValue(name); }
    }


    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    /// 
    [JsonIgnore]
    public bool Changed { get; private set; }


    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename. 
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        CellConnections = new DependencyGraph();
        Cells = new Dictionary<string, Cell>();
        string FileContents = File.ReadAllText(filename);
        var spreadsheet = new Spreadsheet();

        //Check if deserializing causes any issues.
        try
        {
           spreadsheet = JsonSerializer.Deserialize<Spreadsheet>(FileContents);
        }

        catch(Exception)
        {
            throw new SpreadsheetReadWriteException("A spreadsheet cannot be constructed from the given file.");
        }

        //Check if the spreadsheet results into a null after deserializing, if yes throw exception.
        if (spreadsheet is not null)
        {
            ISet<string> NonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();
            foreach (string cell in NonEmptyCells)
            {
               SetContentsOfCell(cell, spreadsheet.Cells[cell].StringForm);
            }
        }
        else
            throw new SpreadsheetReadWriteException("A spreadsheet cannot be constructed from the given file.");
    }


    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1" 
    ///     with contents being the double 5.0, and a cell "B3" with contents 
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary 
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName 
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} } 
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file, 
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        try
        {
            //Creating a file with the text, or overwriting if the file already exists.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.WriteLine(JsonSerializer.Serialize(this, options));
            }
            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Could not open, write to, or close the file successfully.");
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();

        else if (!Cells.ContainsKey(name))
            return string.Empty;

        return Cells[name].value;
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or 
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the 
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a 
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names 
    ///     of all other cells whose value depends, directly or indirectly, 
    ///     on the named cell. The order of the list should be any order 
    ///     such that if cells are re-evaluated in that order, their dependencies 
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        //If the name is invalid, calling SetCellContents in any of the branches will throw InvalidNameException
        if (double.TryParse(content, out double _content))
        {
            IList<string> dependents = SetCellContents(name, _content);
            Changed = true;
            Cells[name.ToUpper()].value = _content;

            //After setting a number to cell, re-evaluate all the formulas that depend on that cell.
            //Here a for loop is used starting from the 2nd element of the dependents list to prevent
            //a StackOverFlowException.
            for (int i = 1; i < dependents.Count; i++)
                SetContentsOfCell(dependents[i], Cells[dependents[i]].StringForm);

            return dependents;
        }
        else if(content == string.Empty)
        {
            IList<string> dependents = SetCellContents(name, content); 
            return dependents;
        }
        else if (content[0] != '=')
        {
            IList<string> dependents = SetCellContents(name, content);
            Changed = true;
            Cells[name.ToUpper()].value = content;
            return dependents;
        }

        else
        {
            //This 'else' statement is entered only when the formula contains a '= in front
            //Remove the equals sign to make the string a possibly valid formula.
            StringBuilder _formula = new StringBuilder();
            for (int i = 1; i < content.Length; i++)
                _formula.Append(content[i]);

            //Will throw a FormulaFormatException if the content cannnot be parsed into a formula.
            Formula formula = new Formula(_formula.ToString());

            //Will throw a CircularException if the formula contains a circular dependency
            IList<string> dependents = SetCellContents(name, formula);
            Changed = true;

            //After setting a formula, update all of its dependents.
            foreach (string cell in dependents)
                Cells[cell.ToUpper()].value = formula.Evaluate(GetCellValueIfDouble);

            return dependents;
        }
    }

    /// <summary>
    /// Returns the value of the Cells that hold numbers or formulas.
    /// This method uses GetCellValue(), but returns a double in order
    /// to be passed as a delegate to the Evaluate() method in the Formula class,=.
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns>A double which is the value of the Cell holding the double or the formula.</returns>
    /// <exception cref="ArgumentException"></exception>
    private double GetCellValueIfDouble(string cellName)
    {
        if (!Cells.ContainsKey(cellName) || GetCellValue(cellName) is not double)
            throw new ArgumentException();

        return (double)GetCellValue(cellName);
    }

    /// <summary>
    /// A Cell object represents a singular cell in a spreadsheet.
    /// Its contents can either contain a double, a string, or Formula.
    /// It's value can either contain a double, a string, or FormulaError.
    /// </summary>
    private class Cell
    {
        /// <summary>
        /// Represents the content this Cell holds. It has been declared
        /// as an object because the content could wither be a double, a
        /// string or a Formula.
        /// </summary>
        public object content;

        /// <summary>
        ///  Represents the value this Cell holds. It has been declared
        /// as an object because the content could wither be a double, a
        /// string or a FormulaError.
        /// </summary>
        // Here value is initialized outside the constructor, as the value of the Cell will
        // be determined when the cell is set into the Spreadsheet.
        public object value = new object();

        /// <summary>
        /// A string representation of the contents of this Cell.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("StringForm")]
        public string StringForm;


        /// <summary>
        /// Default constructor that sets content to an empty object and StringForm 
        /// to an empty string.
        /// </summary>
        public Cell()
        {
           content = new object();
           StringForm = string.Empty;
        }
        /// <summary>
        /// Constructor for when this Cell holds a double.
        /// </summary>
        /// <param name="_number">The double consisted within this Cell.</param>
        public Cell(double _number)
        {
            content = _number;
            StringForm = _number.ToString();
        }

        /// <summary>
        /// Constructor for when this Cell holds a string.
        /// </summary>
        /// <param name="_text">The string consisted within this Cell.</param>
        public Cell(string _text)
        {
            content = _text;
            StringForm = _text;
        }

        /// <summary>
        /// Constructor for when this Cell holds a Formula.
        /// </summary>
        /// <param name="_formula">The Formula consisted within this Cell.</param>
        public Cell(Formula _formula)
        {
            content = _formula;
            StringForm = "=" + _formula.ToString();
        }
    }

   
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}


