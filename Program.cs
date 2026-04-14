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

            Console.Write("Szobaszám (1-30): ");
            int szobaSzam = int.Parse(Console.ReadLine());

            Console.Write("Érkezés dátuma (éééé-hh-nn): ");
            DateTime erkezes = DateTime.Parse(Console.ReadLine());

            Console.Write("Távozás dátuma (éééé-hh-nn): ");
            DateTime tavozas = DateTime.Parse(Console.ReadLine());

            Console.Write("Ár/éjszaka (Ft): ");
            int ar = int.Parse(Console.ReadLine());

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
            Console.WriteLine($"Foglalás rögzítve! (ID: {uj.Id})");
        }
    }
}