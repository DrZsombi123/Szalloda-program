using System;
using System.Globalization;

namespace SzallodaApp;

public static class Beolvaso
{
    public static string KerSzoveg(string prompt, bool lehetUres = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input == null)
            {
                input = "";
            }

            input = input.Trim();

            if (!lehetUres && string.IsNullOrEmpty(input))
            {
                Console.WriteLine("  Hiba: a mező nem lehet üres. Próbáld újra.");
                continue;
            }

            return input;
        }
    }

    public static int KerEgesz(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            try
            {
                int ertek = int.Parse(input ?? "", CultureInfo.InvariantCulture);
                if (ertek < min || ertek > max)
                {
                    Console.WriteLine($"  Hiba: a szám {min} és {max} között kell legyen.");
                    continue;
                }
                return ertek;
            }
            catch (FormatException)
            {
                Console.WriteLine("  Hiba: érvénytelen egész szám. Próbáld újra.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("  Hiba: túl nagy/kicsi szám. Próbáld újra.");
            }
        }
    }

    public static decimal KerDecimal(string prompt, decimal min = 0)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            // Vesszőt pontra cseréljük, hogy magyar beviteltől is működjön.
            if (input != null)
            {
                input = input.Replace(',', '.');
            }

            try
            {
                decimal ertek = decimal.Parse(input ?? "", CultureInfo.InvariantCulture);
                if (ertek < min)
                {
                    Console.WriteLine($"  Hiba: a szám legalább {min} kell legyen.");
                    continue;
                }
                return ertek;
            }
            catch (FormatException)
            {
                Console.WriteLine("  Hiba: érvénytelen szám. Próbáld újra.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("  Hiba: túl nagy/kicsi szám. Próbáld újra.");
            }
        }
    }

    public static DateTime KerDatum(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (formátum: éééé-hh-nn): ");
            string? input = Console.ReadLine();

            try
            {
                DateTime ertek = DateTime.ParseExact(
                    input ?? "",
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture);
                return ertek;
            }
            catch (FormatException)
            {
                Console.WriteLine("  Hiba: érvénytelen dátum. Használd az éééé-hh-nn formátumot, pl. 2026-04-15.");
            }
        }
    }

    public static bool KerIgenNem(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (i/n): ");
            string? input = Console.ReadLine();
            if (input == null)
            {
                continue;
            }
            input = input.Trim().ToLower();

            if (input == "i" || input == "igen" || input == "y" || input == "yes")
            {
                return true;
            }
            if (input == "n" || input == "nem" || input == "no")
            {
                return false;
            }

            Console.WriteLine("  Kérlek, i vagy n választ adj.");
        }
    }
}
