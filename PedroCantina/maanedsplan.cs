using System;
using System.Collections.Generic;
using System.Text;

namespace PedroCantina;

public class Maanedsplan
{
    public int Id { get; set; }
    public int Maaned { get; set; }
    public int Aar { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Maaned}/{Aar}";
    }
}