namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CheckPropertiesTests {

    [TestMethod]
    public void CheckProperties_Basic() {
        var props = new CheckProperties(
            "dummy",              // Kind
            null,                 // Target
            "Dummy",              // Title
            "dummy",              // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("Dummy", props.Title);
        Assert.AreEqual("dummy", props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }

    [TestMethod]
    public void CheckProperties_NullName() {
        var props = new CheckProperties(
            "dummy",              // Kind
            "",                   // Target
            "Dummy",              // Title
            null,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("Dummy", props.Title);
        Assert.AreEqual(null, props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }

    [TestMethod]
    public void CheckProperties_NameWithWhitespace() {
        var props = new CheckProperties(
            "dummy",              // Kind
            "",                   // Target
            "Dummy",              // Title
            "test ",              // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("Dummy", props.Title);
        Assert.AreEqual("test", props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }

    [TestMethod]
    public void CheckProperties_NullTitle1() {
        var props = new CheckProperties(
            "dummy",              // Kind
            "",                   // Target
            null,                 // Title
            "name",               // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("name", props.Title);
        Assert.AreEqual("name", props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }

    [TestMethod]
    public void CheckProperties_NullTitle2() {
        var props = new CheckProperties(
            "dummy",              // Kind
            "",                   // Target
            null,                 // Title
            null,                 // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("dummy", props.Title);
        Assert.AreEqual(null, props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }


    [TestMethod]
    public void CheckProperties_SpaceInName() {
        var props = new CheckProperties(
            "dummy",              // Kind
            "",                   // Target
            null,                 // Title
            "na me",              // Name
            false,                // IsVisible
            false,                // IsBreak
            null,                 // PercentThreshold
            CheckProfile.Default  // Profile
        );
        Assert.AreEqual("DUMMY", props.Kind);
        Assert.AreEqual("", props.Target);
        Assert.AreEqual("na me", props.Title);
        Assert.AreEqual(null, props.Name);
        Assert.AreEqual(CheckProfile.Default, props.CheckProfile);
    }

}