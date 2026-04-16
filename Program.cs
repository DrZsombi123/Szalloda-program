using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace szallodaprogram
{
    class Program
    {
        static List<Foglalas> foglalasok = new List<Foglalas>();
        
        static void Main(string[] args)
        {
            Betoltes();
            bool fut = true;

            while (fut)
            {
                Console.WriteLine();
                Console.WriteLine("=== Szálloda Nyilvántartó ===");
                Console.WriteLine("1. Foglalások listázása");
                Console.WriteLine("2. Új foglalás felvétele");
                Console.WriteLine("3. Foglalás módosítása");
                Console.WriteLine("4. Foglalás törlése");
                Console.WriteLine("5. Keresés vendégnév alapján");
                Console.WriteLine("6. Szabad szobák keresése");
                Console.WriteLine("0. Kilépés");
                Console.Write("Válasszon: ");

                string input = Console.ReadLine();
                int.TryParse(input, out int valasztas);

                switch (valasztas)
                {
                    case 1: Listazas(); break;
                    case 2: UjFoglalas(); break;
                    case 3: Modositas(); break;
                    case 4: Torles(); break;
                    case 5: KeresesNevAlapjan(); break;
                    case 6: SzabadSzobak(); break;
                    case 0: fut = false; break;
                    default:
                        Console.WriteLine("Érvénytelen menüpont!");
                        break;
                }
            }



            Console.WriteLine("Viszontlátásra!");
        }
        static void Listazas()
        {
            if (foglalasok.Count == 0)
            {
                Console.WriteLine("Nincsenek foglalások.");
                return;
            }

            foreach (Foglalas f in foglalasok)
            {
                Console.WriteLine($"ID: {f.Id}, Vendég: {f.VendegNev}, Szoba: {f.SzobaSzam}, {f.Erkezes:yyyy-MM-dd} - {f.Tavozas:yyyy-MM-dd}, Összesen: {f.TeljesAr} Ft");
            }
        }

        static void UjFoglalas()
        {
            Console.WriteLine("\n--- Új foglalás ---");

            Console.Write("Vendég neve: ");
            string nev = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nev))
            {
                Console.WriteLine("A név nem lehet üres!");
                return;
            }

            Console.Write("Szobaszám (1-30): ");
            if (!int.TryParse(Console.ReadLine(), out int szobaSzam) || szobaSzam < 1 || szobaSzam > 30)
            {
                Console.WriteLine("Érvénytelen szobaszám! (1-30 között kell lennie)");
                return;
            }

            Console.Write("Érkezés dátuma (éééé-hh-nn): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime erkezes))
            {
                Console.WriteLine("Érvénytelen dátum!");
                return;
            }

            Console.Write("Távozás dátuma (éééé-hh-nn): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime tavozas))
            {
                Console.WriteLine("Érvénytelen dátum!");
                return;
            }

            if (tavozas <= erkezes)
            {
                Console.WriteLine("A távozás dátumának az érkezés után kell lennie!");
                return;
            }

            Console.Write("Ár/éjszaka (Ft): ");
            if (!int.TryParse(Console.ReadLine(), out int ar) || ar <= 0)
            {
                Console.WriteLine("Az árnak pozitív számnak kell lennie!");
                return;
            }

            int kovetkezoId = foglalasok.Count == 0 ? 1 : foglalasok.Max(f => f.Id) + 1;

            Foglalas uj = new Foglalas
            {
                Id = kovetkezoId,
                VendegNev = nev,
                SzobaSzam = szobaSzam,
                Erkezes = erkezes,
                Tavozas = tavozas,
                ArPerEjszaka = ar
            };
            if (VanUtkozes(uj, -1))
            {
                Console.WriteLine("Ez a szoba ebben az időszakban már foglalt!");
                return;
            }
            foglalasok.Add(uj);
            Mentes();
            Console.WriteLine($"Foglalás rögzítve! (ID: {uj.Id}, Összesen: {uj.TeljesAr} Ft)");
        }
        static void Modositas()
        {
            if (foglalasok.Count == 0)
            {
                Console.WriteLine("Nincsenek foglalások.");
                return;
            }

            Listazas();
            Console.Write("\nMódosítandó foglalás ID-ja: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Érvénytelen ID!");
                return;
            }

            Foglalas f = foglalasok.FirstOrDefault(x => x.Id == id);
            if (f == null)
            {
                Console.WriteLine("Nem található ilyen foglalás!");
                return;
            }

            Console.WriteLine("(Nyomjon Entert, ha nem akarja módosítani az adott mezőt)");

            Console.Write($"Vendég neve ({f.VendegNev}): ");
            string nev = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nev))
                f.VendegNev = nev;

            Console.Write($"Szobaszám ({f.SzobaSzam}): ");
            string szobaTxt = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(szobaTxt))
            {
                if (int.TryParse(szobaTxt, out int szoba) && szoba >= 1 && szoba <= 30)
                    f.SzobaSzam = szoba;
            }

            Console.Write($"Érkezés ({f.Erkezes:yyyy-MM-dd}): ");
            string erkTxt = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(erkTxt) && DateTime.TryParse(erkTxt, out DateTime erk))
                f.Erkezes = erk;

            Console.Write($"Távozás ({f.Tavozas:yyyy-MM-dd}): ");
            string tavTxt = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tavTxt) && DateTime.TryParse(tavTxt, out DateTime tav))
                f.Tavozas = tav;

            Console.Write($"Ár/éjszaka ({f.ArPerEjszaka}): ");
            string arTxt = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(arTxt) && int.TryParse(arTxt, out int ar) && ar > 0)
                f.ArPerEjszaka = ar;
            if (VanUtkozes(f, f.Id))
            {
                Console.WriteLine("Ütközés! Ez a szoba ebben az időszakban már foglalt!");
                return;
            }
            Mentes();
            Console.WriteLine($"Foglalás módosítva! (Összesen: {f.TeljesAr} Ft)");
        }
        static void Betoltes()
        {
            if (!File.Exists(fajlNev))
                return;

            try
            {
                string[] sorok = File.ReadAllLines(fajlNev);
                foreach (string sor in sorok)
                {
                    if (!string.IsNullOrWhiteSpace(sor))
                    {
                        foglalasok.Add(Foglalas.CsvBol(sor));
                    }
                }
                Console.WriteLine($"{foglalasok.Count} foglalás betöltve.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a fájl betöltésekor: {ex.Message}");
            }
        }
        static void Torles()
        {
            if (foglalasok.Count == 0)
            {
                Console.WriteLine("Nincsenek foglalások.");
                return;
            }

            Listazas();
            Console.Write("\nTörlendő foglalás ID-ja: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Érvénytelen ID!");
                return;
            }

            Foglalas f = foglalasok.FirstOrDefault(x => x.Id == id);
            if (f == null)
            {
                Console.WriteLine("Nem található ilyen foglalás!");
                return;
            }

            Console.Write($"Biztosan törli a(z) {f.VendegNev} foglalását? (i/n): ");
            string valasz = Console.ReadLine();
            if (valasz?.ToLower() == "i")
            {
                foglalasok.Remove(f);
                Mentes();
                Console.WriteLine("Foglalás törölve!");
            }
            else
            {
                Console.WriteLine("Törlés megszakítva.");
            }
        }
        static void KeresesNevAlapjan()
        {
            Console.Write("Keresett vendégnév (részlet): ");
            string kereses = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(kereses))
            {
                Console.WriteLine("Üres keresés!");
                return;
            }

            List<Foglalas> talalatok = foglalasok
                .Where(f => f.VendegNev.ToLower().Contains(kereses.ToLower()))
                .ToList();

            if (talalatok.Count == 0)
            {
                Console.WriteLine("Nincs találat.");
                return;
            }

            Console.WriteLine($"\n{talalatok.Count} találat:");
            foreach (Foglalas f in talalatok)
            {
                Console.WriteLine($"ID: {f.Id}, {f.VendegNev}, Szoba: {f.SzobaSzam}, {f.Erkezes:yyyy-MM-dd} - {f.Tavozas:yyyy-MM-dd}, {f.TeljesAr} Ft");
            }
        }
        static bool VanUtkozes(Foglalas uj, int kihagyId)
        {
            return foglalasok.Any(f =>
                f.Id != kihagyId &&
                f.SzobaSzam == uj.SzobaSzam &&
                f.Erkezes < uj.Tavozas &&
                uj.Erkezes < f.Tavozas);
        }
        static void SzabadSzobak()
        {
            Console.WriteLine("\n--- Szabad szobák keresése ---");

            Console.Write("Időszak kezdete (éééé-hh-nn): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime tol))
            {
                Console.WriteLine("Érvénytelen dátum formátum!");
                return;
            }

            Console.Write("Időszak vége (éééé-hh-nn): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime ig))
            {
                Console.WriteLine("Érvénytelen dátum formátum!");
                return;
            }

            if (ig <= tol)
            {
                Console.WriteLine("A végdátumnak a kezdődátum után kell lennie!");
                return;
            }

            List<int> foglaltSzobak = foglalasok
                .Where(f => f.Erkezes < ig && tol < f.Tavozas)
                .Select(f => f.SzobaSzam)
                .Distinct()
                .ToList();

            List<int> szabadSzobak = new List<int>();
            for (int i = 1; i <= 30; i++)
            {
                if (!foglaltSzobak.Contains(i))
                    szabadSzobak.Add(i);
            }

            if (szabadSzobak.Count == 0)
            {
                Console.WriteLine("Ebben az időszakban nincs szabad szoba!");
            }
            else
            {
                Console.WriteLine($"\nSzabad szobák ({tol:yyyy-MM-dd} - {ig:yyyy-MM-dd}): {string.Join(", ", szabadSzobak)}");
                Console.WriteLine($"Összesen {szabadSzobak.Count} szabad szoba a 30-ból.");
            }
        }


    }
}