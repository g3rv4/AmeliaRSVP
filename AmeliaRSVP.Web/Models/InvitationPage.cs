using AmeliaRSVP.Core.Model;

namespace AmeliaRSVP.Web.Models;

public class InvitationPage
{
    public InvitationPage() { }

    public InvitationPage(Invitation invitation)
    {
        Invitation = invitation;
    }

    public Invitation Invitation { get; }
    public bool? RSVPResponse { get; set; }
    public int? ConfirmedAdults { get; set; }
    public int? ConfirmedKids { get; set; }
    public int? ConfirmedBabies { get; set; }
}
