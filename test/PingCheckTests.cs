namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;
using System.Threading;
using System.Globalization;

[TestClass]
public class PingCheckerTests {

    [TestMethod]
    public void Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "ping", target: "127.0.0.1", title: null, name: null, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void NokUnpingable() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "ping", target: "255.255.255.255", title: null, name: null, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void EmptyHostNotWorking() {
        var test = Check.FromConfigData(kind: "ping", target: "", title: null, name: null, CheckProfile.Default);
        Assert.IsNull(test);
    }

    [TestMethod]
    public void Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "ping", target: "aaseq.com", title: null, name: null,
                                        new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1));
        Assert.IsInstanceOfType(test, typeof(PingCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

}