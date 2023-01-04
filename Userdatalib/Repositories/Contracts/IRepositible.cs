namespace Userdatalib.Repositories.Contracts;

public interface IRepositible<T> : ICrudable<T>, IJsonable<T>, IReflectable<T> { }
