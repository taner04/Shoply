namespace Api.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceInjectionAttribute(ServiceLifetime serviceLifetime) : Attribute
{
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ServiceInjectionAttribute<T, TInterface>(ServiceLifetime serviceLifetime) : Attribute
    where T : TInterface
{
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
    public Type ImplementationType => typeof(T);
    public Type InterfaceType => typeof(TInterface);
}