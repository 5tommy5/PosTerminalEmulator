using PosTerminalEmulator.Core.Models;

namespace PosTerminalEmulator.Core.Interfaces;

public interface IProductRepository
{
    Task Add(Product product);
    Task<Product> Get(string code);
    Task<bool> Exists(string code);
}