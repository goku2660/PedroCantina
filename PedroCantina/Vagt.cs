namespace PedroCantina;

public class Vagt
{
    public int Id { get; set; }
    public int MånedsplanId { get; set; }
    public string Dato { get; set; } = "";
    public string StartTid { get; set; } = "";
    public string SlutTid { get; set; } = "";

    public override string ToString()
    {
        return $"{Id}: {Dato} {StartTid}-{SlutTid}";
    }
}
