using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ISD.API.ViewModels
{
    public class RefreshTokenViewModel
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByUserName { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
