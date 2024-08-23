namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class RandomCheckTests {

    [TestMethod]
    public void RandomCheck_Basic() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(new CheckProperties(
            "random",
            "test",
            null,
            null,
            false,
            false,
            null,
            CheckProfile.Default)
        );

        Assert.IsInstanceOfType(test, typeof(RandomCheck));

        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));

        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(false, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
        Assert.AreEqual(true, test.CheckIsHealthy([], cancelToken));
    }

}