using DeviceDbModel;

namespace CommonVending.DbProvider
{
  public static class DeviceDbProvider
  {
    static DeviceDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
  }
}
