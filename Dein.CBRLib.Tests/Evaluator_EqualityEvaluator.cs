
namespace Dein.CBRLib.Tests
{
    public class EqualityEvaluator_Evaluate
    {
        [Fact]
        public void Evaluate_Equal()
        {
            var evaluator = new EqualityEvaluator<int>();
            Assert.Equal(1, evaluator.Evaluate(42, 42));
        }

        [Fact]
        public void Evaluate_NotEqual()
        {
            var evaluator = new EqualityEvaluator<int>();
            Assert.Equal(0, evaluator.Evaluate(42, 0));
        }
    }
}