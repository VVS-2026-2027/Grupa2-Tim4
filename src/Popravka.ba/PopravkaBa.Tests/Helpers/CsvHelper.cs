namespace PopravkaBa.Tests.Helpers
{
    // Čita CSV iz TestData foldera i svaki red pretvara u parametre za test.
    public static class CsvHelper 
    {
        public static IEnumerable<object[]> Ucitaj(string fajl, Func<string[], object[]> mapiraj, char separator = ';')
        {
            var putanja = Path.Combine(AppContext.BaseDirectory, "TestData", fajl);

            return File.ReadLines(putanja)
                       .Skip(1)                                  
                       .Where(red => !string.IsNullOrWhiteSpace(red))
                       .Select(red => mapiraj(red.Split(separator)));
        }
    }
}