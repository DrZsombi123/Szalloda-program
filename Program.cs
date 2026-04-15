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
                    case 3: Console.WriteLine("[módosítás - később]"); break;
                    case 4: Console.WriteLine("[törlés - később]"); break;
                    case 5: Console.WriteLine("[keresés - később]"); break;
                    case 6: Console.WriteLine("[szabad szobák - később]"); break;
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

            foglalasok.Add(uj);
            Mentes();
            Console.WriteLine($"Foglalás rögzítve! (ID: {uj.Id}, Összesen: {uj.TeljesAr} Ft)");
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
    }
}