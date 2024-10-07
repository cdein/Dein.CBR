using System.Collections.Generic;
using System.Linq;

namespace Dein.CBRLib
{
    public record Result<T>(double Similarity, T CaseObject) where T : class
    { }

    public record ReasoningRequest<T>(T Query, int Offset = 0, int Limit = 10, double Threshold = 0.1d) where T : class
    { }

    public record ReasoningResponse<T>(int TotalNumberOfHits, IEnumerable<Result<T>> Hits) where T : class
    { }

    public class Casebase
    {

        public static ReasoningResponse<T> Infer<T>(IEnumerable<T> casebase, ReasoningRequest<T> request, IEvaluator<T> evaluator) where T : class
        {
            T queryObject = request.Query;
            double threshold = request.Threshold;
            int offset = request.Offset;
            int limit = request.Limit;
            var calculateResult = casebase
                .Select(
                    caseObject => new Result<T>(evaluator.Evaluate(queryObject, caseObject), caseObject)
                )
                .OrderByDescending(result => result.Similarity)
                .Where(result => result.Similarity >= threshold);
            var resultList = calculateResult.ToList();
            int totalNumberOfHits = resultList.Count;

            return new ReasoningResponse<T>(
                totalNumberOfHits,
                [.. resultList.GetRange(offset, limit > totalNumberOfHits ? totalNumberOfHits : limit)]);
        }
    }
}