namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;
using System.Security.Cryptography;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests.
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    ///     This test adds a many dependencies to a DependencyGraph and stress tests  
    ///     the class by adding, removing and then again adding dependencies and then 
    ///     asserts that the methods work as expected. The tests keep track of the 
    ///     expected outcome by updating elements two HashSet arrays,and then
    ///     comparing the elements in the HashSet arrays to the elements in the 
    ///     DependencyGraph.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]  // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new
            HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new
            HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    ///     This tests adds a hundred elements in a DependencyGraph 
    ///     and checks to if Size returns the correct number of elements
    ///     in the graph.
    /// </summary>
    [TestMethod]
    public void Size_Test100Elements_EqualTo100()
    {
        DependencyGraph dg = new DependencyGraph();
        for (int i = 0; i < 100; i++)
            dg.AddDependency(('a' + i).ToString(), ('a' + (i + 1)).ToString());

        Assert.AreEqual(100, dg.Size);
    }


    [TestMethod]
    public void Size_TestNoElements_EqualToZero()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.AreEqual(0, dg.Size);
    }

    [TestMethod]
    public void HasDependents_TestOneDependent_True()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());
        Assert.IsTrue(dg.HasDependents(('a' + 1).ToString()));
    }

    /// <summary>
    ///     This tests adds a hundred elements in a DependencyGraph with
    ///     only one dependee and checks if HasDependents() correctly 
    ///     return true.
    /// </summary>
    [TestMethod]
    public void HasDependents_Test100Dependents_True()
    {
        DependencyGraph dg = new DependencyGraph();
        for (int i = 0; i < 100; i++)
            dg.AddDependency(('a' + 1).ToString(), ('a' + (i + 1)).ToString());

        Assert.IsTrue(dg.HasDependents(('a' + 1).ToString()));
    }

    [TestMethod]
    public void HasDependents_TestNoDependents_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());

        Assert.IsFalse(dg.HasDependents(('a' + 2).ToString()));
    }

    [TestMethod]
    public void HasDependees_TestOneDependee_True()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());

        Assert.IsTrue(dg.HasDependees(('a' + 2).ToString()));
    }

    /// <summary>
    ///     This tests adds a hundred elements in a DependencyGraph with
    ///     only one dependent and checks if HasDependees() correctly 
    ///     return true.
    /// </summary>
    [TestMethod]
    public void HasDependees_Test100Dependees_True()
    {
        DependencyGraph dg = new DependencyGraph();
        for (int i = 0; i < 100; i++)
            dg.AddDependency(('a' + i).ToString(), ('a' + 1).ToString());

        Assert.IsTrue(dg.HasDependees(('a' + 1).ToString()));
    }

    [TestMethod]
    public void HasDependees_TestNoDependees_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());

        Assert.IsFalse(dg.HasDependees(('a' + 1).ToString()));
    }

    [TestMethod]
    public void GetDependents_TestOneDependent_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());

        HashSet<string> dependents = new HashSet<string>();
        dependents.Add(('a' + 2).ToString());

        Assert.IsTrue(dependents.SetEquals(dg.GetDependents(('a' + 1).ToString())));
    }

    /// <summary>
    ///     This tests adds a hundred elements in a DependencyGraph with
    ///     only one dependee and checks to see if the GetDependents() method
    ///     correctly returns the expected HashSet. The expected HashSet is 
    ///     kept track of by adding the expected elements into another HashSet, 
    ///     and then comparing it to the result of GetDependents().
    /// </summary>
    [TestMethod]
    public void GetDependents_Test100Dependents_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependents = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            dg.AddDependency(('a' + 1.ToString()), ('a' + (i + 1).ToString()));
            dependents.Add(('a' + (i + 1).ToString()));
        }
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents(('a' + 1.ToString()))));
    }

    [TestMethod]
    public void GetDependents_TestNoDependents_SetsAreNotEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependents = new HashSet<string>();
        dg.AddDependency(('a' + 1.ToString()), ('a' + 2.ToString()));
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents(('a' + 2.ToString()))));
    }

    [TestMethod]
    public void GetDependees_TestOneDependee_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency(('a' + 1).ToString(), ('a' + 2).ToString());

        HashSet<string> dependees = new HashSet<string>();
        dependees.Add(('a' + 1).ToString());

        Assert.IsTrue(dependees.SetEquals(dg.GetDependees(('a' + 2).ToString())));
    }

    /// <summary>
    ///     This tests adds a hundred elements in a DependencyGraph with
    ///     only one dependent and checks to see if the GetDependees() method
    ///     correctly returns the expected HashSet. The expected HashSet is 
    ///     kept track of by adding the expected elements into another HashSet, 
    ///     and then comparing it to the result of GetDependees().
    /// </summary>
    [TestMethod]
    public void GetDependees_Test100Dependees_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependees = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            dg.AddDependency(('a' + (i + 1).ToString()), ('a' + 1.ToString()));
            dependees.Add(('a' + (i + 1).ToString()));
        }
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees(('a' + 1.ToString()))));
    }

    [TestMethod]
    public void GetDependees_TestNoDependees_SetsAreNotEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency(('a' + 1.ToString()), ('a' + 2.ToString()));
        Assert.IsTrue(dependees.SetEquals(dg.GetDependees(('a' + 1.ToString()))));
    }

    /// <summary>
    ///     This test adds a one element to the DependencyGraph and checks
    ///     to see if the the AddDependency() method correctly adds dependents and
    ///     dependees to the graph.
    /// </summary>
    [TestMethod]
    public void AddDependency_TestAddingOne_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependents = new HashSet<string>();
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("a1", "b1");
        dependents.Add("b1");
        dependees.Add("a1");
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("a1")) &&
                      dependees.SetEquals(dg.GetDependees("b1")));
    }

    /// <summary>
    ///    This test adds two elements to the DependencyGraph followed by removing
    ///    one element, and then checks to see if the element was successfullly
    ///    removed.
    /// </summary>
    [TestMethod]
    public void RemoveDependency_TestRemovingOne_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> dependents = new HashSet<string>();
        HashSet<string> dependees = new HashSet<string>();
        dg.AddDependency("a1", "b1");
        dg.AddDependency("a2", "b2");
        dependents.Add("b1");
        dependees.Add("a1");
        dg.RemoveDependency("a2", "b2");
        Assert.IsTrue(dependents.SetEquals(dg.GetDependents("a1")) &&
                      dependees.SetEquals(dg.GetDependees("b1")));
    }

    /// <summary>
    ///     This test adds two ordered pairs with the same dependee and then changes 
    ///     the dependents of that dependee using the ReplaceDependents() method, and then
    ///     checks to see if the method replaces the data correctly.
    /// </summary>
    [TestMethod]
    public void ReplaceDependents_TestReplacingTwo_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        //Add two dependencies
        dg.AddDependency("a1", "b1");
        dg.AddDependency("a1", "b2");

        //Create new data
        HashSet<string> newDependents = new HashSet<string>();
        newDependents.Add("c1");
        newDependents.Add("c2");

        dg.ReplaceDependents("a1", newDependents);

        HashSet<string> dependees = new HashSet<string>();
        //Add to the singular dependee to a set
        dependees.Add("a1");

        //Check if the dependents of a1 were updated and dependee of c1 and c2
        //is now a1
        Assert.IsTrue(newDependents.SetEquals(dg.GetDependents("a1")) &&
                      dependees.SetEquals(dg.GetDependees("c1")) &&
                      dependees.SetEquals(dg.GetDependees("c2")));
    }


    /// <summary>
    ///     This test adds two ordered pairs with the same dependent and then changes 
    ///     the dependees of that dependent using the ReplaceDependees() method, and then
    ///     checks to see if the method replaces the data correctly.
    /// </summary>
    [TestMethod]
    public void ReplaceDependees_TestReplacingTwo_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        //Add two dependencies
        dg.AddDependency("a1", "b1");
        dg.AddDependency("a2", "b1");

        //Create new data
        HashSet<string> newDependees = new HashSet<string>();
        newDependees.Add("c1");
        newDependees.Add("c2");

        dg.ReplaceDependees("b1", newDependees);

        //Add to the singular dependent to a set
        HashSet<string> dependents = new HashSet<string>();
        dependents.Add("b1");

        //Check if the dependees of b1 were updated and dependent of c1 and c2
        //is now b1
        Assert.IsTrue(newDependees.SetEquals(dg.GetDependees("b1")) &&
                      dependents.SetEquals(dg.GetDependents("c1")) &&
                      dependents.SetEquals(dg.GetDependents("c2")));
    }

    /// <summary>
    ///     This test adds 100 ordered pairs with the same dependee and then changes 
    ///     the dependents of that dependee using the ReplaceDependents() method, and then
    ///     checks to see if the method replaces the data correctly.
    /// </summary>
    [TestMethod]
    public void ReplaceDependents_TestReplacing100_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> newDependents = new HashSet<string>();
        HashSet<string> dependees = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            //Add a 100 dependencies with same dependee
            dg.AddDependency("a0", "a" + (i + 1).ToString());

            //Create new data
            newDependents.Add("b" + (i + 1).ToString());
        }
        //Add the singular dependee to a HashSet
        dependees.Add("a0");

        dg.ReplaceDependents("a0", newDependents);

        //Check if the dependents for a0 were updated
        Assert.IsTrue(newDependents.SetEquals(dg.GetDependents("a0")));

        //Check if the dependee for the new dependents was updated to a0
        foreach (string dependent in newDependents)
        {
            Assert.IsTrue(dependees.SetEquals(dg.GetDependees(dependent)));

        }
    }

    /// <summary>
    ///     This test adds 100 ordered pairs with the same dependent and then changes 
    ///     the dependees of that dependee=nt using the ReplaceDependees() method, and then
    ///     checks to see if the method replaces the data correctly.
    /// </summary>
    [TestMethod]
    public void ReplaceDependees_TestReplacing100_SetsAreEqual()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> newDependees = new HashSet<string>();
        HashSet<string> dependents = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            //Add a 100 dependencies with same dependee
            dg.AddDependency("a" + (i + 1).ToString(), "a0");

            //Create new data
            newDependees.Add("b" + (i + 1).ToString());
        }
        //Add the singular dependent to a HashSet
        dependents.Add("a0");

        dg.ReplaceDependees("a0", newDependees);

        //Check if the dependees for a0 were updated
        Assert.IsTrue(newDependees.SetEquals(dg.GetDependees("a0")));

        //Check if the dependent for the new dependents was updated to a0
        foreach (string dependent in newDependees)
        {
            Assert.IsTrue(dependents.SetEquals(dg.GetDependents(dependent)));

        }
    }
}