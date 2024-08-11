namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CheckTests {

    [TestMethod]
    public void Dummy() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "dummy", CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual("dummy", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullName() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: null, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NameWithWhitespace() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "test ", CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual("test", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullTitle1() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: null, name: "name", CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("name", test.Title);
        Assert.AreEqual("name", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullTitle2() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: null, name: null, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("dummy", test.Title);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }


    [TestMethod]
    public void ConstructorErrors() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(()  // space in name
            => Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "te st", CheckProfile.Default));
        Assert.ThrowsException<ArgumentOutOfRangeException>(()  // empty name
            => Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "", CheckProfile.Default));
    }

}