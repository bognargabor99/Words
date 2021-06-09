using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words.Models
{
    // A TheSaurus szervertől visszakapott szinonímák modell osztálya
    // A SaurusService egy ilyen objektumot formál a szervertől kapott válaszból
    public class SaurusSynonym
    {
        // A szervertől kapott válaszok tömbje
        public Response[] Response { get; set; }
    }

    // A szervertől kapott válasz osztálya
    public class Response
    {
        // A szinonímák listája
        public List List { get; set; }
    }

    // A szinonímák listáját tartalmazó osztály
    public class List
    {
        // A szinonímákhoz tartozó lista '|' karakterekkel elválasztva
        public string Synonyms { get; set; }
    }
}
