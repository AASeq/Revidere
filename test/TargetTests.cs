namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class TargetTests {

    [TestMethod]
    public void Dummy() {
        var test = new Target("dummy", "Dummy", new Uri("dummy://localhost"), CheckProfile.Default);
        Assert.AreEqual("dummy", test.Name);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual(new Uri("dummy://localhost"), test.TargetUri);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullName() {
        var test = new Target(null, "Dummy", new Uri("dummy://localhost"), CheckProfile.Default);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual("Dummy", test.Title);
        Assert.AreEqual(new Uri("dummy://localhost"), test.TargetUri);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullTitle1() {
        var test = new Target("dummy", null, new Uri("dummy://localhost"), CheckProfile.Default);
        Assert.AreEqual("dummy", test.Name);
        Assert.AreEqual("dummy", test.Title);
        Assert.AreEqual(new Uri("dummy://localhost"), test.TargetUri);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }

    [TestMethod]
    public void NullTitle2() {
        var test = new Target(null, null, new Uri("dummy://localhost"), CheckProfile.Default);
        Assert.AreEqual(null, test.Name);
        Assert.AreEqual("", test.Title);
        Assert.AreEqual(new Uri("dummy://localhost"), test.TargetUri);
        Assert.AreEqual(CheckProfile.Default, test.CheckProfile);
    }


    [TestMethod]
    public void ConstructorErrors() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(  // space in name
            () => new Target("dummy ", "Dummy", new Uri("dummy://localhost"), CheckProfile.Default));
        Assert.ThrowsException<ArgumentOutOfRangeException>(()  // empty name
            => new Target("", "Dummy", new Uri("dummy://localhost"), CheckProfile.Default));
    }

}