using System;
using System.Collections.Generic;
using System.Numerics;

namespace Dein.CBRLib
{
    public interface IInterpolation
    {
        double Interpolate(double stretchedDistance, double linearity);
    }

    public class Polynom : IInterpolation
    {
        public double Interpolate(double stretchedDistance, double linearity)
        {
            if (linearity == 0d)
                return 0d;
            else if (linearity == 1d)
                return 1d - stretchedDistance;
            return Math.Pow(1d - stretchedDistance, 1d / linearity);
        }

        public static readonly Polynom Default = new Polynom();
    }

    public class Root : IInterpolation
    {
        public double Interpolate(double stretchedDistance, double linearity)
        {
            if (linearity == 0d)
                return 1d;
            else if (linearity == 1d)
                return 1d - stretchedDistance;
            return Math.Pow(1d - stretchedDistance, linearity);
        }

        public static readonly Root Default = new Root();
    }

    public class Sigmoid : IInterpolation
    {
        public double Interpolate(double stretchedDistance, double linearity)
        {
            if (linearity == 1d)
                return 1d - stretchedDistance;
            if (stretchedDistance < 0.5d)
            {
                if (linearity == 0d)
                    return 1d;
                return 1d - Math.Pow(2d * stretchedDistance, 1d / linearity) / 2d;
            }
            if (linearity == 0d)
                return 0d;
            return Math.Pow(2d - 2d * stretchedDistance, 1d / linearity) / 2d;
        }

        public static readonly Sigmoid Default = new Sigmoid();
    }

    public record NumericCalculationParameter(double Equality = 0d, double Tolerance = 0.5d, double Linearity = 1d, IInterpolation _Interpolation = null)
    {
        public IInterpolation Interpolation { get; } = _Interpolation == null ? Polynom.Default : _Interpolation;

        public static readonly NumericCalculationParameter Default = new NumericCalculationParameter();
    }

    public record NumericEvaluatorOptions<T>(NumericCalculationParameter _IfLess = null, NumericCalculationParameter _IfMore = null, T _Origin = default, bool UseOrigin = false, bool Cyclic = false) where T : INumber<T>
    {
        public NumericCalculationParameter IfLess { get; } = _IfLess == null ? NumericCalculationParameter.Default : _IfLess;
        public NumericCalculationParameter IfMore { get; } = _IfMore == null ? NumericCalculationParameter.Default : _IfMore;

        public double Origin { get; } = Convert.ToDouble(_Origin == null ? 0 : _Origin);
    }

    public class NumericEvaluator<T> : IEvaluator<T> where T : INumber<T>
    {
        private double _maxPossibleDistance;

        private NumericEvaluatorOptions<T> _options;

        public NumericEvaluator(T minimum, T maximum) : this(minimum, maximum, new NumericEvaluatorOptions<T>())
        { }

        public NumericEvaluator(T minimum, T maximum, NumericEvaluatorOptions<T> options)
        {
            _maxPossibleDistance = Math.Abs(Convert.ToDouble(maximum) - Convert.ToDouble(minimum));
            _options = options;
        }

        public double Evaluate(T queryValue, T caseValue)
        {
            double q = Convert.ToDouble(queryValue);
            double c = Convert.ToDouble(caseValue);

            double maxDistance = CalculateMaxDistance(q, _maxPossibleDistance, _options.Origin, _options.UseOrigin);
            if (maxDistance == 0d)
                return 1d;

            double distance = CalculateDistance(q, c, _maxPossibleDistance, _options.Cyclic);
            double relativeDistance = distance / maxDistance;
            if (relativeDistance >= 1d)
                return 0d;

            bool isLess = IsLess(q, c, _maxPossibleDistance, _options.Cyclic);
            NumericCalculationParameter calculationParameter = isLess ? _options.IfLess : _options.IfMore;

            if (relativeDistance <= calculationParameter.Equality)
                return 1d;
            else if (relativeDistance >= calculationParameter.Tolerance)
                return 0d;

            double stretchedDistance = (relativeDistance - calculationParameter.Equality) / (calculationParameter.Tolerance - calculationParameter.Equality);
            return calculationParameter.Interpolation.Interpolate(stretchedDistance, calculationParameter.Linearity);
        }

        private static double CalculateDistance(double v1, double v2, double maxDistance, bool cyclic)
        {
            double result = Math.Abs(v1 - v2);
            if (cyclic && result > maxDistance)
                return 2 * maxDistance - result;
            return result;
        }

        private static double CalculateMaxDistance(double v, double maxDistance, double origin, bool useOrigin)
        {
            if (useOrigin)
                return Math.Abs(v - origin);
            return maxDistance;
        }

        private static bool IsLess(double v1, double v2, double maxDistance, bool cyclic)
        {
            if (!cyclic)
                return v1 < v2;

            double leftDistance;
            if (v1 < v2)
                leftDistance = v2 - v1;
            else
                leftDistance = 2 * maxDistance - v1 + v2;

            double rightDistance = 2 * maxDistance - leftDistance;
            return leftDistance < rightDistance;
        }
    }
}