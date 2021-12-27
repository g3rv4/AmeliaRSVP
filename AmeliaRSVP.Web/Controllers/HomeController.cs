﻿using System.Collections.Immutable;
using AmeliaRSVP.Core.Helpers;
using AmeliaRSVP.Core.Model;
using AmeliaRSVP.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AmeliaRSVP.Web.Controllers;

public class HomeController : Controller
{
    private static ImmutableDictionary<string, Invitation> _invitations = ImmutableDictionary<string, Invitation>.Empty;
    private static DateTime? _lastUpdate;

    private static async Task<ImmutableDictionary<string, Invitation>> GetInvitationsAsync(bool refreshCache = false)
    {
        if (!_lastUpdate.HasValue || _invitations.IsEmpty || refreshCache ||
            DateTime.UtcNow.Subtract(_lastUpdate.Value).TotalMinutes >= 5)
        {
            _invitations = (await InvitationsHelper.GetInvitationsAsync()).ToImmutableDictionary(i => i.Code);
            _lastUpdate = DateTime.UtcNow;
        }

        return _invitations;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("{*code}", Order = 1)]
    [HttpGet]
    public async Task<IActionResult> InvitationPage(string code)
    {
        var invitations = await GetInvitationsAsync();
        if (!invitations.TryGetValue(code, out var invitation))
        {
            return NotFound();
        }

        return View(new InvitationPage(invitation));
    }

    [Route("{*code}", Order = 1)]
    [HttpPost]
    public async Task<IActionResult> InvitationPageSubmit(InvitationPage model, string code)
    {
        var invitations = await GetInvitationsAsync();
        if (!invitations.TryGetValue(code, out var invitation))
        {
            return NotFound();
        }

        var response = model.RSVPResponse ?? false;
        invitation.Response = response;
        invitation.ConfirmedAdults = response ? Math.Min(invitation.MaxAdults, model.ConfirmedAdults ?? 0) : 0;
        invitation.ConfirmedKids = response ? Math.Min(invitation.MaxKids, model.ConfirmedKids ?? 0) : 0;
        invitation.ConfirmedBabies = response ? Math.Min(invitation.MaxBabies, model.ConfirmedBabies ?? 0) : 0;
        invitation.LastRSVP = DateTime.UtcNow;

        await InvitationsHelper.SaveInvitationResponse(invitation);
        return Redirect(response ? "/woohooo" : "/booo");
    }

    [Route("refresh")]
    public IActionResult Refresh()
    {
        _invitations = ImmutableDictionary<string, Invitation>.Empty;
        ;
        return Content("ok");
    }

    [Route("woohooo")]
    public IActionResult Woohoo()
    {
        return Content("que alegría");
    }

    [Route("booo")]
    public IActionResult Boo()
    {
        return Content("Si te arrepentís, volvé por acá");
    }
}
