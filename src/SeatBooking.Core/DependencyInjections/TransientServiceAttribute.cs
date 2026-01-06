namespace SeatBooking.Core.DependencyInjections;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TransientServiceAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TransientServiceAttribute<TService> : TransientServiceAttribute;