namespace DecorPlastsAPI.Interface
{
    public interface IDataRepository
    {
        IEnumerable<T> Query<T>(string storedProcedure, dynamic param = null);
        int ExecuteSP(string storedProcedure, dynamic param = null);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultipleSP<T1, T2>(string storedProcedure, dynamic param = null);
        T QueryFirstOrDefault<T>(string storedProcedure, dynamic param = null);

    }
}
