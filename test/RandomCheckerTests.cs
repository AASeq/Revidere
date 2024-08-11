namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class RandomCheckerTests {

    [TestMethod]
    public void Basic() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromConfigData(kind: "random", target: "test", title: null, name: null, CheckProfile.Default);

        Assert.IsInstanceOfType(test, typeof(RandomCheck));

        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));

        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy(cancelToken));
    }

}