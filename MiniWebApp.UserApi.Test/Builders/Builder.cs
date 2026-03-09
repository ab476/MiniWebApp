namespace MiniWebApp.UserApi.Test.Builders;

public abstract class Builder<TEntity, TBuilder>
    where TBuilder : Builder<TEntity, TBuilder>, new()
    where TEntity : class
{
    public static TBuilder New() => new();
    public static TBuilder Default => New().WithDefaults();

    public abstract TEntity Build();
    public abstract TBuilder WithDefaults();
    protected string RandomString(int length = 8)
    {
        return Guid.NewGuid().ToString("N")[..length];
    }

    protected TBuilder Instance => (TBuilder)this;

    public static implicit operator TEntity(Builder<TEntity, TBuilder> builder)
    {
        return builder.Build();
    }
}
