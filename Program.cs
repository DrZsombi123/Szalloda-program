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
                    case 1: Console.WriteLine("[listázás - később]"); break;
                    case 2: Console.WriteLine("[új foglalás - később]"); break;
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
    }
}