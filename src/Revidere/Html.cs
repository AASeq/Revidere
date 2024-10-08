namespace Revidere;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Serilog;

internal static class Html {

    private static readonly Encoding Utf8 = new UTF8Encoding(false);

    public static void FillResponse(HttpListenerResponse response, IEnumerable<CheckState> checkStates, string webTitle, int refreshInterval, bool showHistory) {
        var sw = Stopwatch.StartNew();
        try {
            Log.Verbose("Starting HTML response");

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Revidere._Resources.Overview.html");
            using var reader = new StreamReader(stream!);
            var content = new StringBuilder(reader.ReadToEnd());

            content.Replace("<!--{{TITLE}}-->", webTitle);
            content.Replace("<!--{{REFRESH}}-->", refreshInterval.ToString("0"));

            var sb = new StringBuilder();
            sb.AppendLine("    " + """<div class="container">""");
            foreach (var checkState in checkStates) {
                var check = checkState.Check;
                if (check.Properties.IsVisible == false) { continue; }

                var sinceText = GetSinceText(checkState);

                sb.AppendLine(checkState.IsHealthy switch {
                    true => $"""        <div class="ok" title="{sinceText}">""",
                    false => $"""        <div class="nok" title="{sinceText}">""",
                    _ => $"""        <div class="pending" title="{sinceText}">""",
                });
                sb.AppendLine($"""            <div class="title">{checkState.Check.Properties.Title}</div>""");
                sb.AppendLine("""            <div class="semaphore"></div>""");

                if (showHistory && (check is not CompositeCheck)) {
                    sb.AppendLine("""            <div class="history">""");
                    var i = 0;
                    foreach (var state in checkState.HealthHistory) {
                        sb.AppendLine("                " + (state ? """<div class="historicalOk"></div>""" : """<div class="historicalNok"></div>"""));
                        i++;
                        if (i >= 10) { break; }
                    }
                    sb.AppendLine("""            </div>""");
                } else {
                    sb.AppendLine("""            <div class="nohistory">""");
                    sb.AppendLine("""            </div>""");
                }

                sb.AppendLine("""        </div>""");

                if (check.Properties.IsBreak) {
                    sb.AppendLine("""    </div>""");
                    sb.AppendLine("    " + """<div class="container">""");
                }
            }
            sb.AppendLine("""    </div>""");

            content.Replace("<!--{{CONTENT}}-->", sb.ToString());

            response.StatusCode = (int)HttpStatusCode.OK;

            response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");

            response.ContentType = "text/html";
            var buffer = Utf8.GetBytes(content.ToString());
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer);
        } finally {
            Log.Verbose("Completed HTML response in {Interval} ms", sw.ElapsedMilliseconds);
        }
    }


    private static string GetSinceText(CheckState checkState) {
        var isHealthy = checkState.IsHealthy;
        var lastChange = checkState.LastChanged;

        if (isHealthy == null) {
            return "Pending";
        } else {
            var healthText = (isHealthy.Value ? "Healthy" : "Unhealthy");
            if (lastChange != null) {
                var duration = DateTime.UtcNow - lastChange.Value;

                var sbDuration = new StringBuilder();
                if (duration.Days > 0) {
                    sbDuration.Append("for ");
                    sbDuration.Append(GetPlural(duration.Days, "day", "days"));
                    sbDuration.Append(", " + GetPlural(duration.Hours, "hour", "hours"));
                    sbDuration.Append(", and " + GetPlural(duration.Minutes, "minute", "minutes"));
                } else if (duration.Hours > 0) {
                    sbDuration.Append("for ");
                    sbDuration.Append(GetPlural(duration.Hours, "hour", "hours"));
                    sbDuration.Append(", and " + GetPlural(duration.Minutes, "minute", "minutes"));
                } else if (duration.Minutes > 0) {
                    sbDuration.Append("for ");
                    sbDuration.Append(GetPlural(duration.Minutes, "minute", "minutes"));
                } else {
                    sbDuration.Append("as of recently");
                }

                return healthText + " " + sbDuration.ToString() + " (" + lastChange.Value.ToString("yyyy-MM-dd HH:mm:ss") + ")";
            } else {
                return healthText;
            }
        }
    }

    private static string GetPlural(double value, string singular, string plural) {
        var wholeNumber = (int)value;
        return wholeNumber.ToString("0") + " " + (wholeNumber == 1 ? singular : plural);
    }

}