namespace Tests;

using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class HttpCheckTests {

    [TestMethod]
    public void Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData("get", "http://example.com", title: null, name: null, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void NokInvalidHost() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData("get", "http://", title: null, name: null, CheckProfile.Default);
        Assert.IsNull(test);
    }

    [TestMethod]
    public void Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData("get", "http://aaseq.com", title: null, name: null,
                                        new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1));
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

}