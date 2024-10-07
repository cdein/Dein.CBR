using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Dein.CBRLib;

var ageEvaluator = new NumericEvaluator<int>(0, 100);
var ratingEvaluator = new NumericEvaluator<int>(0, 10,
    new NumericEvaluatorOptions<int>(
        new NumericCalculationParameter(0, 0.1),
        new NumericCalculationParameter(0, 0.1)
    ));

var whiskeyEvaluator = new ObjectAverageEvaluator<Whiskey>();
whiskeyEvaluator.AddPropertyEvaluator<string>("Distillery", new EqualityEvaluator<string>());
whiskeyEvaluator.AddPropertyEvaluator<int>("Age", ageEvaluator);
whiskeyEvaluator.AddPropertyEvaluator<int>("Sweetness", ratingEvaluator);
whiskeyEvaluator.AddPropertyEvaluator<int>("Peatiness", ratingEvaluator);
whiskeyEvaluator.AddPropertyEvaluator<int>("Availability", ratingEvaluator);
whiskeyEvaluator.AddPropertyEvaluator<string>("Color", new EqualityEvaluator<string>());

List<Whiskey> whiskeys;
using (var whiskeyReader = new CsvReader(
        new StreamReader("Whiskey.csv"),
        new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8, HeaderValidated = null })
    )
{
    whiskeys = whiskeyReader.GetRecords<Whiskey>().ToList();
}

var result = Casebase.Infer(whiskeys, new ReasoningRequest<Whiskey>(new Whiskey(null, 40), 0, 20), whiskeyEvaluator);
foreach(Result<Whiskey> hit in result.Hits)
{
    Console.WriteLine(hit.Similarity + " > " + hit.CaseObject.Distillery + " (Age: " + hit.CaseObject.Age + ")");
}

record Whiskey([Name("distillery")] string? Distillery = null, [Name("age")] int? Age = null, [Name("sweetness")] int? Sweetness = null,
    [Name("peatiness")] int? Peatiness = null, [Name("availability")] int? Availability = null, [Name("colour")] string? Color = null)
{ }
