
namespace Dein.CBRLib.Tests
{
    public class PrimitiveEvaluator_TableLookup
    {
        [Fact]
        public void Evaluate()
        {
            var evaluator = new TableLookup(new Dictionary<string, IDictionary<string, double>>()
            {
                { "red", new Dictionary<string, double>()
                    {
                        {"orange", 0.8d},
                        {"yellow", 0.4d},
                    } },
                { "orange", new Dictionary<string, double>()
                    {
                        {"red", 0.8d},
                        {"yellow", 0.8d},
                    } }
            });
            Assert.Equal(1d, evaluator.Evaluate("red", "red"));
            Assert.Equal(0.8d, evaluator.Evaluate("red", "orange"));
            Assert.Equal(0.4d, evaluator.Evaluate("red", "yellow"));

            Assert.Equal(1d, evaluator.Evaluate("orange", "orange"));
            Assert.Equal(0.8d, evaluator.Evaluate("orange", "red"));
            Assert.Equal(0.8d, evaluator.Evaluate("orange", "yellow"));

            Assert.Equal(1d, evaluator.Evaluate("yellow", "yellow"));

            Assert.Equal(1d, evaluator.Evaluate("green", "green"));

            Assert.Equal(0d, evaluator.Evaluate("red", "green"));
            Assert.Equal(0d, evaluator.Evaluate("yellow", "green"));
            Assert.Equal(0d, evaluator.Evaluate("orange", "green"));
            Assert.Equal(0d, evaluator.Evaluate("green", "red"));
            Assert.Equal(0d, evaluator.Evaluate("green", "yellow"));
            Assert.Equal(0d, evaluator.Evaluate("green", "orange"));
        }
    }
}
