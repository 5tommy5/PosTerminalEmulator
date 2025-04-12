using Moq;
using PosTerminalEmulator.Core.Interfaces;
using PosTerminalEmulator.Core.Models;
using PosTerminalEmulator.Infrastructure.Discounts;

namespace PosTerminalEmulator.Tests;

public class PosTerminalTests
{
    [Test]
    public void Constructor_WithIProductRepository_CreatesPosTerminal()
    {
        var repositoryMock = new Mock<IProductRepository>();
        repositoryMock.Setup(x => x.Get("A")).ReturnsAsync(() => new Product("A", 1.25m));
        
        PosTerminal posTerminal;
        
        Assert.DoesNotThrow(() => posTerminal = new PosTerminal(repositoryMock.Object));
    }

    [Test]
    public void Constructor_WithActionConfiguresRepository()
    {
        PosTerminal posTerminal;
        
        var terminalInitialization = () => posTerminal = new PosTerminal(repo =>
        {
            repo.Add(new Product("A", 10m));
        });
        
        Assert.DoesNotThrow(() => terminalInitialization());
    }

    [Test]
    public void Scan_ExistingProduct()
    {
        var repositoryMock = new Mock<IProductRepository>();
        repositoryMock.Setup(x => x.Exists("A")).ReturnsAsync(() => true);
        
        var posTerminal = new PosTerminal(repositoryMock.Object);
        
        Assert.DoesNotThrowAsync(async () => await posTerminal.Scan("A"));
    }
    
    [Test]
    public void Scan_MissingProduct()
    {
        var repositoryMock = new Mock<IProductRepository>();
        repositoryMock.Setup(x => x.Exists("A")).ReturnsAsync(() => false);
        
        var posTerminal = new PosTerminal(repositoryMock.Object);
        
        Assert.ThrowsAsync<ArgumentException>(async () => await posTerminal.Scan("A"));
    }
    
    [TestCase(null)]
    [TestCase("")]
    public void Scan_InvalidCode_Throws(string code)
    {
        var repoMock = new Mock<IProductRepository>();
        var posTerminal = new PosTerminal(repoMock.Object);

        Assert.ThrowsAsync<ArgumentException>(() => posTerminal.Scan(code));
    }
    
    [Test]
    public async Task Clear_EmptyCart()
    {
        var repositoryMock = new Mock<IProductRepository>();
        repositoryMock.Setup(x => x.Exists("A")).ReturnsAsync(() => true);
        
        var posTerminal = new PosTerminal(repositoryMock.Object);
        await posTerminal.Scan("A");
        posTerminal.Clear();
        var calculationResult = await posTerminal.CalculateTotal();
        
        Assert.Zero(calculationResult);
    }
    
    [Test]
    public async Task CalculateTotal_ExactVolumeDiscountMatch()
    {
        var posTerminal = new PosTerminal(repo =>
        {
            repo.Add(new Product("X", 2m, new VolumeDiscount(2, 3m)));
        });

        await posTerminal.Scan("X");
        await posTerminal.Scan("X");

        var result = await posTerminal.CalculateTotal();

        Assert.That(result, Is.EqualTo(3m));
    }
    
    [Test]
    public async Task CalculateTotal_VolumeDiscountWithRemainderUnits()
    {
        var posTerminal = new PosTerminal(repo =>
        {
            repo.Add(new Product("A", 1.25m, new VolumeDiscount(3, 3m)));
        });

        await posTerminal.Scan("A");
        await posTerminal.Scan("A");
        await posTerminal.Scan("A");
        await posTerminal.Scan("A");

        var result = await posTerminal.CalculateTotal();

        Assert.That(result, Is.EqualTo(4.25m));
    }
    
    [Test]
    public async Task CalculateTotal_ProductWithoutDiscount()
    {
        var terminal = new PosTerminal(repo =>
        {
            repo.Add(new Product("B", 4.25m));
        });

        await terminal.Scan("B");
        await terminal.Scan("B");

        var result = await terminal.CalculateTotal();

        Assert.That(result, Is.EqualTo(8.50m));
    }
    
    [Test]
    public async Task ScanAfterClear_WorksCorrectly()
    {
        var terminal = new PosTerminal(repo =>
        {
            repo.Add(new Product("A", 1.0m));
        });

        await terminal.Scan("A");
        terminal.Clear();

        await terminal.Scan("A");
        var result = await terminal.CalculateTotal();

        Assert.That(result, Is.EqualTo(1.0m));
    }
}