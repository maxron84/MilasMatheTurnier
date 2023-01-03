namespace Userdatalib.Repositories;

public interface IRepositible<T> : ICrudable<T>, IJsonable<T>, IReflectable<T> { }
