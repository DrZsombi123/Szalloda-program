using System;
using System.Globalization;

namespace SzallodaApp;

public class Foglalas
{
    public int Id { get; set; }
    public int Szobaszam { get; set; }
    public string VendegNev { get; set; }
    public string VendegTelefon { get; set; }
    public DateTime ErkezesDatum { get; set; }
    public DateTime TavozasDatum { get; set; }
    public decimal TeljesAr { get; set; }

    public Foglalas(
        int id,
        int szobaszam,
        string vendegNev,
        string vendegTelefon,
        DateTime erkezesDatum,
        DateTime tavozasDatum,
        decimal teljesAr)
    {
        Id = id;
        Szobaszam = szobaszam;
        VendegNev = vendegNev;
        VendegTelefon = vendegTelefon;
        ErkezesDatum = erkezesDatum;
        TavozasDatum = tavozasDatum;
        TeljesAr = teljesAr;
    }

    public int EjszakakSzama()
    {
        return (TavozasDatum - ErkezesDatum).Days;
    }

    // Intervallum-átfedés: két időszak akkor ütközik, ha A kezdete korábbi
    // mint B vége, ÉS B kezdete korábbi mint A vége.
    public bool UtkozikE(DateTime erkezes, DateTime tavozas)
    {
        return erkezes < this.TavozasDatum && this.ErkezesDatum < tavozas;
    }

    public string ToCsv()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0};{1};{2};{3};{4:yyyy-MM-dd};{5:yyyy-MM-dd};{6}",
            Id,
            Szobaszam,
            VendegNev,
            VendegTelefon,
            ErkezesDatum,
            TavozasDatum,
            TeljesAr
        );
    }

    public static Foglalas FromCsv(string sor)
    {
        string[] mezok = sor.Split(';');
        if (mezok.Length != 7)
        {
            throw new FormatException($"Hibás CSV sor (nem 7 mező): {sor}");
        }

        int id = int.Parse(mezok[0], CultureInfo.InvariantCulture);
        int szobaszam = int.Parse(mezok[1], CultureInfo.InvariantCulture);
        string nev = mezok[2];
        string tel = mezok[3];
        DateTime erkezes = DateTime.ParseExact(mezok[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime tavozas = DateTime.ParseExact(mezok[5], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        decimal ar = decimal.Parse(mezok[6], CultureInfo.InvariantCulture);

        return new Foglalas(id, szobaszam, nev, tel, erkezes, tavozas, ar);
    }

    public override string ToString()
    {
        return $"#{Id,3} | Szoba {Szobaszam} | {VendegNev,-20} | " +
               $"{ErkezesDatum:yyyy-MM-dd} -> {TavozasDatum:yyyy-MM-dd} " +
               $"({EjszakakSzama()} éj) | {TeljesAr,9:N0} Ft";
    }
}
