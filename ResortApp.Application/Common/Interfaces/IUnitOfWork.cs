namespace ResortApp.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IVillaRepository Villa { get; }
}