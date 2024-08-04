namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;
using System.Threading;

[TestClass]
public class PingCheckerTests {

    [TestMethod]
    public void Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new PingChecker(new Uri("ping://127.0.0.1"));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void NokInvalidIp() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new PingChecker(new Uri("ping://255.255.255.255"));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void NokInvalidHost() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new PingChecker(new Uri("ping://" + Guid.NewGuid().ToString()));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new PingChecker(new Uri("ping://aaseq.com"));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.FromMilliseconds(1)));
    }

}