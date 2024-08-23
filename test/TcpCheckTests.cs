namespace Tests;

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class TcpCheckTests {

    [TestMethod]
    public void TcpCheck_Ok() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("tcp", "aaseq.com:80"));
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void TcpCheck_NotReachable() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("tcp", "255.255.255.255:10000"));
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

    [TestMethod]
    public void TcpCheck_EmptyHostNotWorking() {
        var test = Check.FromProperties(Helpers.GetProperties("tcp", ""));
        Assert.IsNull(test);
    }

    [TestMethod]
    public void TcpCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("tcp", "aaseq.com:80",
            new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1))
        );
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
    }

}