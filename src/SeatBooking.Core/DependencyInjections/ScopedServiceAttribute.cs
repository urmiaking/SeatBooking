namespace SeatBooking.Core.DependencyInjections;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScopedServiceAttribute : Attribute;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScopedServiceAttribute<TService> : ScopedServiceAttribute;