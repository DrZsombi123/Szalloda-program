using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SzallodaApp;

public class SzobaKezelo
{
    private List<Szoba> szobak;
    private string csvEleresiUt;

    private const string Fejlec = "szobaszam;tipus;ferohelyek;ejszakai_ar";

    public SzobaKezelo(string csvEleresiUt)
    {
        this.csvEleresiUt = csvEleresiUt;
        this.szobak = new List<Szoba>();
        Betolt();
    }

    public void Betolt()
    {
        szobak.Clear();

        if (!File.Exists(csvEleresiUt))
        {
            return;
        }

        string[] sorok = File.ReadAllLines(csvEleresiUt);

        // 0. sor a fejléc, ezért 1-től indulunk
        for (int i = 1; i < sorok.Length; i++)
        {
            string sor = sorok[i].Trim();
            if (string.IsNullOrEmpty(sor))
            {
                continue;
            }

            try
            {
                Szoba sz = Szoba.FromCsv(sor);
                szobak.Add(sz);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Figyelmeztetés: hibás sor a szobak.csv fájlban ({i + 1}. sor): {ex.Message}");
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
        foreach (Szoba sz in szobak)
        {
            sorok.Add(sz.ToCsv());
        }

        File.WriteAllLines(csvEleresiUt, sorok);
    }

    public void Hozzaad(Szoba uj)
    {
        if (Keres(uj.Szobaszam) != null)
        {
            throw new InvalidOperationException(
                $"Már létezik {uj.Szobaszam} számú szoba. Egy szobaszám csak egyszer lehet.");
        }

        if (uj.EjszakaiAr <= 0)
        {
            throw new ArgumentException("Az éjszakai ár csak pozitív lehet.");
        }

        if (uj.FerohelyekSzama <= 0)
        {
            throw new ArgumentException("A férőhelyek száma csak pozitív lehet.");
        }

        if (string.IsNullOrWhiteSpace(uj.Tipus))
        {
            throw new ArgumentException("A szoba típusa nem lehet üres.");
        }

        szobak.Add(uj);
        Ment();
    }

    public bool Torol(int szobaszam)
    {
        Szoba? talalt = Keres(szobaszam);
        if (talalt == null)
        {
            return false;
        }

        szobak.Remove(talalt);
        Ment();
        return true;
    }

    public Szoba? Keres(int szobaszam)
    {
        foreach (Szoba sz in szobak)
        {
            if (sz.Szobaszam == szobaszam)
            {
                return sz;
            }
        }
        return null;
    }

    public void Modosit(int szobaszam, string ujTipus, int ujFerohelyek, decimal ujAr)
    {
        Szoba? sz = Keres(szobaszam);
        if (sz == null)
        {
            throw new InvalidOperationException($"Nincs {szobaszam} számú szoba.");
        }

        if (ujAr <= 0)
        {
            throw new ArgumentException("Az éjszakai ár csak pozitív lehet.");
        }
        if (ujFerohelyek <= 0)
        {
            throw new ArgumentException("A férőhelyek száma csak pozitív lehet.");
        }
        if (string.IsNullOrWhiteSpace(ujTipus))
        {
            throw new ArgumentException("A szoba típusa nem lehet üres.");
        }

        sz.Tipus = ujTipus;
        sz.FerohelyekSzama = ujFerohelyek;
        sz.EjszakaiAr = ujAr;
        Ment();
    }

    public List<Szoba> Osszes()
    {
        return new List<Szoba>(szobak);
    }

    public List<Szoba> RendezArSzerint()
    {
        return szobak.OrderBy(s => s.EjszakaiAr).ToList();
    }
}
