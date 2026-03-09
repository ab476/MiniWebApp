namespace MiniWebApp.UserApi.Test.Builders;

public interface IBuilder<TEntity, TBuilder> 
    where TBuilder : IBuilder<TEntity, TBuilder>
    where TEntity : class
{
    public abstract static TBuilder Default { get; }
    TBuilder WithDefaults();
    public static abstract implicit operator TEntity(TBuilder builder);
}
