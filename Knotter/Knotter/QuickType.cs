﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public partial class CPost
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("locked_tags")]
        public object LockedTags { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("created_at")]
        public CreatedAt CreatedAt { get; set; }

        [JsonProperty("creator_id")]
        public long CreatorId { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("change")]
        public long Change { get; set; }

        [JsonProperty("source")]
        public Uri Source { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("fav_count")]
        public long FavCount { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("file_url")]
        public Uri FileUrl { get; set; }

        [JsonProperty("file_ext")]
        public string FileExt { get; set; }

        [JsonProperty("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("preview_width")]
        public long PreviewWidth { get; set; }

        [JsonProperty("preview_height")]
        public long PreviewHeight { get; set; }

        [JsonProperty("sample_url")]
        public Uri SampleUrl { get; set; }

        [JsonProperty("sample_width")]
        public long SampleWidth { get; set; }

        [JsonProperty("sample_height")]
        public long SampleHeight { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("has_comments")]
        public bool HasComments { get; set; }

        [JsonProperty("has_notes")]
        public bool HasNotes { get; set; }

        [JsonProperty("has_children")]
        public bool HasChildren { get; set; }

        [JsonProperty("children")]
        public string Children { get; set; }

        [JsonProperty("parent_id")]
        public object ParentId { get; set; }

        [JsonProperty("artist")]
        public List<string> Artist { get; set; }

        [JsonProperty("sources")]
        public List<Uri> Sources { get; set; }
    }

    public partial class CreatedAt
    {
        [JsonProperty("json_class")]
        public string JsonClass { get; set; }

        [JsonProperty("s")]
        public long S { get; set; }

        [JsonProperty("n")]
        public long N { get; set; }
    }

    public class LoginFailure
    {
        [JsonProperty("success")]
        public string Success { get; set; }
    }
    public class Failure_type
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class LoginSuccess
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("password_hash")]
        public string PasswordHash { get; set; }
    }


    public static class Serialize
    {
        public static string ToJson(this List<dynamic> self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }
    public static class Deserialize
    {
        public static T FromJson<T>(string json) => (T) JsonConvert.DeserializeObject<T>(json, QuickType.Converter.Settings);
    }   

    internal static class Converter
    {
        public static List<string> errors = new List<string>();

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,

            MissingMemberHandling = MissingMemberHandling.Error,

            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
