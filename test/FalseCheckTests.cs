namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class FalseCheckTests {

    [TestMethod]
    public void FalseCheck_Basic() {
        var test = Check.FromProperties(Helpers.GetProperties("false", ""));
        Assert.IsTrue(test.CheckIsHealthy([], CancellationToken.None));
    }

}