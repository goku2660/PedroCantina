using System;
using System.Collections.Generic;
using System.Text;
    
        namespace PedroCantina;

    public class Medarbejder
    {
        public int Id { get; set; }
        public string Navn { get; set; } = "";
        public string Telefon { get; set; } = "";
        public bool ErLeder { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Navn}, tlf. {Telefon}" + (ErLeder ? " (leder)" : "");
        }
    }

