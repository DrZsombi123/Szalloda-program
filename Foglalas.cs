using System;

namespace szallodaprogram
{
    class Foglalas
    {
        public int Id { get; set; }
        public string VendegNev { get; set; }
        public int SzobaSzam { get; set; }
        public DateTime Erkezes { get; set; }
        public DateTime Tavozas { get; set; }
        public int ArPerEjszaka { get; set; }

        public int Ejszakak => (Tavozas - Erkezes).Days;
        public int TeljesAr => Ejszakak * ArPerEjszaka;
    
        public string CsvSor()
        {
            return $"{Id};{VendegNev};{SzobaSzam};{Erkezes:yyyy-MM-dd};{Tavozas:yyyy-MM-dd};{ArPerEjszaka}";
        }
    
    }

}