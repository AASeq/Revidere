namespace Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CheckStateTests {

    [TestMethod]
    public void Basic() {
        var test = Check.FromConfigData(kind: "dummy", target: "localhost", title: "Dummy", name: null, new CheckProfile(1, 1));
        var state = new CheckState(test);

        Assert.AreEqual(null, state.IsHealthy);
        Assert.AreEqual(0, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(true, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(1, state.HealthHistory.Count);

        state.UpdateCheck(false);
        Assert.AreEqual(false, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(false, state.HealthHistory[1]);
        Assert.AreEqual(2, state.HealthHistory.Count);
    }

    [TestMethod]
    public void Default() {
        var test = Check.FromConfigData(kind: "dummy", target: "localhost", title: "Dummy", name: null, CheckProfile.Default);
        var state = new CheckState(test);

        Assert.AreEqual(null, state.IsHealthy);
        Assert.AreEqual(0, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(null, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(1, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(null, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(2, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(true, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(3, state.HealthHistory.Count);

        state.UpdateCheck(false);
        Assert.AreEqual(true, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(4, state.HealthHistory.Count);

        state.UpdateCheck(false);
        Assert.AreEqual(true, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(false, state.HealthHistory[4]);
        Assert.AreEqual(5, state.HealthHistory.Count);

        state.UpdateCheck(false);
        Assert.AreEqual(false, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(false, state.HealthHistory[4]);
        Assert.AreEqual(false, state.HealthHistory[5]);
        Assert.AreEqual(6, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(false, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(false, state.HealthHistory[4]);
        Assert.AreEqual(false, state.HealthHistory[5]);
        Assert.AreEqual(true, state.HealthHistory[6]);
        Assert.AreEqual(7, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(false, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(false, state.HealthHistory[4]);
        Assert.AreEqual(false, state.HealthHistory[5]);
        Assert.AreEqual(true, state.HealthHistory[6]);
        Assert.AreEqual(true, state.HealthHistory[7]);
        Assert.AreEqual(8, state.HealthHistory.Count);

        state.UpdateCheck(true);
        Assert.AreEqual(true, state.IsHealthy);
        Assert.AreEqual(true, state.HealthHistory[0]);
        Assert.AreEqual(true, state.HealthHistory[1]);
        Assert.AreEqual(true, state.HealthHistory[2]);
        Assert.AreEqual(false, state.HealthHistory[3]);
        Assert.AreEqual(false, state.HealthHistory[4]);
        Assert.AreEqual(false, state.HealthHistory[5]);
        Assert.AreEqual(true, state.HealthHistory[6]);
        Assert.AreEqual(true, state.HealthHistory[7]);
        Assert.AreEqual(true, state.HealthHistory[8]);
        Assert.AreEqual(9, state.HealthHistory.Count);
    }

}