using System;
using System.Collections.Generic;
using System.Text;

namespace PedroCantina
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Forbindelsen hentes fra User Secrets ("Pcstring") inde i PedroForbindelse
            Iarbejder repository = new MedarbejderRepository();
            var plan = new PlanRepository();

            Console.WriteLine("Pedros Cantina");
            Console.WriteLine("==============");

            bool kører = true;
            while (kører)
            {
                Console.WriteLine();
                Console.WriteLine("1. Vis alle medarbejdere");
                Console.WriteLine("2. Find medarbejder");
                Console.WriteLine("3. Opret medarbejder");
                Console.WriteLine("4. Opdater medarbejder");
                Console.WriteLine("5. Slet medarbejder");
                Console.WriteLine("6. Vis månedsplan");
                Console.WriteLine("7. Belastning for en måned");
                Console.WriteLine("8. Kontakt hvis en er syg");
                Console.WriteLine("9. Belastning over året");
                Console.WriteLine("10. Lav ny månedsplan med en vagt");
                Console.WriteLine("11. Juster en vagt (byt medarbejder)");
                Console.WriteLine("0. Afslut");
                Console.Write("Vælg: ");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            {
                                var alle = repository.GetAllMedarbejdere();
                                if (alle.Count == 0)
                                {
                                    Console.WriteLine("Ingen medarbejdere fundet.");
                                }
                                foreach (var m in alle)
                                {
                                    Console.WriteLine(m);
                                }
                                break;
                            }

                        case "2":
                            {
                                Console.Write("Id: ");
                                int id = int.Parse(Console.ReadLine()!);
                                var m = repository.GetMedarbejderById(id);
                                Console.WriteLine(m != null ? m.ToString() : "Medarbejder ikke fundet.");
                                break;
                            }

                        case "3":
                            {
                                var ny = new Medarbejder();
                                Console.Write("Navn: ");
                                ny.Navn = Console.ReadLine()!;
                                Console.Write("Telefon: ");
                                ny.Telefon = Console.ReadLine()!;
                                repository.CreateMedarbejder(ny);
                                Console.WriteLine("Oprettet: " + ny);
                                break;
                            }

                        case "4":
                            {
                                Console.Write("Id på medarbejder der skal opdateres: ");
                                int id = int.Parse(Console.ReadLine()!);
                                var m = repository.GetMedarbejderById(id);
                                if (m == null)
                                {
                                    Console.WriteLine("Medarbejder ikke fundet.");
                                    break;
                                }
                                Console.WriteLine("Nuværende: " + m);
                                Console.Write("Nyt navn: ");
                                m.Navn = Console.ReadLine()!;
                                Console.Write("Ny telefon: ");
                                m.Telefon = Console.ReadLine()!;
                                repository.UpdateMedarbejder(m);
                                Console.WriteLine("Opdateret: " + m);
                                break;
                            }

                        case "5":
                            {
                                Console.Write("Id på medarbejder der skal slettes: ");
                                int id = int.Parse(Console.ReadLine()!);
                                var m = repository.GetMedarbejderById(id);
                                if (m == null)
                                {
                                    Console.WriteLine("Medarbejder ikke fundet.");
                                    break;
                                }
                                repository.DeleteMedarbejder(id);
                                Console.WriteLine("Slettet: " + m);
                                break;
                            }

                        case "6":
                            {
                                Console.Write("Måned: ");
                                int maaned = int.Parse(Console.ReadLine()!);
                                Console.Write("År: ");
                                int aar = int.Parse(Console.ReadLine()!);
                                plan.VisMaanedsplan(maaned, aar);
                                break;
                            }

                        case "7":
                            {
                                Console.Write("Måned: ");
                                int maaned = int.Parse(Console.ReadLine()!);
                                Console.Write("År: ");
                                int aar = int.Parse(Console.ReadLine()!);
                                plan.VisBelastningForMaaned(maaned, aar);
                                break;
                            }

                        case "8":
                            {
                                Console.Write("Id på vagten hvor en er syg: ");
                                int vagtId = int.Parse(Console.ReadLine()!);
                                plan.VisKontaktVedSygdom(vagtId);
                                break;
                            }

                        case "9":
                            {
                                Console.Write("År: ");
                                int aar = int.Parse(Console.ReadLine()!);
                                plan.VisBelastningForAar(aar);
                                break;
                            }

                        case "10":
                            {
                                Console.Write("Måned: ");
                                int maaned = int.Parse(Console.ReadLine()!);
                                Console.Write("År: ");
                                int aar = int.Parse(Console.ReadLine()!);
                                int planId = plan.OpretMaanedsplan(maaned, aar);

                                Console.Write("Dato for vagten (fx 2026-11-01): ");
                                DateTime dato = DateTime.Parse(Console.ReadLine()!);
                                Console.Write("Starttid (9 eller 14): ");
                                int start = int.Parse(Console.ReadLine()!);
                                int vagtId = plan.OpretVagt(planId, dato, new TimeSpan(start, 0, 0), new TimeSpan(start + 5, 0, 0));

                                Console.Write("Id på leder: ");
                                plan.TilfoejMedarbejderTilVagt(vagtId, int.Parse(Console.ReadLine()!));
                                Console.Write("Id på medarbejder: ");
                                plan.TilfoejMedarbejderTilVagt(vagtId, int.Parse(Console.ReadLine()!));

                                Console.WriteLine("Oprettet vagt med id " + vagtId + ":");
                                plan.VisMaanedsplan(maaned, aar);
                                break;
                            }

                        case "11":
                            {
                                Console.Write("Id på vagten: ");
                                int vagtId = int.Parse(Console.ReadLine()!);
                                Console.Write("Id på medarbejder der er syg: ");
                                int gammelId = int.Parse(Console.ReadLine()!);
                                Console.Write("Id på medarbejder der tager vagten: ");
                                int nyId = int.Parse(Console.ReadLine()!);
                                plan.SkiftMedarbejder(vagtId, gammelId, nyId);
                                Console.WriteLine("Vagten er justeret.");
                                break;
                            }

                        case "0":
                            kører = false;
                            break;

                        default:
                            Console.WriteLine("Ugyldigt valg.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fejl: " + ex.Message);
                }
            }
        }
    }
}
