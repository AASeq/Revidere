namespace Tests;

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class RandomCheckerTests {

    [TestMethod]
    public void Basic() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new RandomChecker(new Uri("random://test"));

        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));

        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.Zero));
    }

}