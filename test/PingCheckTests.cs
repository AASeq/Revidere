namespace Tests;

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class PingCheckTests {

    [TestMethod]
    public void PingCheck_Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("ping", "127.0.0.1"));
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void PingCheck_NokUnpingable() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("ping", "255.255.255.255"));
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void PingCheck_EmptyHostNotWorking() {
        var test = Check.FromProperties(Helpers.GetProperties("ping", ""));
        Assert.IsNull(test);
    }

    [TestMethod]
    public void PingCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("ping", "aaseq.com",
            new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1))
        );
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

}