using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Words.Models;

namespace Words.Services
{
    class SaurusService : ServiceBase
    {
        // Example: http://thesaurus.altervista.org/thesaurus/v1?key=AwvB9jZGZztlpi9zoMPW&word=good&language=en_US&output=json
        private readonly string theSaurusApiKey = "AwvB9jZGZztlpi9zoMPW";
        private readonly Uri apiUrl = new Uri("http://thesaurus.altervista.org/thesaurus/");
        private readonly List<string> langs = new List<string>() { "cs_CZ", "da_DK", "de_CH", "de_DE", "en_US", "el_GR", "es_ES", "fr_FR", "hu_HU", "it_IT", "no_NO", "pl_PL", "pt_PT", "ro_RO", "ru_RU", "sk_SK" };

        // Visszaadja a kiválasztható nyelvet listáját
        public List<string> GetLangs()
        {
            return langs;
        }

        // A szervertől a elkéri a paraméterben kapott szóhoz és nyelvhez tartozó szinonímákat
        public async Task<SaurusSynonym> GetSynonymAsync(string word, string lang)
        {
            return await GetAsync<SaurusSynonym>(new Uri(apiUrl, $"v1?key={theSaurusApiKey}&word={word}&language={lang}&output=json"));
        }
    }
}
