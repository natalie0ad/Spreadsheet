// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024
// <authors> [Rishabh Saini] </authors>
// <date> [18th September, 2024] </date>

using System.ComponentModel;
using System.Drawing;

namespace CS3500.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    ///   Represents a Dictionary with the dependee as the key and the set of dependents as the value.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependents;

    /// <summary>
    ///   Represents a Dictionary with the depenedent as the key and the set of dependees as the value.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependees;

    /// <summary>
    ///   Represents the number of ordered pairs in the DependencyGraph.
    /// </summary>
    private int size;


    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        dependents = new();
        dependees = new();
        size = 0;
    }

    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return size; }
    }

    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        if (dependents.ContainsKey(nodeName))

            //If the HashSet mapped to nodeName in depedents is not a empty set, return true
            return !(dependents[nodeName].SetEquals(new HashSet<string>()));

        else
            return false;
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        if(dependees.ContainsKey(nodeName)) 

        //If the HashSet mapped to nodeName in depedees is not a empty set, return true
            return !(dependees[nodeName].SetEquals(new HashSet<string>()));

        else
            return false;
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (dependents.ContainsKey(nodeName))
            return dependents[nodeName];

        else
            return new HashSet<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if(dependees.ContainsKey(nodeName))   
             return dependees[nodeName];

        else
            return new HashSet<string>();
    }

    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        //If the dependee does not exist in the Dictionary, add a new key.
        if (!dependents.ContainsKey(dependee))
        {
            HashSet<string> newDependents = new HashSet<string>();
            newDependents.Add(dependent);
            if (dependents.TryAdd(dependee, newDependents))
                size++;
        }

        //If the dependee already exists, update its value.
        else if (dependents[dependee].Add(dependent))
            size++;
        
        //Adds the strings containing no dependents into the Dictionary and maps them to an empty set.
        if (!dependents.ContainsKey(dependent))
            dependents.Add(dependent, new HashSet<string>());

        //If the dependent does not exist in the Dictionary, add a new key.
        if (!dependees.ContainsKey(dependent))
        {
            HashSet<string> newDependees = new HashSet<string>();
            newDependees.Add(dependee);
            dependees.Add(dependent, newDependees);

        }

        //If the dependent already exists, update its value.
        else
            dependees[dependent].Add(dependee);

        //Adds the strings containing no dependees into the Dictionary and maps them to an empty set.
        if (!dependees.ContainsKey(dependee))
            dependees.Add(dependee, new HashSet<string>());

        
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists.
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        if (dependents.ContainsKey(dependee) &&
           (dependents[dependee].Remove(dependent) && dependees[dependent].Remove(dependee)))
            size--;
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced </param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        foreach (string dependent in GetDependents(nodeName))
            RemoveDependency(nodeName, dependent);


        foreach (string newDependent in newDependents)
            AddDependency(nodeName, newDependent);

    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        foreach (string dependee in GetDependees(nodeName))
            RemoveDependency(dependee, nodeName);


        foreach (string newDependent in newDependees)
            AddDependency(newDependent, nodeName);
    }
}
