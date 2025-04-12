namespace PosTerminalEmulator.Core.Interfaces;

public interface IDiscount
{
    Task<bool> IsApplicable(int amount, decimal price);
    Task<decimal> CalculateWithDiscount(int amount, decimal previousPrice);
}