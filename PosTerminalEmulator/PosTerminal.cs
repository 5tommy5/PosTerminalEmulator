using PosTerminalEmulator.Core.Interfaces;
using PosTerminalEmulator.Infrastructure.Repositories;

namespace PosTerminalEmulator;

public class PosTerminal
{
    private readonly IProductRepository _productsRepository;
    private Dictionary<string, int> _cartProducts = new();

    public PosTerminal(IProductRepository repository)
    {
        _productsRepository = repository;
    }
    public PosTerminal(Action<IProductRepository> productsOptions)
    {
        _productsRepository = new ProductsInMemoryRepository();
        productsOptions.Invoke(_productsRepository);
    }
    
    public async Task Scan(string code)
    {
        var exists = await _productsRepository.Exists(code);
        
        if (!exists)
            throw new ArgumentException($"Code {code} does not exist");
        
        var alreadyScanned = _cartProducts.TryGetValue(code, out var currentAmount);

        if (!alreadyScanned)
        {
            _cartProducts.Add(code, 1);
            return;
        }
        
        _cartProducts[code] = currentAmount + 1;
    }

    public async Task<decimal> CalculateTotal()
    {
        var totalPrice = 0m;

        foreach (var cartProduct in _cartProducts)
        {
            var product = await _productsRepository.Get(cartProduct.Key);
            totalPrice += await product.CalculatePrice(cartProduct.Value);
        }
        
        return totalPrice;
    }

    public void Clear()
    {
        _cartProducts.Clear();
    }
    
}