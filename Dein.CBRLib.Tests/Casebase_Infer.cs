namespace Dein.CBRLib.Tests
{
    public record CBTestRecord(string Name)
    {
    }

    public class Casebase_Infer
    {
        [Fact]
        public void Infer_All()
        {
            var objectEvaluator = new ObjectMedianEvaluator<CBTestRecord>();
            objectEvaluator.AddPropertyEvaluator("Name", new EqualityEvaluator<string>());
            var result = Casebase.Infer(
                [new CBTestRecord("Name 1"), new CBTestRecord("Name 2"), new CBTestRecord("Name 3")],
                new ReasoningRequest<CBTestRecord>(new CBTestRecord("Name 1"), 0, 2, 0.0),
                objectEvaluator
            );
            Assert.Equal(3, result.TotalNumberOfHits);
            Assert.NotEmpty(result.Hits);
            Assert.Equal(2, result.Hits.Count());

            Result<CBTestRecord> hit;
            IEnumerator<Result<CBTestRecord>> enumerator = result.Hits.GetEnumerator();

            enumerator.MoveNext();
            hit = enumerator.Current;
            Assert.Equal(1d, hit.Similarity);

            enumerator.MoveNext();
            hit = enumerator.Current;
            Assert.Equal(0d, hit.Similarity);
        }

        [Fact]
        public void Infer_WithThreshold()
        {
            var objectEvaluator = new ObjectMedianEvaluator<CBTestRecord>();
            objectEvaluator.AddPropertyEvaluator("Name", new EqualityEvaluator<string>());
            var result = Casebase.Infer(
                [new CBTestRecord("Name 1"), new CBTestRecord("Name 2"), new CBTestRecord("Name 3")],
                new ReasoningRequest<CBTestRecord>(new CBTestRecord("Name 1"), 0, 2, 0.1),
                objectEvaluator
            );
            Assert.NotEmpty(result.Hits);
            Assert.Single(result.Hits);

            Result<CBTestRecord> hit;
            IEnumerator<Result<CBTestRecord>> enumerator = result.Hits.GetEnumerator();

            enumerator.MoveNext();
            hit = enumerator.Current;
            Assert.Equal(1d, hit.Similarity);
        }
    }
}