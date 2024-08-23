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
        var test = Check.FromConfigData(kind: "tcp", target: "aaseq.com:80", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void TcpCheck_NotReachable() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "tcp", target: "255.255.255.255:10000", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

    [TestMethod]
    public void TcpCheck_EmptyHostNotWorking() {
        var test = Check.FromConfigData(kind: "tcp", target: "", title: null, name: null, isVisible: false, isBreak: false, CheckProfile.Default);
        Assert.IsNull(test);
    }

    [TestMethod]
    public void TcpCheck_Timeout() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "tcp", target: "aaseq.com:80", title: null, name: null, isVisible: false, isBreak: false,
                                        new CheckProfile(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), 1, 1));
        Assert.IsInstanceOfType(test, typeof(TcpCheck));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
    }

}