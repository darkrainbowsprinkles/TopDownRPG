namespace RPG.Utils
{
    public interface IPredicateEvaluator
    {
        bool? Evaluate(EPredicate predicate, string[] parameters);
    }
}