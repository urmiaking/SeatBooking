namespace SeatBooking.Core.DependencyInjections;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SingletonServiceAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SingletonServiceAttribute<TService> : SingletonServiceAttribute;
