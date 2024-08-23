namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CheckTests {

    [TestMethod]
    public void Check_Dummy() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "dummy", isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual("dummy", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void Check_NullName() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void Check_NameWithWhitespace() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "test ", isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual("test", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void Check_NullTitle1() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: null, name: "name", isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("name", test.Title);
        Assert.AreEqual("name", test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void Check_NullTitle2() {
        var test = Check.FromConfigData(kind: "dummy", target: "", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.AreEqual("DUMMY", test.Kind);
        Assert.AreEqual("", test.Target);
        Assert.AreEqual("dummy", test.Title);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }


    [TestMethod]
    public void Check_ConstructorErrors() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(()  // space in name
            => Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "te st", isVisible: false, isBreak: false, CheckProfile.Default));
        Assert.ThrowsException<ArgumentOutOfRangeException>(()  // empty name
            => Check.FromConfigData(kind: "dummy", target: "", title: "Dummy", name: "", isVisible: false, isBreak: false, CheckProfile.Default));
    }

}