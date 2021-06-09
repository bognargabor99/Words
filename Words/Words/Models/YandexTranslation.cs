using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words.Models
{
    // A Yandex szervertől visszakapott fordítások modell osztálya
    // A YandexDictionaryService egy ilyen objektumot formál a szervertől kapott válaszból
    public class YandexTranslation
    {
        // A fordításokat tartalmazó tömb
        public Def[] Def { get; set; }
    }

    // fordításokat tartalmazó osztály
    public class Def
    {
        // A fordítások tömbje
        public Tr[] Tr { get; set; }
    }

    // A lefordított szó adatai
    public class Tr
    {
        // A lefordított szó a célnyelven
        public string Text { get; set; }

        // A lefordított szó a szófaja
        public string Pos { get; set; }

        // A lefordított szó jelentései a forrásnyelven
        public Mean[] Mean { get; set; }
    }

    // A lefordított szó eredeti nyelven való egy jelentése
    public class Mean
    {
        // A eredeti nyelven lévő jelentés
        public string Text { get; set; }
    }
}
