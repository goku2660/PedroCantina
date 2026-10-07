using System;
using System.Collections.Generic;
using System.Text;

namespace PedroCantina
{
    public interface Iarbejder
    {
        List<Medarbejder> GetAllMedarbejdere();

        Medarbejder? GetMedarbejderById(int Id);

        public void CreateMedarbejder(Medarbejder medarbejder);

        public void UpdateMedarbejder(Medarbejder medarbejder);

        public void DeleteMedarbejder(int Id);
    }
}
