using System.Collections.Immutable;
using System.Net.Http.Headers;
using AmeliaRSVP.Core.Model;
using FastMember;
using Jil;

namespace AmeliaRSVP.Core.Helpers;

public class InvitationsHelper
{
    private static readonly ImmutableArray<string> _fieldsToUpdate = new[] { nameof(Invitation.Response), nameof(Invitation.ConfirmedAdults), nameof(Invitation.ConfirmedKids), nameof(Invitation.ConfirmedBabies), nameof(Invitation.LastRSVP) }.ToImmutableArray();

    private static Dictionary<string, Func<string, object>> _parserByField;
    private static readonly TypeAccessor _accessor = TypeAccessor.Create(typeof(Invitation));

    private static async Task<HttpClient> GetClientAsync()
    {
        var cred = Config.Instance.AccountCredential;
        var accessToken = await cred.GetAccessTokenForRequestAsync();

        return new HttpClient { DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) } };
    }

    public static async Task<ImmutableArray<Invitation>> GetInvitationsAsync()
    {
        var (invitations, _) = await GetInvitationsAndMappingsAsync();
        return invitations;
    }

    private static async Task<(ImmutableArray<Invitation>, ImmutableDictionary<int, string>)> GetInvitationsAndMappingsAsync()
    {
        var client = await GetClientAsync();
        var res = await client.GetStringAsync(Config.Instance.SpreadsheetUrl + "/values/Invitades!A:M");
        var data = JSON.Deserialize<SpreadsheetData>(res, Options.CamelCase);

        var builder = ImmutableArray.CreateBuilder<Invitation>();
        var mappings = ImmutableDictionary<int, string>.Empty;
        for (var i = 0; i < data.Values.Length; i++)
        {
            var row = data.Values[i];
            if (row.Length == 0)
            {
                continue;
            }

            if (mappings.IsEmpty)
            {
                var dictBuilder = ImmutableDictionary.CreateBuilder<int, string>();
                for (var j = 0; j < row.Length; j++)
                {
                    dictBuilder[j] = row[j];
                }

                mappings = dictBuilder.ToImmutable();
            }
            else
            {
                var invitation = new Invitation { Row = i + 1 };
                for (var j = 0; j < row.Length; j++)
                {
                    SetValue(invitation, mappings[j], row[j]);
                }

                builder.Add(invitation);
            }
        }

        return (builder.ToImmutable(), mappings);
    }

    public static async Task SaveInvitationResponse(Invitation invitation)
    {
        var (invitations, mappings) = await GetInvitationsAndMappingsAsync();
        var currentInvitation = invitations.FirstOrDefault(i => i.Code == invitation.Code);

        var reverseMapping = mappings.ToImmutableDictionary(e => e.Value, e => e.Key);
        var request = new BatchUpdateRequest();
        foreach (var fieldName in _fieldsToUpdate)
        {
            var column = (char)(reverseMapping[fieldName] + 'A');
            var range = $"Invitades!{column}{currentInvitation.Row}";
            request.UpdateCell(range, _accessor[invitation, fieldName]?.ToString());
        }

        var requestBody = JSON.SerializeDynamic(request, Options.IncludeInheritedCamelCase);
        var content = new StringContent(requestBody);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var client = await GetClientAsync();
        var response = await client.PostAsync(Config.Instance.SpreadsheetUrl + "/values:batchUpdate", content);
        var responseStr = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
    }

    private static ImmutableArray<string> _ignoredFields = ImmutableArray.Create("Notes");
    private static void SetValue(Invitation invitation, string fieldName, string value)
    {
        if (_ignoredFields.Contains(fieldName))
        {
            return;
        }
        
        if (_parserByField == null)
        {
            _parserByField = new Dictionary<string, Func<string, object>>();
            foreach (var property in typeof(Invitation).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    _parserByField[property.Name] = v => v;
                }
                else if (property.PropertyType == typeof(int))
                {
                    _parserByField[property.Name] = v => v.HasValue() ? int.Parse(v) : 0;
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    _parserByField[property.Name] = v => v.HasValue() ? DateTime.Parse(v) : null;
                }
                else if (property.PropertyType == typeof(bool?))
                {
                    _parserByField[property.Name] = v => v.HasValue() ? bool.Parse(v) : null;
                }
                else if (property.PropertyType == typeof(int?))
                {
                    _parserByField[property.Name] = v => v.HasValue() ? int.Parse(v) : null;
                }
            }
        }

        _accessor[invitation, fieldName] = _parserByField[fieldName](value);
    }

    private class SpreadsheetData
    {
        public string[][] Values { get; private set; }
    }

    private class BatchUpdateRequest
    {
        public BatchUpdateRequest()
        {
            Data = new List<BatchUpdateDataElement>();
        }

        public string ValueInputOption => "USER_ENTERED";
        public bool IncludeValuesInResponse => false;
        public List<BatchUpdateDataElement> Data { get; }

        public BatchUpdateRequest UpdateCell(string cell, string value)
        {
            if (value.HasValue())
            {
                Data.Add(new BatchUpdateDataElement { Range = cell, Values = new[] { new[] { value } } });
            }
            else
            {
                Data.Add(new BatchUpdateDataElement { Range = cell, Values = new[] { new[] { "" } } });
            }

            return this;
        }

        public class BatchUpdateDataElement
        {
            public string Range { get; set; }
            public string[][] Values { get; set; }
        }
    }
}
