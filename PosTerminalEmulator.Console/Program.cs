using PosTerminalEmulator;
using PosTerminalEmulator.Core.Models;
using PosTerminalEmulator.Infrastructure.Discounts;

var pos = new PosTerminal((o) =>
{
    o.Add(new Product("A", 1.25m, new VolumeDiscount(3, 3m)));
    o.Add(new Product("B", 4.25m));
    o.Add(new Product("C", 1m, new VolumeDiscount(6, 5m)));
    o.Add(new Product("D", 0.75m));
});

pos.Scan("A");
pos.Scan("B");
pos.Scan("C");
pos.Scan("D");

var totalSum = await pos.CalculateTotal();

Console.WriteLine($"Total sum: {totalSum}");