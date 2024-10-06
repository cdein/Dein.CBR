namespace Dein.CBRLib
{
    public interface IEvaluator<T>
    {
        double Evaluate(T queryValue, T caseValue);
    }

    public class EqualityEvaluator<T> : IEvaluator<T>
    {
        public double Evaluate(T queryValue, T caseValue)
        {
            if (queryValue == null || caseValue == null)
                return 0;
            if (!queryValue.Equals(caseValue))
                return 0;
            return 1;
        }
    }
}