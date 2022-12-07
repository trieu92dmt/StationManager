using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ISD.API.EntityModels.Models
{
    [MetadataTypeAttribute(typeof(RefreshToken.MetaData))]
    public partial class RefreshToken
    {
        internal sealed class MetaData
        {
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
}
