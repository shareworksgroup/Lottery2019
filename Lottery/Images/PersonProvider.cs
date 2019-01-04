using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lottery2019.Images
{
    public static class PersonProvider
    {
        public static IEnumerable<Person> GetPersonsFromImage()
        {
            return Directory.EnumerateFiles("./Resources/avatars")
                .Select(x => Person.FromFile(x));
        }
    }

    public class PersonJson
    {
        public Dictionary<string, Person> Persons { get; set; }

        public HashSet<string> Excluded { get; set; }

        public HashSet<string> ExcludedForBig { get; set; }
    }
}
