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
        var test = Check.FromProperties(Helpers.GetProperties("get", "http://example.com"));
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void HttpCheck_NokInvalidHost() {
        var test = Check.FromProperties(Helpers.GetProperties("get", "http://"));
        Assert.IsNull(test);
    }

    [TestMethod]
    public void HttpCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("get", "http://aaseq.com",
            new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1))
        );
        Assert.IsInstanceOfType(test, typeof(HttpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

}