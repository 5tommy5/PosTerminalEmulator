using PosTerminalEmulator.Core.Interfaces;

namespace PosTerminalEmulator.Infrastructure.Discounts;

public class VolumeDiscount : IDiscount
{
    private readonly int _volume;
    private readonly decimal _price;
    
    public VolumeDiscount(int volume, decimal price)
    {
        _volume = volume;
        _price = price;
    }

    public Task<bool> IsApplicable(int amount, decimal _)
    {
        return Task.FromResult(amount >= _volume);
    }
    
    public Task<decimal> CalculateWithDiscount(int amount, decimal previousPrice)
    {
        var discountAppliedTimes = amount / _volume;
        var notAppliedTimes = amount % _volume;
        
        var totalPrice = (discountAppliedTimes * _price) + (notAppliedTimes * previousPrice);
        return Task.FromResult(totalPrice);
    }
}