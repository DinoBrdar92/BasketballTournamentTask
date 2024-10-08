﻿using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BasketballTournamentTask_cdbhnd.Database.Helpers
{
    public class JsonDateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string DATE_FORMAT = "dd/MM/yy";

        // Serializer
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }

        // Deserializer
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.ParseExact(reader.GetString()!, DATE_FORMAT);
        }

    }

}
