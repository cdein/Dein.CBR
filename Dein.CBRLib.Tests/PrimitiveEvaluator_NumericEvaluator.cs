using System.Reflection;

namespace Dein.CBRLib.Tests
{
    public class PrimitiveEvaluator_NumericEvaluator
    {
        [Fact]
        public void NumericCalculationParameter_Defaults()
        {
            var parameter = new NumericCalculationParameter();
            Assert.Equal(0d, parameter.Equality);
            Assert.Equal(0.5d, parameter.Tolerance);
            Assert.Equal(1d, parameter.Linearity);
            Assert.Equal(typeof(Polynom), parameter.Interpolation.GetType());
        }

        [Fact]
        public void NumericEvaluatorOptions_Defaults()
        {
            var evaluatorOptions1 = new NumericEvaluatorOptions<int>();
            Assert.Equal(NumericCalculationParameter.Default, evaluatorOptions1.IfLess);
            Assert.Equal(NumericCalculationParameter.Default, evaluatorOptions1.IfMore);
            Assert.Equal(0d, evaluatorOptions1.Origin);
            Assert.False(evaluatorOptions1.UseOrigin);
            Assert.False(evaluatorOptions1.Cyclic);
            var evaluatorOptions2 = new NumericEvaluatorOptions<double>();
            Assert.Equal(0d, evaluatorOptions2.Origin);
            var evaluatorOptions3 = new NumericEvaluatorOptions<decimal>();
            Assert.Equal(0d, evaluatorOptions3.Origin);
        }

        [Fact]
        public void NumericEvaluator_CalculateDistance()
        {
            var evaluator = new NumericEvaluator<int>((int)0, (int)10);
            MethodInfo calculateDistance = GetStaticNonPublicMethod(evaluator, "CalculateDistance");

            object[] parameters;
            parameters = [7, 15, 7, false];
            Assert.Equal(8d, calculateDistance.Invoke(evaluator, parameters));
            parameters = [7, 15, 7, true];
            Assert.Equal(6d, calculateDistance.Invoke(evaluator, parameters));
        }

        [Fact]
        public void NumericEvaluator_CalculateMaxDistance()
        {
            var evaluator = new NumericEvaluator<int>((int)0, (int)10);
            MethodInfo calculateMaxDistance = GetStaticNonPublicMethod(evaluator, "CalculateMaxDistance");

            object[] parameters = [7, 15, 0, false];
            Assert.Equal(15d, calculateMaxDistance.Invoke(evaluator, parameters));
        }

        [Fact]
        public void NumericEvaluator_IsLess()
        {
            var evaluator = new NumericEvaluator<int>((int)0, (int)10);
            MethodInfo isLess = GetStaticNonPublicMethod(evaluator, "IsLess");

            object[] parameters;
            parameters = [1, 2, 3, false];
            Assert.True((bool?)isLess.Invoke(evaluator, parameters));
            parameters = [2, 1, 3, false];
            Assert.False((bool?)isLess.Invoke(evaluator, parameters));

            parameters = [3, 4, 3, true];
            Assert.True((bool?)isLess.Invoke(evaluator, parameters));
            parameters = [4, 3, 3, true];
            Assert.False((bool?)isLess.Invoke(evaluator, parameters));
        }

        [Fact]
        public void NumericEvaluator_Evaluate_Equality()
        {
            NumericEvaluator<double> evaluator;
            evaluator = new NumericEvaluator<double>(0d, 10d, new NumericEvaluatorOptions<double>(
                new NumericCalculationParameter(0.1d)
            ));
            for (int i = 0, n = 10; i < n; i++)
            {
                Assert.Equal(1d, evaluator.Evaluate(10d, 9d + (((double)i) / 10d)), Constants.EqualTolerance);
            }
            Assert.True(evaluator.Evaluate(8.9999999999d, 10d) < 1d);
            evaluator = new NumericEvaluator<double>(0d, 10d, new NumericEvaluatorOptions<double>(
                NumericCalculationParameter.Default,
                new NumericCalculationParameter(0.1d)
            ));
            for (int i = 0, n = 10; i < n; i++)
            {
                Assert.Equal(1d, evaluator.Evaluate(9d + (((double)i) / 10d), 10d), Constants.EqualTolerance);
            }
            Assert.True(evaluator.Evaluate(10d, 8.9999999999d) < 1d);
        }

        [Fact]
        public void NumericEvaluator_Evaluate()
        {
            var evaluator = new NumericEvaluator<int>((int)0, (int)20);
            double[] expectedValues = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0];
            for (int i = 0, n = expectedValues.Length; i < n; i++)
            {
                Assert.Equal(expectedValues[i], evaluator.Evaluate(i + 1, 10), Constants.EqualTolerance);
            }
        }

        MethodInfo GetStaticNonPublicMethod(object instance, string name)
        {
            MethodInfo? methodInfo = instance.GetType().GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(methodInfo);
            return methodInfo;
        }
    }
}