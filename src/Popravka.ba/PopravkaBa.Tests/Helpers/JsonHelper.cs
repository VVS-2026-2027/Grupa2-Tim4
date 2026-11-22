using System.Text.Json;

namespace PopravkaBa.Tests.Helpers
{
    // Cita JSON niz iz TestData foldera i svaki element pretvara u parametre za test.
    public static class JsonHelper
    {
        public static IEnumerable<object[]> Ucitaj(string fajl, Func<JsonElement, object[]> mapiraj)
        {
            var putanja = Path.Combine(AppContext.BaseDirectory, "TestData", fajl);

            using var dokument = JsonDocument.Parse(File.ReadAllText(putanja));

            return dokument.RootElement
                           .EnumerateArray()
                           .Select(mapiraj)
                           .ToList();
        }
    }
}
