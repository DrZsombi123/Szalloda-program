using System;
using System.Globalization;

namespace SzallodaApp;

public class Szoba
{
    public int Szobaszam { get; set; }
    public string Tipus { get; set; }
    public int FerohelyekSzama { get; set; }
    public decimal EjszakaiAr { get; set; }

    public Szoba(int szobaszam, string tipus, int ferohelyekSzama, decimal ejszakaiAr)
    {
        Szobaszam = szobaszam;
        Tipus = tipus;
        FerohelyekSzama = ferohelyekSzama;
        EjszakaiAr = ejszakaiAr;
    }

    // InvariantCulture: a CSV tartalma legyen független a gép nyelvi beállításától.
    public string ToCsv()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0};{1};{2};{3}",
            Szobaszam,
            Tipus,
            FerohelyekSzama,
            EjszakaiAr
        );
    }

    public static Szoba FromCsv(string sor)
    {
        string[] mezok = sor.Split(';');
        if (mezok.Length != 4)
        {
            throw new FormatException($"Hibás CSV sor (nem 4 mező): {sor}");
        }

        int szobaszam = int.Parse(mezok[0], CultureInfo.InvariantCulture);
        string tipus = mezok[1];
        int ferohelyek = int.Parse(mezok[2], CultureInfo.InvariantCulture);
        decimal ar = decimal.Parse(mezok[3], CultureInfo.InvariantCulture);

        return new Szoba(szobaszam, tipus, ferohelyek, ar);
    }

    public override string ToString()
    {
        return $"Szoba #{Szobaszam} | {Tipus,-12} | {FerohelyekSzama} fő | {EjszakaiAr,8:N0} Ft/éj";
    }
}
