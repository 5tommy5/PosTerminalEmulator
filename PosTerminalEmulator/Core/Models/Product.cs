using PosTerminalEmulator.Core.Interfaces;

namespace PosTerminalEmulator.Core.Models;

public class Product
{
    public Product(string code, decimal price, IDiscount? discount = null)
    {
        Code = code;
        Price = price;
        Discount = discount;
    }
    public string Code { get; }
    private decimal Price { get; }
    private IDiscount? Discount { get; }

    
    public async Task<decimal> CalculatePrice(int amount)
    {
        if (Discount is null || !await Discount.IsApplicable(amount, Price))
            return amount*Price;
        
        return await Discount.CalculateWithDiscount(amount, Price);
    }
}