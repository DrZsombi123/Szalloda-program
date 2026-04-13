using System;
using System.Collections.Generic;
using System.IO;

namespace SzallodaApp;

public class Program
{
    private const string SzobakCsvUt = "data/szobak.csv";
    private const string FoglalasokCsvUt = "data/foglalasok.csv";

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=====================================");
        Console.WriteLine("    SZÁLLODA NYILVÁNTARTÓ PROGRAM    ");
        Console.WriteLine("=====================================");
        Console.WriteLine();

        SzobaKezelo szobaKezelo = new SzobaKezelo(SzobakCsvUt);
        FoglalasKezelo foglalasKezelo = new FoglalasKezelo(FoglalasokCsvUt, szobaKezelo);

        Console.WriteLine($"Betöltve: {szobaKezelo.Osszes().Count} szoba, {foglalasKezelo.Osszes().Count} foglalás.");
        Console.WriteLine();

        bool kilepes = false;
        while (!kilepes)
        {
            Console.WriteLine();
            Console.WriteLine("=== FŐMENÜ ===");
            Console.WriteLine("1. Szobák kezelése");
            Console.WriteLine("2. Foglalások kezelése");
            Console.WriteLine("3. Szabad szobák keresése (dátum szerint)");
            Console.WriteLine("0. Kilépés");
            int valasztas = Beolvaso.KerEgesz("Választás: ", 0, 3);

            switch (valasztas)
            {
                case 1:
                    SzobaMenu(szobaKezelo);
                    break;
                case 2:
                    FoglalasMenu(foglalasKezelo, szobaKezelo);
                    break;
                case 3:
                    SzabadSzobakKereses(foglalasKezelo);
                    break;
                case 0:
                    kilepes = true;
                    break;
            }
        }

