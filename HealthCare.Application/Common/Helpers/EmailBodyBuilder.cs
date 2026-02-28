namespace HealthCare.Application.Common.Helpers;

public static class EmailBodyBuilder
{
    public static string BuildEmailBody(string templetePath, Dictionary<string, string> templeteModel)
    {
        var streamReader = new StreamReader(templetePath);
        var body = streamReader.ReadToEnd();
        foreach (var item in templeteModel)
        {
            body = body.Replace($"{item.Key}", item.Value);
        }

        return body;
    }
}
