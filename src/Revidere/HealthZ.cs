namespace Revidere;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

internal static class HealthZ {

    public static void FillRootResponse(HttpListenerResponse response, IEnumerable<TargetState>? targetStates) {
        response.StatusCode = (int)HttpStatusCode.OK;  // always ok

        response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        response.Headers.Add("Pragma", "no-cache");
        response.Headers.Add("Expires", "0");

        response.ContentType = "application/json";
        var buffer = HealthZ.GetRootJsonBuffer(targetStates);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);

    }

    public static void FillTargetResponse(HttpListenerResponse response, TargetState targetState) {
        response.StatusCode = targetState.IsHealthy == true ? (int)HttpStatusCode.OK : (int)HttpStatusCode.ServiceUnavailable;

        response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        response.Headers.Add("Pragma", "no-cache");
        response.Headers.Add("Expires", "0");

        response.ContentType = "application/json";
        var buffer = GetTargetJsonBuffer(targetState);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer);
    }

    private static byte[] GetRootJsonBuffer(IEnumerable<TargetState>? targetStates) {
        using var bufferStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(bufferStream)) {
            jsonWriter.WriteStartObject();
            HealthZ.WriteStatusToJsonWriter(jsonWriter, null, includeTimestamp: true);
            jsonWriter.WriteString("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (targetStates != null) {
                jsonWriter.WriteStartArray("targets");

                foreach (var targetState in targetStates) {
                    jsonWriter.WriteStartObject();
                    HealthZ.WriteStatusToJsonWriter(jsonWriter, targetState);
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }
        return bufferStream.ToArray();
    }

    private static byte[] GetTargetJsonBuffer(TargetState targetState) {
        using var bufferStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(bufferStream)) {
            jsonWriter.WriteStartObject();
            WriteStatusToJsonWriter(jsonWriter, targetState, includeTimestamp: true);

            jsonWriter.WriteEndObject();
        }
        return bufferStream.ToArray();
    }


    private static void WriteStatusToJsonWriter(Utf8JsonWriter jsonWriter, TargetState? targetState, bool includeTimestamp = false) {
        jsonWriter.WriteString("status", (targetState == null) ? "healthy" : targetState.IsHealthy switch {
            true => "healthy",
            false => "unhealthy",
            _ => "unknown"
        });
        if (includeTimestamp) { jsonWriter.WriteString("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")); }

        if (targetState != null) {
            if (targetState.Target.Name != null) { jsonWriter.WriteString("name", targetState.Target.Name); }
            if (!string.IsNullOrEmpty(targetState.Target.Title)) { jsonWriter.WriteString("title", targetState.Target.Title); }
            if (targetState.LastChanged != null) { jsonWriter.WriteString("since", targetState.LastChanged.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); }
            if ((targetState.LastChanged != null) && (targetState.IsHealthy == true)) { jsonWriter.WriteString("uptime", GetUpTimeText(DateTimeOffset.Now - targetState.LastChanged.Value)); }
            if ((targetState.LastChanged != null) && (targetState.IsHealthy == false)) { jsonWriter.WriteString("downtime", GetUpTimeText(DateTimeOffset.Now - targetState.LastChanged.Value)); }
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
