using System;

namespace InventoryManagementSystem.ApplicationCore.Services
{
    public class EnumUtil
    {
      public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}