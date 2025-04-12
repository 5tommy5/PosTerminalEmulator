using PosTerminalEmulator.Core.Interfaces;
using PosTerminalEmulator.Core.Models;

namespace PosTerminalEmulator.Infrastructure.Repositories;

public class ProductsInMemoryRepository : IProductRepository
{
    private Dictionary<string, Product> _products = new();
    
    public Task Add(Product product)
    {
        _products.Add(product.Code, product);
        return Task.CompletedTask;
    }

    public Task<Product> Get(string code)
    {
        if (_products.TryGetValue(code, out var product))
            return Task.FromResult(product);
        
        throw new KeyNotFoundException();
    }

    public Task<bool> Exists(string code)
    {
        return Task.FromResult(_products.ContainsKey(code));
    }
}