using PosTerminalEmulator.Core.Models;
using PosTerminalEmulator.Infrastructure.Discounts;

namespace PosTerminalEmulator.Tests;

public class PosTerminalCalculationTests
{
    private PosTerminal _posTerminalEmulator;
    [SetUp]
    public void Setup()
    {
        _posTerminalEmulator = new PosTerminal(o =>
        {
            o.Add(new Product("A", 1.25m, new VolumeDiscount(3, 3m)));
            o.Add(new Product("B", 4.25m));
            o.Add(new Product("C", 1m, new VolumeDiscount(6, 5m)));
            o.Add(new Product("D", 0.75m));
        });
    }

    [TestCaseSource(nameof(CalculationData))]
    public async Task CalculateTotal_ReturnsCorrectResult(decimal expectedResult, string[] scans)
    {
        foreach (var scan in scans)
        {
            _posTerminalEmulator.Scan(scan);
        }

        var actualResult = await _posTerminalEmulator.CalculateTotal();
        
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }
    
    public static IEnumerable<object[]> CalculationData()
    {
        yield return new object[] { 13.25m, new [] {"A", "A", "A", "A", "B", "C", "D", "A", "A", "A"}};
        yield return new object[] { 6m, new [] {"C", "C", "C", "C", "C", "C", "C"}};
        yield return new object[] { 7.25m, new [] {"A", "B", "C", "D"}};
    }
}