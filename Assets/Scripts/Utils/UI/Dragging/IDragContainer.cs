namespace RPG.Utils.UI
{
    public interface IDragContainer<T> : IDragDestination<T>, IDragSource<T> where T : class { }
}