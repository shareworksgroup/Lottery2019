using Lottery2019.Images;
using Lottery2019.UI.Feature;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lottery2019.State
{
    public class LotteryDatabase
    {
        private readonly string _file;

        private Dictionary<string, List<string>> Winners
            = new Dictionary<string, List<string>>();

        private HashSet<string> WinnedPersons 
            = new HashSet<string>();

        private PersonJson PersonJson;

        public LotteryDatabase(string file)
        {
            _file = file;
            if (File.Exists(file))
            {
                Winners = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    File.ReadAllText(file));
                UpdateWinnedPersons();
            }
            PersonJson = JsonConvert.DeserializeObject<PersonJson>(
                File.ReadAllText("./Resources/person.json"));
            foreach (var kv in PersonJson.Persons)
                kv.Value.Name = kv.Key;
            foreach (var name in PersonJson.Excluded ?? new HashSet<string>())
                if (!PersonJson.Persons.ContainsKey(name)) MessageBox.Show($"名字不存在: {name}");
            foreach (var name in PersonJson.ExcludedForBig ?? new HashSet<string>())
                if (!PersonJson.Persons.ContainsKey(name)) MessageBox.Show($"名字不存在: {name}");
        }

        public IEnumerable<Person> GetAllPerson()
        {
            return PersonJson.Persons.Values;
        }

        public IEnumerable<Person> GetPersonForPrize(PrizeDto prize)
        {
            if (prize == null || prize.Count > 10)
            {
                return PersonJson.Persons
                    .Where(x => !WinnedPersons.Contains(x.Key))
                    .Where(x => !PersonJson.Excluded.Contains(x.Key))
                    .Select(x => x.Value);
            }
            else
            {
                return PersonJson.Persons
                    .Where(x => !WinnedPersons.Contains(x.Key))
                    .Where(x => !PersonJson.Excluded.Contains(x.Key))
                    .Where(x => !PersonJson.ExcludedForBig.Contains(x.Key))
                    .Select(x => x.Value);
            }
        }

        private void UpdateWinnedPersons()
        {
            WinnedPersons = new HashSet<string>(Winners.Values.SelectMany(x => x));
        }

        public bool IsPersonWinned(string personId)
        {
            return WinnedPersons.Contains(personId);
        }

        private void Save()
        {
            File.WriteAllText(_file, JsonConvert.SerializeObject(Winners, Formatting.Indented));
            UpdateWinnedPersons();
        }

        public void SetWinPersons(string id, List<Person> winPersons)
        {
            if (Winners.ContainsKey(id))
                MessageBox.Show($"{id} already exists, this should not happen.");
            
            Winners[id] = winPersons.Select(x => x.Name).ToList();
            Save();
        }

        public string[] GetWinnedPrizes()
        {
            return Winners.Keys.ToArray();
        }
    }
}
