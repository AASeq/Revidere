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
        var test = Check.FromProperties(new CheckProperties(
            "get",
            "http://example.com",
            null,
            null,
            false,
            false,
            null,
            CheckProfile.Default)
        );
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void HttpCheck_NokInvalidHost() {
        var test = Check.FromProperties(new CheckProperties(
            "get",
            "http://",
            null,
            null,
            false,
            false,
            null,
            CheckProfile.Default)
        );
        Assert.IsNull(test);
    }

    [TestMethod]
    public void HttpCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(new CheckProperties(
            "get",
            "http://aaseq.com",
            null,
            null,
            false,
            false,
            null,
            new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1))
        );
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

}