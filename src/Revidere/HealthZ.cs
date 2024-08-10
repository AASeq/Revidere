namespace Revidere;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

internal static class HealthZ {

    public static void FillRootResponse(HttpListenerResponse response, IEnumerable<CheckState>? checkStates) {
        response.StatusCode = (int)HttpStatusCode.OK;  // always ok

        response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        response.Headers.Add("Pragma", "no-cache");
        response.Headers.Add("Expires", "0");

        response.ContentType = "application/json";
        var buffer = HealthZ.GetRootJsonBuffer(checkStates);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);

    }

    public static void FillCheckResponse(HttpListenerResponse response, CheckState checkState) {
        response.StatusCode = checkState.IsHealthy == true ? (int)HttpStatusCode.OK : (int)HttpStatusCode.ServiceUnavailable;

        response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        response.Headers.Add("Pragma", "no-cache");
        response.Headers.Add("Expires", "0");

        response.ContentType = "application/json";
        var buffer = GetCheckJsonBuffer(checkState);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);
    }

    private static byte[] GetRootJsonBuffer(IEnumerable<CheckState>? checkStates) {
        using var bufferStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(bufferStream)) {
            jsonWriter.WriteStartObject();
            HealthZ.WriteStatusToJsonWriter(jsonWriter, null, includeTimestamp: true);
            jsonWriter.WriteString("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (checkStates != null) {
                jsonWriter.WriteStartArray("checks");

                foreach (var checkState in checkStates) {
                    jsonWriter.WriteStartObject();
                    HealthZ.WriteStatusToJsonWriter(jsonWriter, checkState);
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }
        return bufferStream.ToArray();
    }

    private static byte[] GetCheckJsonBuffer(CheckState checkState) {
        using var bufferStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(bufferStream)) {
            jsonWriter.WriteStartObject();
            WriteStatusToJsonWriter(jsonWriter, checkState, includeTimestamp: true);

            jsonWriter.WriteEndObject();
        }
        return bufferStream.ToArray();
    }


    private static void WriteStatusToJsonWriter(Utf8JsonWriter jsonWriter, CheckState? checkState, bool includeTimestamp = false) {
        jsonWriter.WriteString("status", (checkState == null) ? "healthy" : checkState.IsHealthy switch {
            true => "healthy",
            false => "unhealthy",
            _ => "unknown"
        });
        if (includeTimestamp) { jsonWriter.WriteString("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")); }

        if (checkState != null) {
            if (checkState.Check.Name != null) { jsonWriter.WriteString("name", checkState.Check.Name); }
            if (!string.IsNullOrEmpty(checkState.Check.Title)) { jsonWriter.WriteString("title", checkState.Check.Title); }
            if (checkState.LastChanged != null) { jsonWriter.WriteString("since", checkState.LastChanged.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if ((checkState.LastChanged != null) && (checkState.IsHealthy == true)) { jsonWriter.WriteString("uptime", GetUpTimeText(DateTimeOffset.Now - checkState.LastChanged.Value)); }
            if ((checkState.LastChanged != null) && (checkState.IsHealthy == false)) { jsonWriter.WriteString("downtime", GetUpTimeText(DateTimeOffset.Now - checkState.LastChanged.Value)); }
        }
    }

    private static string GetUpTimeText(TimeSpan uptime) {
        var sb = new StringBuilder();
        if (uptime.Days > 0) { sb.Append($"{uptime.Days}d "); }
        if (uptime.Hours > 0) { sb.Append($"{uptime.Hours}h "); }
        if (uptime.Minutes > 0) { sb.Append($"{uptime.Minutes}m "); }
        if (uptime.Seconds > 0) { sb.Append($"{uptime.Seconds}s "); }
        return sb.ToString().TrimEnd();
    }
}
