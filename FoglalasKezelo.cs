using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SzallodaApp;

public class FoglalasKezelo
{
    private List<Foglalas> foglalasok;
    private string csvEleresiUt;
    private SzobaKezelo szobaKezelo;

    private const string Fejlec = "id;szobaszam;vendeg_nev;telefon;erkezes;tavozas;teljes_ar";

    public FoglalasKezelo(string csvEleresiUt, SzobaKezelo szobaKezelo)
    {
        this.csvEleresiUt = csvEleresiUt;
        this.szobaKezelo = szobaKezelo;
        this.foglalasok = new List<Foglalas>();
        Betolt();
    }

    public void Betolt()
    {
        foglalasok.Clear();

        if (!File.Exists(csvEleresiUt))
        {
            return;
        }

        string[] sorok = File.ReadAllLines(csvEleresiUt);
        for (int i = 1; i < sorok.Length; i++)
        {
            string sor = sorok[i].Trim();
            if (string.IsNullOrEmpty(sor))
            {
                continue;
            }

            try
            {
                Foglalas f = Foglalas.FromCsv(sor);
                foglalasok.Add(f);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Figyelmeztetés: hibás sor a foglalasok.csv fájlban ({i + 1}. sor): {ex.Message}");
            }
        }
    }

    public void Ment()
    {
        string? mappa = Path.GetDirectoryName(csvEleresiUt);
        if (!string.IsNullOrEmpty(mappa) && !Directory.Exists(mappa))
        {
            Directory.CreateDirectory(mappa);
        }

        List<string> sorok = new List<string>();
        sorok.Add(Fejlec);
        foreach (Foglalas f in foglalasok)
        {
            sorok.Add(f.ToCsv());
        }

        File.WriteAllLines(csvEleresiUt, sorok);
    }

    // Új foglalás felvétele - minden validáció itt fut le.
    public Foglalas UjFoglalas(
        int szobaszam,
        string vendegNev,
        string vendegTelefon,
        DateTime erkezes,
        DateTime tavozas)
    {
        Szoba? szoba = szobaKezelo.Keres(szobaszam);
        if (szoba == null)
        {
            throw new InvalidOperationException(
                $"Nincs {szobaszam} számú szoba. Először vegyél fel ilyen szobát.");
        }

        if (tavozas <= erkezes)
        {
            throw new ArgumentException(
                "A távozás dátumának későbbnek kell lennie, mint az érkezés.");
        }

        if (string.IsNullOrWhiteSpace(vendegNev))
        {
            throw new ArgumentException("A vendég neve nem lehet üres.");
        }

        // Ütközés ellenőrzés ugyanarra a szobára
        foreach (Foglalas letezo in foglalasok)
        {
            if (letezo.Szobaszam != szobaszam)
            {
                continue;
            }

            if (letezo.UtkozikE(erkezes, tavozas))
            {
                throw new InvalidOperationException(
                    $"Ütközés! A {szobaszam}. szoba már foglalt " +
                    $"{letezo.ErkezesDatum:yyyy-MM-dd} és {letezo.TavozasDatum:yyyy-MM-dd} között " +
                    $"({letezo.VendegNev}).");
            }
        }

        int ejszakak = (tavozas - erkezes).Days;
        decimal teljesAr = ejszakak * szoba.EjszakaiAr;
        int ujId = KovetkezoId();

        Foglalas ujFoglalas = new Foglalas(
            ujId, szobaszam, vendegNev, vendegTelefon, erkezes, tavozas, teljesAr);

        foglalasok.Add(ujFoglalas);
        Ment();
        return ujFoglalas;
    }

    private int KovetkezoId()
    {
        if (foglalasok.Count == 0)
        {
            return 1;
        }

        int max = 0;
        foreach (Foglalas f in foglalasok)
        {
            if (f.Id > max)
            {
                max = f.Id;
            }
        }
        return max + 1;
    }

    public bool Torol(int id)
    {
        Foglalas? talalt = Keres(id);
        if (talalt == null)
        {
            return false;
        }

        foglalasok.Remove(talalt);
        Ment();
        return true;
    }

    public Foglalas? Keres(int id)
    {
        foreach (Foglalas f in foglalasok)
        {
            if (f.Id == id)
            {
                return f;
            }
        }
        return null;
    }

    public List<Foglalas> KeresNevSzerint(string reszletNev)
    {
        List<Foglalas> eredmeny = new List<Foglalas>();
        if (string.IsNullOrWhiteSpace(reszletNev))
        {
            return eredmeny;
        }

        string kereses = reszletNev.ToLower();
        foreach (Foglalas f in foglalasok)
        {
            if (f.VendegNev.ToLower().Contains(kereses))
            {
                eredmeny.Add(f);
            }
        }
        return eredmeny;
    }

    public void ModositDatum(int id, DateTime ujErkezes, DateTime ujTavozas)
    {
        Foglalas? f = Keres(id);
        if (f == null)
        {
            throw new InvalidOperationException($"Nincs #{id} számú foglalás.");
        }

        if (ujTavozas <= ujErkezes)
        {
            throw new ArgumentException("A távozásnak későbbnek kell lennie, mint az érkezés.");
        }

        // Ütközés check, de a saját magát ne tekintsük ütközőnek
        foreach (Foglalas letezo in foglalasok)
        {
            if (letezo.Id == id)
            {
                continue;
            }
            if (letezo.Szobaszam != f.Szobaszam)
            {
                continue;
            }
            if (letezo.UtkozikE(ujErkezes, ujTavozas))
            {
                throw new InvalidOperationException(
                    $"Ütközés! A szoba már foglalt " +
                    $"{letezo.ErkezesDatum:yyyy-MM-dd} - {letezo.TavozasDatum:yyyy-MM-dd} között.");
            }
        }

        Szoba? szoba = szobaKezelo.Keres(f.Szobaszam);
        if (szoba == null)
        {
            throw new InvalidOperationException("A foglalás szobája már nem létezik.");
        }

        int ejszakak = (ujTavozas - ujErkezes).Days;
        f.ErkezesDatum = ujErkezes;
        f.TavozasDatum = ujTavozas;
        f.TeljesAr = ejszakak * szoba.EjszakaiAr;
        Ment();
    }

    public List<Foglalas> Osszes()
    {
        return new List<Foglalas>(foglalasok);
    }

    public List<Foglalas> MaiErkezesek()
    {
        List<Foglalas> eredmeny = new List<Foglalas>();
        DateTime ma = DateTime.Today;
        foreach (Foglalas f in foglalasok)
        {
            if (f.ErkezesDatum.Date == ma)
            {
                eredmeny.Add(f);
            }
        }
        return eredmeny;
    }

    // Azokat a szobákat adja vissza, amelyekre az adott időszakban nincs
    // ütköző foglalás - ez a program "Excel-killer" funkciója.
    public List<Szoba> SzabadSzobak(DateTime erkezes, DateTime tavozas)
    {
        if (tavozas <= erkezes)
        {
            throw new ArgumentException("A távozásnak későbbnek kell lennie, mint az érkezés.");
        }

        List<Szoba> szabadSzobak = new List<Szoba>();

        foreach (Szoba sz in szobaKezelo.Osszes())
        {
            bool foglalt = false;

            foreach (Foglalas f in foglalasok)
            {
                if (f.Szobaszam != sz.Szobaszam)
                {
                    continue;
                }
                if (f.UtkozikE(erkezes, tavozas))
                {
                    foglalt = true;
                    break;
                }
            }

            if (!foglalt)
            {
                szabadSzobak.Add(sz);
            }
        }

        return szabadSzobak;
    }
}
