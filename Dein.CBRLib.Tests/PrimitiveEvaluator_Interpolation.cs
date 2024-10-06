namespace Dein.CBRLib.Tests
{
    public class PrimitiveEvaluator_Interpolation_Interpolate
    {
        [Fact]
        public void Polynom_Linearity_Zero()
        {
            var interpolation = new Polynom();
            Assert.Equal(0d, interpolation.Interpolate(0.8d, 0), Constants.EqualTolerance);
        }

        [Fact]
        public void Polynom_Linearity_One()
        {
            var interpolation = new Polynom();
            for (int i = 0, n = 10; i < n; i++)
            {
                double requestedValue = ((double)i) / 10;
                Assert.Equal(1d - requestedValue, interpolation.Interpolate(requestedValue, 1), Constants.EqualTolerance);
            }
        }

        [Fact]
        public void Polynom_Linearity_05()
        {
            var interpolation = new Polynom();
            double[] expectedValues = [1.0, 0.81, 0.64, 0.49, 0.36, 0.25, 0.16, 0.09, 0.04, 0.01, 0.0];
            for (int i = 0, n = expectedValues.Length; i < n; i++)
            {
                Assert.Equal(expectedValues[i], interpolation.Interpolate(((double)i) / 10, 0.5), Constants.EqualTolerance);
            }
        }

        [Fact]
        public void Root_Linearity_Zero()
        {
            var interpolation = new Root();
            Assert.Equal(1d, interpolation.Interpolate(0.8d, 0), Constants.EqualTolerance);
        }

        [Fact]
        public void Root_Linearity_One()
        {
            var interpolation = new Root();
            for (int i = 0, n = 10; i < n; i++)
            {
                double requestedValue = ((double)i) / 10;
                Assert.Equal(1d - requestedValue, interpolation.Interpolate(requestedValue, 1), Constants.EqualTolerance);
            }
        }

        [Fact]
        public void Root_Linearity_05()
        {
            var interpolation = new Root();
            double[] expectedValues = [1.0, 0.948, 0.894, 0.836, 0.774, 0.707, 0.632, 0.547, 0.447, 0.316, 0.0];
            // We use a lower tolerance, because the values must not be exact here.
            double equalTolerance = 0.001;
            for (int i = 0, n = expectedValues.Length; i < n; i++)
            {
                Assert.Equal(expectedValues[i], interpolation.Interpolate(((double)i) / 10, 0.5), equalTolerance);
            }
        }

        [Fact]
        public void Sigmoid_Linearity_Zero()
        {
            var interpolation = new Sigmoid();
            Assert.Equal(0d, interpolation.Interpolate(0.8d, 0), Constants.EqualTolerance);
            Assert.Equal(1d, interpolation.Interpolate(0.4d, 0), Constants.EqualTolerance);
        }

        [Fact]
        public void Sigmoid_Linearity_One()
        {
            var interpolation = new Sigmoid();
            for (int i = 0, n = 10; i < n; i++)
            {
                double requestedValue = ((double)i) / 10;
                Assert.Equal(1d - requestedValue, interpolation.Interpolate(requestedValue, 1), Constants.EqualTolerance);
            }
        }

        [Fact]
        public void Sigmoid_Linearity_05()
        {
            var interpolation = new Sigmoid();
            double[] expectedValues = [1.0, 0.98, 0.92, 0.82, 0.68, 0.5, 0.32, 0.18, 0.08, 0.02, 0.0];
            for (int i = 0, n = expectedValues.Length; i < n; i++)
            {
                Assert.Equal(expectedValues[i], interpolation.Interpolate(((double)i) / 10, 0.5), Constants.EqualTolerance);
            }
        }
    }

}