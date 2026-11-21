using System.Xml.Linq;

namespace PopravkaBa.Tests.Helpers
{
    public static class XmlHelper  
    {
        public static IEnumerable<object[]> Ucitaj(string fajl, string element, Func<XElement, object[]> mapiraj)
        {
            var putanja = Path.Combine(AppContext.BaseDirectory, "TestData", fajl);

            return XDocument.Load(putanja)
                            .Descendants(element)
                            .Select(mapiraj);
        }
    }
}