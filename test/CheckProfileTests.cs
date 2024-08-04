namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CheckProfileTests {

    [TestMethod]
    public void Basic() {
        var test = new CheckProfile(TimeSpan.FromSeconds(13), TimeSpan.FromSeconds(7), 5, 1);
        Assert.AreEqual(TimeSpan.FromSeconds(13), test.Period);
        Assert.AreEqual(TimeSpan.FromSeconds(7), test.Timeout);
        Assert.AreEqual(5, test.SuccessCount);
        Assert.AreEqual(1, test.FailureCount);
    }


    [TestMethod]
    public void ConstructorErrors() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(0.999), TimeSpan.FromSeconds(5), 3, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(600.001), TimeSpan.FromSeconds(5), 3, 3));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0.099), 3, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(60.001), 3, 3));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), 0, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), 11, 3));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), 3, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CheckProfile(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), 3, 11));
    }

}