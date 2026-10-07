using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace PedroCantina
{
    public class MedarbejderRepository : Iarbejder
    {
        public PedroForbindelse db { get; set; }

        public MedarbejderRepository()
        {
            db = new PedroForbindelse();
        }

        public List<Medarbejder> GetAllMedarbejdere()
        {
            var medarbejdere = new List<Medarbejder>();

            db.OpretForbindelse();
            var command = new SqlCommand("SELECT Id, Navn, Telefon, ErLeder FROM Medarbejder", db.Forbindelse);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Medarbejder medarbejder = new Medarbejder
                    {
                        Id = reader.GetInt32(0),
                        Navn = reader.GetString(1),
                        Telefon = reader.GetString(2),
                        ErLeder = reader.GetBoolean(3)
                    };
                    medarbejdere.Add(medarbejder);
                }
            }
            db.LukForbindelse();

            return medarbejdere;
        }

        public Medarbejder? GetMedarbejderById(int medarbejderId)
        {
            Medarbejder? medarbejder = null;

            db.OpretForbindelse();
            var command = new SqlCommand("SELECT Id, Navn, Telefon, ErLeder FROM Medarbejder WHERE Id = @Id", db.Forbindelse);
            command.Parameters.AddWithValue("@Id", medarbejderId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    medarbejder = new Medarbejder
                    {
                        Id = reader.GetInt32(0),
                        Navn = reader.GetString(1),
                        Telefon = reader.GetString(2),
                        ErLeder = reader.GetBoolean(3)
                    };
                }
            }
            db.LukForbindelse();

            return medarbejder;
        }

        public void CreateMedarbejder(Medarbejder medarbejder)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
              "INSERT INTO Medarbejder (Navn, Telefon, ErLeder) VALUES (@Navn, @Telefon, @ErLeder); SELECT SCOPE_IDENTITY();",
              db.Forbindelse);
            command.Parameters.AddWithValue("@Navn", medarbejder.Navn);
            command.Parameters.AddWithValue("@Telefon", medarbejder.Telefon);
            command.Parameters.AddWithValue("@ErLeder", medarbejder.ErLeder);

            var idObj = command.ExecuteScalar();
            if (idObj != null && int.TryParse(idObj.ToString(), out var id))
            {
                medarbejder.Id = id;
            }
            db.LukForbindelse();
        }

        public void UpdateMedarbejder(Medarbejder medarbejder)
        {
            db.OpretForbindelse();
            var command = new SqlCommand(
              "UPDATE Medarbejder SET Navn = @Navn, Telefon = @Telefon, ErLeder = @ErLeder WHERE Id = @Id",
              db.Forbindelse);
            command.Parameters.AddWithValue("@Navn", medarbejder.Navn);
            command.Parameters.AddWithValue("@Telefon", medarbejder.Telefon);
            command.Parameters.AddWithValue("@ErLeder", medarbejder.ErLeder);
            command.Parameters.AddWithValue("@Id", medarbejder.Id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        public void DeleteMedarbejder(int medarbejderId)
        {
            db.OpretForbindelse();

            // Fjern først medarbejderen fra sine vagter (ellers stopper fremmednøglen os)
            var fjernVagter = new SqlCommand("DELETE FROM VagtMedarbejder WHERE MedarbejderId = @Id", db.Forbindelse);
            fjernVagter.Parameters.AddWithValue("@Id", medarbejderId);
            fjernVagter.ExecuteNonQuery();

            var command = new SqlCommand("DELETE FROM Medarbejder WHERE Id = @Id", db.Forbindelse);
            command.Parameters.AddWithValue("@Id", medarbejderId);
            command.ExecuteNonQuery();

            db.LukForbindelse();
        }
    }
}
