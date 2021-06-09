using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Words.Models;

namespace Words.Services
{
    class YandexDictionaryService : ServiceBase
    {
        private readonly string yandexApiKey = "dict.1.1.20210505T105153Z.b760e9c17cec79ed.fca3acece0af1ce5d1b07658d5668886160afa54";
        private readonly Uri yandexApiUrl = new Uri("https://dictionary.yandex.net/api/v1/dicservice.json");

        // A fordítási kéréséhez tartozó függvény
        // Paraméterben megkapja a lefordítandó szót és a kiválasztott nyelvpárt (pl. 'en-de')
        public async Task<YandexTranslation> GetTranslationAsync(string word, string lang)
        {
            return await GetAsync<YandexTranslation>(new Uri($"{yandexApiUrl.AbsoluteUri}/lookup?key={yandexApiKey}&lang={lang}&text={word}"));
        }

        // A szervertől elkéri a választható nyelvpárok listáját
        public async Task<List<string>> GetLangsAsync()
        {
            return await GetAsync<List<string>>(new Uri($"{yandexApiUrl.AbsoluteUri}/getLangs?key={yandexApiKey}"));
        }
    }
}
