using AmeliaRSVP.Core;
using AmeliaRSVP.Core.Helpers;

Config.Init(args[0]);
var invitations = await InvitationsHelper.GetInvitationsAsync();
var current = invitations.First(i => i.Code == "456");

current.Response = true;
current.ConfirmedAdults = 4;
current.ConfirmedKids = 5;
current.LastRSVP = DateTime.UtcNow;

await InvitationsHelper.SaveInvitationResponse(current);
