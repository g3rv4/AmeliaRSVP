namespace AmeliaRSVP.Core.Model;

public class Invitation
{
    public int Row { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string WhatsAppNumbersG { get; set; }
    public string WhatsAppNumbersD { get; set; }
    public int MaxAdults { get; set; }
    public int MaxKids { get; set; }
    public int MaxBabies { get; set; }
    public DateTime? LastRSVP { get; set; }
    public bool? Response { get; set; }
    public int? ConfirmedAdults { get; set; }
    public int? ConfirmedKids { get; set; }
    public int? ConfirmedBabies { get; set; }

    public bool UsePlural => MaxAdults + MaxBabies + MaxKids > 1;
}
