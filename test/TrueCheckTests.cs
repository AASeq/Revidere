namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class TrueCheckTests {

    [TestMethod]
    public void TrueCheck_Basic() {
        var test = Check.FromProperties(Helpers.GetProperties("true", ""));
        Assert.IsTrue(test.CheckIsHealthy([], CancellationToken.None));
    }

    [TestMethod]
    public void TrueCheck_Dummy() {
        var test = Check.FromProperties(Helpers.GetProperties("dummy", ""));
        Assert.IsTrue(test.CheckIsHealthy([], CancellationToken.None));
    }

}