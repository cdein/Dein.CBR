using System.Reflection;

namespace Dein.CBRLib
{

    public record PropertyEvaluatorMapping<T>(string PropertyName, IEvaluator<T> Evaluator, Type InstanceType, double Weight = 1d)
    {
        PropertyInfo? Property { get; } = InstanceType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public);

        public T? GetValue(object instance)
        {
            if (Property == null)
                return default;
            return (T?)Property.GetValue(instance);
        }
    }

    public abstract class ObjectEvaluator<T> : IEvaluator<T> where T : class
    {
        private List<dynamic> _evaluators;

        public ObjectEvaluator()
        {
            _evaluators = [];
        }

        protected List<dynamic> Evaluators { get { return _evaluators; } }

        public void AddPropertyEvaluator<P>(string propertyName, IEvaluator<P> evaluator, double weight = 1d)
        {
            _evaluators.Add(new PropertyEvaluatorMapping<P>(propertyName, evaluator, typeof(T), weight));
        }

        public abstract double Evaluate(T queryObject, T caseObject);

        protected List<double> CollectSimilarities(T queryObject, T caseObject)
        {
            List<double> similarities = new List<double>();
            for (int i = 0, n = _evaluators.Count; i < n; i++)
            {
                var queryValue = _evaluators[i].GetValue(queryObject);
                if (queryValue == null)
                    continue;
                var caseValue = _evaluators[i].GetValue(caseObject);
                similarities.Add(_evaluators[i].Evaluator.Evaluate(queryValue, caseValue));
            }
            return similarities;
        }
    }

    public class ObjectMedianEvaluator<T> : ObjectEvaluator<T> where T : class
    {
        public override double Evaluate(T queryValue, T caseValue)
        {
            List<double> similarities = CollectSimilarities(queryValue, caseValue);
            if (similarities.Count == 0)
                return 0d;
            else if (similarities.Count == 1)
                return similarities[0];
            similarities.Sort();
            int mid = similarities.Count / 2;
            if (mid % 2 != 0)
                return similarities[mid];
            return (similarities[mid - 1] + similarities[mid]) / 2;
        }
    }

    public class ObjectMinEvaluator<T> : ObjectEvaluator<T> where T : class
    {
        public override double Evaluate(T queryValue, T caseValue)
        {
            List<double> similarities = CollectSimilarities(queryValue, caseValue);
            return similarities.Min();
        }
    }

    public class ObjectMaxEvaluator<T> : ObjectEvaluator<T> where T : class
    {
        public override double Evaluate(T queryValue, T caseValue)
        {
            List<double> similarities = CollectSimilarities(queryValue, caseValue);
            return similarities.Max();
        }
    }

    public class ObjectEuclideanEvaluator<T> : ObjectEvaluator<T> where T : class
    {
        public override double Evaluate(T queryObject, T caseObject)
        {
            double similarity_sum = 0;
            for (int i = 0, n = Evaluators.Count; i < n; i++)
            {
                var queryValue = Evaluators[i].GetValue(queryObject);
                if (queryValue == null)
                    continue;
                var caseValue = Evaluators[i].GetValue(caseObject);
                double similarity = Evaluators[i].Evaluator.Evaluate(queryValue, caseValue);
                if (similarity <= 0)
                    continue;
                similarity_sum += Math.Pow(similarity, 2);
            }
            return Math.Sqrt(similarity_sum);
        }
    }

    public class ObjectAverageEvaluator<T> : ObjectEvaluator<T> where T : class
    {
        public override double Evaluate(T queryObject, T caseObject)
        {
            double divider = 0;
            double similarity_sum = 0;
            for (int i = 0, n = Evaluators.Count; i < n; i++)
            {
                var queryValue = Evaluators[i].GetValue(queryObject);
                if (queryValue == null)
                    continue;
                double weight = Evaluators[i].Weight;
                divider += weight;
                var caseValue = Evaluators[i].GetValue(caseObject);
                similarity_sum += weight * Evaluators[i].Evaluator.Evaluate(queryValue, caseValue);
            }
            if (divider == 0)
                return 0d;
            return similarity_sum / divider;
        }
    }
}