namespace AmeliaRSVP.Core.Model;

public class Invitation
{
    public int Row { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string NameToUse => FromDaycare ? Name.FirstName() : Name;
    public string WhatsAppNumbersG { get; set; }
    public string WhatsAppNumbersD { get; set; }
    public string DaycareMomsPhone { get; set; }
    public string DaycareMomsName { get; set; }
    public string DaycareDadsPhone { get; set; }
    public string DaycareDadsName { get; set; }
    public bool FromDaycare => DaycareDadsPhone.HasValue() || DaycareMomsPhone.HasValue();
    public string Sala { get; set; }
    public int MaxAdults { get; set; }
    public int MaxKids { get; set; }
    public int MaxBabies { get; set; }
    public DateTime? LastRSVP { get; set; }
    public bool? Response { get; set; }
    public int? ConfirmedAdults { get; set; }
    public int? ConfirmedKids { get; set; }
    public int? ConfirmedBabies { get; set; }
    public DateTime? WhatsAppNumbersGSent { get; set; }
    public DateTime? WhatsAppNumbersDSent { get; set; }

    public bool UsePlural
    {
        get
        {
            if (FromDaycare && MaxKids == 1)
            {
                return false;
            }
            return MaxAdults + MaxBabies + MaxKids > 1;
        }
    }
}