        Console.WriteLine();
        Console.WriteLine("Viszlát! Az adatok elmentve.");
    }

    // ===== SZOBA MENÜ =====


    private static void SzobaMenu(SzobaKezelo szk)
    {
        bool vissza = false;
        while (!vissza)
        {
            Console.WriteLine();
            Console.WriteLine("--- SZOBÁK ---");
            Console.WriteLine("1. Új szoba felvétele");
            Console.WriteLine("2. Szobák listázása");
            Console.WriteLine("3. Szoba keresése (szobaszám szerint)");
            Console.WriteLine("4. Szoba módosítása");
            Console.WriteLine("5. Szoba törlése");
            Console.WriteLine("6. Rendezés ár szerint");
            Console.WriteLine("0. Vissza a főmenübe");
            int v = Beolvaso.KerEgesz("Választás: ", 0, 6);

            switch (v)
            {
                case 1: UjSzoba(szk); break;
                case 2: SzobakListazasa(szk); break;
                case 3: SzobaKereses(szk); break;
                case 4: SzobaModositasa(szk); break;
                case 5: SzobaTorlese(szk); break;
                case 6: SzobakRendezve(szk); break;
                case 0: vissza = true; break;
            }
        }
    }

    private static void UjSzoba(SzobaKezelo szk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Új szoba felvétele --");
        try
        {
            int szam = Beolvaso.KerEgesz("Szobaszám: ", 1, 9999);
            string tipus = Beolvaso.KerSzoveg("Típus (pl. Egyágyas, Kétágyas, Lakosztály): ");
            int ferohely = Beolvaso.KerEgesz("Férőhelyek száma: ", 1, 20);
            decimal ar = Beolvaso.KerDecimal("Éjszakai ár (Ft): ", 1);

            Szoba uj = new Szoba(szam, tipus, ferohely, ar);
            szk.Hozzaad(uj);
            Console.WriteLine("Szoba sikeresen felvéve.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HIBA: {ex.Message}");
        }
    }

    private static void SzobakListazasa(SzobaKezelo szk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Szobák listája --");
        List<Szoba> lista = szk.Osszes();
        if (lista.Count == 0)
        {
            Console.WriteLine("(nincsenek szobák)");
            return;
        }
        foreach (Szoba sz in lista)
        {
            Console.WriteLine(sz);
        }
    }

    private static void SzobaKereses(SzobaKezelo szk)
    {
        Console.WriteLine();
        int szam = Beolvaso.KerEgesz("Keresett szobaszám: ", 1, 9999);
        Szoba? talalt = szk.Keres(szam);
        if (talalt == null)
        {
            Console.WriteLine($"Nincs {szam} számú szoba.");
        }
        else
        {
            Console.WriteLine("Megtaláltam: " + talalt);
        }
    }

    private static void SzobaModositasa(SzobaKezelo szk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Szoba módosítása --");
        try
        {
            int szam = Beolvaso.KerEgesz("Melyik szobaszámot módosítod? ", 1, 9999);
            Szoba? letezo = szk.Keres(szam);
            if (letezo == null)
            {
                Console.WriteLine($"Nincs {szam} számú szoba.");
                return;
            }
            Console.WriteLine("Jelenlegi: " + letezo);

            string tipus = Beolvaso.KerSzoveg("Új típus: ");
            int ferohely = Beolvaso.KerEgesz("Új férőhelyek száma: ", 1, 20);
            decimal ar = Beolvaso.KerDecimal("Új éjszakai ár (Ft): ", 1);

            szk.Modosit(szam, tipus, ferohely, ar);
            Console.WriteLine("Sikeresen módosítva.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HIBA: {ex.Message}");
        }
    }

    private static void SzobaTorlese(SzobaKezelo szk)
    {
        Console.WriteLine();
        int szam = Beolvaso.KerEgesz("Törlendő szobaszám: ", 1, 9999);
        Szoba? letezo = szk.Keres(szam);
        if (letezo == null)
        {
            Console.WriteLine($"Nincs {szam} számú szoba.");
            return;
        }
        Console.WriteLine("Törlendő: " + letezo);
        bool megerosit = Beolvaso.KerIgenNem("Biztosan törlöd?");
        if (megerosit)
        {
            szk.Torol(szam);
            Console.WriteLine("Törölve.");
        }
        else
        {
            Console.WriteLine("Mégsem.");
        }
    }

    private static void SzobakRendezve(SzobaKezelo szk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Szobák ár szerint (olcsóbbtól) --");
        List<Szoba> lista = szk.RendezArSzerint();
        if (lista.Count == 0)
        {
            Console.WriteLine("(nincsenek szobák)");
            return;
        }
        foreach (Szoba sz in lista)
        {
            Console.WriteLine(sz);
        }
    }

    // ===== FOGLALÁS MENÜ =====

    // A foglalási menü kezeli az új foglalásokat, módosítást, törlést,
    // és a mai érkezések listázását.

    private static void FoglalasMenu(FoglalasKezelo fk, SzobaKezelo szk)
    {
        bool vissza = false;
        while (!vissza)
        {
            Console.WriteLine();
            Console.WriteLine("--- FOGLALÁSOK ---");
            Console.WriteLine("1. Új foglalás felvétele");
            Console.WriteLine("2. Foglalások listázása");
            Console.WriteLine("3. Keresés vendég név szerint");
            Console.WriteLine("4. Foglalás dátumának módosítása");
            Console.WriteLine("5. Foglalás törlése");
            Console.WriteLine("6. Mai érkezések");
            Console.WriteLine("0. Vissza a főmenübe");
            int v = Beolvaso.KerEgesz("Választás: ", 0, 6);

            switch (v)
            {
                case 1: UjFoglalas(fk, szk); break;
                case 2: FoglalasokListazasa(fk); break;
                case 3: FoglalasKeresesNev(fk); break;
                case 4: FoglalasModositasa(fk); break;
                case 5: FoglalasTorlese(fk); break;
                case 6: MaiErkezesek(fk); break;
                case 0: vissza = true; break;
            }
        }
    }

    private static void UjFoglalas(FoglalasKezelo fk, SzobaKezelo szk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Új foglalás --");
        try
        {
            int szam = Beolvaso.KerEgesz("Szobaszám: ", 1, 9999);
            Szoba? szoba = szk.Keres(szam);
            if (szoba == null)
            {
                Console.WriteLine($"HIBA: nincs {szam} számú szoba.");
                return;
            }
            Console.WriteLine("Foglalandó szoba: " + szoba);

            string nev = Beolvaso.KerSzoveg("Vendég neve: ");
            string tel = Beolvaso.KerSzoveg("Telefonszám: ", lehetUres: true);
            DateTime erkezes = Beolvaso.KerDatum("Érkezés dátuma");
            DateTime tavozas = Beolvaso.KerDatum("Távozás dátuma");

            Foglalas uj = fk.UjFoglalas(szam, nev, tel, erkezes, tavozas);
            Console.WriteLine("Foglalás sikeres!");
            Console.WriteLine(uj);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HIBA: {ex.Message}");
        }
    }

    private static void FoglalasokListazasa(FoglalasKezelo fk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Foglalások listája --");
        List<Foglalas> lista = fk.Osszes();
        if (lista.Count == 0)
        {
            Console.WriteLine("(nincsenek foglalások)");
            return;
        }
        foreach (Foglalas f in lista)
        {
            Console.WriteLine(f);
        }
    }

    private static void FoglalasKeresesNev(FoglalasKezelo fk)
    {
        Console.WriteLine();
        string nev = Beolvaso.KerSzoveg("Keresett vendég név (vagy név-rész): ");
        List<Foglalas> talaltak = fk.KeresNevSzerint(nev);
        if (talaltak.Count == 0)
        {
            Console.WriteLine("Nincs találat.");
            return;
        }
        Console.WriteLine($"{talaltak.Count} találat:");
        foreach (Foglalas f in talaltak)
        {
            Console.WriteLine(f);
        }
    }

    private static void FoglalasModositasa(FoglalasKezelo fk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Foglalás dátumának módosítása --");
        try
        {
            int id = Beolvaso.KerEgesz("Foglalás azonosítója (Id): ", 1);
            Foglalas? letezo = fk.Keres(id);
            if (letezo == null)
            {
                Console.WriteLine($"Nincs #{id} számú foglalás.");
                return;
            }
            Console.WriteLine("Jelenlegi: " + letezo);

            DateTime ujErk = Beolvaso.KerDatum("Új érkezés dátuma");
            DateTime ujTav = Beolvaso.KerDatum("Új távozás dátuma");

            fk.ModositDatum(id, ujErk, ujTav);
            Console.WriteLine("Sikeresen módosítva.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HIBA: {ex.Message}");
        }
    }

    private static void FoglalasTorlese(FoglalasKezelo fk)
    {
        Console.WriteLine();
        int id = Beolvaso.KerEgesz("Törlendő foglalás Id: ", 1);
        Foglalas? letezo = fk.Keres(id);
        if (letezo == null)
        {
            Console.WriteLine($"Nincs #{id} számú foglalás.");
            return;
        }
        Console.WriteLine("Törlendő: " + letezo);
        bool megerosit = Beolvaso.KerIgenNem("Biztosan törlöd?");
        if (megerosit)
        {
            fk.Torol(id);
            Console.WriteLine("Törölve.");
        }
        else
        {
            Console.WriteLine("Mégsem.");
        }
    }

    private static void MaiErkezesek(FoglalasKezelo fk)
    {
        Console.WriteLine();
        Console.WriteLine($"-- Mai érkezések ({DateTime.Today:yyyy-MM-dd}) --");
        List<Foglalas> lista = fk.MaiErkezesek();
        if (lista.Count == 0)
        {
            Console.WriteLine("Ma nincs érkező vendég.");
            return;
        }
        foreach (Foglalas f in lista)
        {
            Console.WriteLine(f);
        }
    }

    // ===== SZABAD SZOBÁK KERESÉSE =====

    // Ez az a funkció, amit demón kiemelünk, mert Excelben ezt
    // nagyon nehéz lenne megbízhatóan megcsinálni.


    private static void SzabadSzobakKereses(FoglalasKezelo fk)
    {
        Console.WriteLine();
        Console.WriteLine("-- Szabad szobák keresése --");
        try
        {
            DateTime erkezes = Beolvaso.KerDatum("Érkezés dátuma");
            DateTime tavozas = Beolvaso.KerDatum("Távozás dátuma");

            List<Szoba> szabadak = fk.SzabadSzobak(erkezes, tavozas);
            if (szabadak.Count == 0)
            {
                Console.WriteLine("Ebben az időszakban nincs szabad szoba.");
                return;
            }

            int ejszakak = (tavozas - erkezes).Days;
            Console.WriteLine($"{szabadak.Count} szabad szoba ({ejszakak} éjszakára):");
            foreach (Szoba sz in szabadak)
            {
                decimal teljes = ejszakak * sz.EjszakaiAr;
                Console.WriteLine($"  {sz}  ->  összesen: {teljes,10:N0} Ft");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HIBA: {ex.Message}");
        }
    }
}
