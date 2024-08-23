namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class RandomCheckTests {

    [TestMethod]
    public void RandomCheck_Basic() {
        var cancelToken = new CancellationTokenSource().Token;
        var test = Check.FromProperties(Helpers.GetProperties("random", "test"));

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