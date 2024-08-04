namespace Tests;

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class HttpCheckerTests {

    [TestMethod]
    public void Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new HttpChecker(new Uri("http://example.com"));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void NokInvalidHost() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new HttpChecker(new Uri("http://" + Guid.NewGuid().ToString()));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = new HttpChecker(new Uri("http://aaseq.com"));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken, TimeSpan.FromMilliseconds(1)));
    }

}