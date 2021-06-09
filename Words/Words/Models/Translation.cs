using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words.Models
{
    // Az egy fordítás objektumhoz a View-n megjelenítendő adatok osztálya
    public class Translation
    {
        // A lefordított szó a célnyelven
        public string TranslatedWord { get; set; }
        
        // A lefordított szó szófaja a célnyelven
        public string WordType { get; set; }

        // A lefordított szó jelentései a forrásnyelven vesszővel elválasztva
        public string Meaning { get; set; }
    }
}
