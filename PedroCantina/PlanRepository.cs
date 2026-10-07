using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedroCantina
{
    public class PlanRepository
    {
        public PedroForbindelse db { get; set; }

        public PlanRepository()
        {
            db = new PedroForbindelse();
        }

        // 1. Vis en månedsplan
        public void VisMaanedsplan(int maaned, int aar)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "SELECT v.Dato, v.StartTid, v.SlutTid, m.Navn, m.ErLeder " +
                "FROM Månedsplan mp " +
                "JOIN Vagt v ON v.MånedsplanId = mp.Id " +
                "JOIN VagtMedarbejder vm ON vm.VagtId = v.Id " +
                "JOIN Medarbejder m ON m.Id = vm.MedarbejderId " +
                "WHERE mp.Måned = @Maaned AND mp.År = @Aar " +
                "ORDER BY v.Dato, v.StartTid, m.Navn",
                db.Forbindelse);
            command.Parameters.AddWithValue("@Maaned", maaned);
            command.Parameters.AddWithValue("@Aar", aar);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var dato = reader.GetDateTime(0);
                    var start = reader.GetTimeSpan(1);
                    var slut = reader.GetTimeSpan(2);
                    var navn = reader.GetString(3);
                    var erLeder = reader.GetBoolean(4);

                    Console.WriteLine($"{dato:dd-MM-yyyy} {start:hh\\:mm}-{slut:hh\\:mm}  {navn}" + (erLeder ? " (leder)" : ""));
                }
            }
            db.LukForbindelse();
        }

        // 2. Vis belastningen af de enkelte medarbejdere for en måned
        public void VisBelastningForMaaned(int maaned, int aar)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "SELECT m.Navn, COUNT(*) AS AntalVagter " +
                "FROM Medarbejder m " +
                "JOIN VagtMedarbejder vm ON vm.MedarbejderId = m.Id " +
                "JOIN Vagt v ON v.Id = vm.VagtId " +
                "JOIN Månedsplan mp ON mp.Id = v.MånedsplanId " +
                "WHERE mp.Måned = @Maaned AND mp.År = @Aar " +
                "GROUP BY m.Navn " +
                "ORDER BY AntalVagter DESC",
                db.Forbindelse);
            command.Parameters.AddWithValue("@Maaned", maaned);
            command.Parameters.AddWithValue("@Aar", aar);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var navn = reader.GetString(0);
                    var antalVagter = reader.GetInt32(1);

                    // En vagt er 5 timer (9-14 eller 14-19)
                    Console.WriteLine($"{navn}: {antalVagter} vagter ({antalVagter * 5} timer)");
                }
            }
            db.LukForbindelse();
        }

        // 3. Vis kontaktinfo på dem der ikke er på vagten, hvis en er syg
        public void VisKontaktVedSygdom(int vagtId)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "SELECT Navn, Telefon, ErLeder " +
                "FROM Medarbejder " +
                "WHERE Id NOT IN (SELECT MedarbejderId FROM VagtMedarbejder WHERE VagtId = @VagtId) " +
                "ORDER BY Navn",
                db.Forbindelse);
            command.Parameters.AddWithValue("@VagtId", vagtId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var navn = reader.GetString(0);
                    var telefon = reader.GetString(1);
                    var erLeder = reader.GetBoolean(2);

                    Console.WriteLine($"{navn}, tlf. {telefon}" + (erLeder ? " (leder)" : ""));
                }
            }
            db.LukForbindelse();
        }

        // 4. Vis belastningen af medarbejdere hen over året
        public void VisBelastningForAar(int aar)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "SELECT m.Navn, mp.Måned, COUNT(*) AS AntalVagter " +
                "FROM Medarbejder m " +
                "JOIN VagtMedarbejder vm ON vm.MedarbejderId = m.Id " +
                "JOIN Vagt v ON v.Id = vm.VagtId " +
                "JOIN Månedsplan mp ON mp.Id = v.MånedsplanId " +
                "WHERE mp.År = @Aar " +
                "GROUP BY m.Navn, mp.Måned " +
                "ORDER BY m.Navn, mp.Måned",
                db.Forbindelse);
            command.Parameters.AddWithValue("@Aar", aar);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var navn = reader.GetString(0);
                    var maaned = reader.GetInt32(1);
                    var antalVagter = reader.GetInt32(2);

                    Console.WriteLine($"{navn} - måned {maaned}: {antalVagter} vagter");
                }
            }
            db.LukForbindelse();
        }

        // 5a. Lav en ny månedsplan, returnerer det nye Id
        public int OpretMaanedsplan(int maaned, int aar)
        {
            int id = 0;

            db.OpretForbindelse();
            var command = new SqlCommand(
                "INSERT INTO Månedsplan (Måned, År) VALUES (@Maaned, @Aar); SELECT SCOPE_IDENTITY();",
                db.Forbindelse);
            command.Parameters.AddWithValue("@Maaned", maaned);
            command.Parameters.AddWithValue("@Aar", aar);

            var idObj = command.ExecuteScalar();
            if (idObj != null)
            {
                id = int.Parse(idObj.ToString()!);
            }
            db.LukForbindelse();

            return id;
        }

        // 5b. Lav en vagt i en månedsplan, returnerer det nye Id
        public int OpretVagt(int maanedsplanId, DateTime dato, TimeSpan startTid, TimeSpan slutTid)
        {
            int id = 0;

            db.OpretForbindelse();
            var command = new SqlCommand(
                "INSERT INTO Vagt (MånedsplanId, Dato, StartTid, SlutTid) VALUES (@PlanId, @Dato, @Start, @Slut); SELECT SCOPE_IDENTITY();",
                db.Forbindelse);
            command.Parameters.AddWithValue("@PlanId", maanedsplanId);
            command.Parameters.AddWithValue("@Dato", dato);
            command.Parameters.AddWithValue("@Start", startTid);
            command.Parameters.AddWithValue("@Slut", slutTid);

            var idObj = command.ExecuteScalar();
            if (idObj != null)
            {
                id = int.Parse(idObj.ToString()!);
            }
            db.LukForbindelse();

            return id;
        }

        // 5c. Sæt en medarbejder på en vagt
        public void TilfoejMedarbejderTilVagt(int vagtId, int medarbejderId)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "INSERT INTO VagtMedarbejder (VagtId, MedarbejderId) VALUES (@VagtId, @MedarbejderId)",
                db.Forbindelse);
            command.Parameters.AddWithValue("@VagtId", vagtId);
            command.Parameters.AddWithValue("@MedarbejderId", medarbejderId);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // 5d. Juster en plan, byt en medarbejder ud med en anden på en vagt
        public void SkiftMedarbejder(int vagtId, int gammelMedarbejderId, int nyMedarbejderId)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "UPDATE VagtMedarbejder SET MedarbejderId = @NyId " +
                "WHERE VagtId = @VagtId AND MedarbejderId = @GammelId",
                db.Forbindelse);
            command.Parameters.AddWithValue("@NyId", nyMedarbejderId);
            command.Parameters.AddWithValue("@VagtId", vagtId);
            command.Parameters.AddWithValue("@GammelId", gammelMedarbejderId);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // Slet en månedsplan med alle dens vagter
        public void SletMaanedsplan(int maanedsplanId)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
                "DELETE FROM VagtMedarbejder WHERE VagtId IN (SELECT Id FROM Vagt WHERE MånedsplanId = @Id); " +
                "DELETE FROM Vagt WHERE MånedsplanId = @Id; " +
                "DELETE FROM Månedsplan WHERE Id = @Id",
                db.Forbindelse);
            command.Parameters.AddWithValue("@Id", maanedsplanId);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }
    }
}
