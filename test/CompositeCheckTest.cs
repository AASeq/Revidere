namespace Tests;

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revidere;

[TestClass]
public class CompositeCheckTests {

    [TestMethod]
    public void CompositeCheck_100_2_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 100));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_100_1_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 100));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_100_0_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 100));
        var checkStates = new CheckState[] {
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }


    [TestMethod]
    public void CompositeCheck_99_2_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 99));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_99_1_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 99));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_99_0_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 99));
        var checkStates = new CheckState[] {
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }


    [TestMethod]
    public void CompositeCheck_50_2_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 50));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_50_1_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 50));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_50_0_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 50));
        var checkStates = new CheckState[] {
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }


    [TestMethod]
    public void CompositeCheck_1_2_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 1));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_1_1_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 1));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_1_0_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 1));
        var checkStates = new CheckState[] {
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsFalse(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }


    [TestMethod]
    public void CompositeCheck_0_2_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 0));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true); checkStates[1].UpdateCheck(true);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_0_1_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 0));
        var checkStates = new CheckState[] {
            new CheckState(new TrueCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true); checkStates[0].UpdateCheck(true);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

    [TestMethod]
    public void CompositeCheck_0_0_2() {
        var test = Check.FromProperties(Helpers.GetProperties("composite", "a b", 0));
        var checkStates = new CheckState[] {
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "a"))),
            new CheckState(new FalseCheck(Helpers.GetProperties("composite", "", "b"))),
        };
        checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false); checkStates[0].UpdateCheck(false);
        checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false); checkStates[1].UpdateCheck(false);

        Assert.IsTrue(test.CheckIsHealthy(checkStates, CancellationToken.None));
    }

}