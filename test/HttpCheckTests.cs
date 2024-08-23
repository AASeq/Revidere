namespace Tests;

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class HttpCheckTests {

    [TestMethod]
    public void HttpCheck_Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData("get", "http://example.com", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void HttpCheck_NokInvalidHost() {
        var test = Check.FromConfigData("get", "http://", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.IsNull(test);
    }

    [TestMethod]
    public void HttpCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData("get", "http://aaseq.com", title: null, name: null, isVisible: false, isBreak: false,
                                        new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1));
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

}