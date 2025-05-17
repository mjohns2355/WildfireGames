using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;


public class NewsFetcher : MonoBehaviour
{
    public InputField zipCodeInput;
    public Button fetchButton;
    public TMP_Text newsText;


    private const string zipApi = "https://api.zippopotam.us/us/";
    private const string newsApiKey = "931206936dfa40109f16f10b1813c803"; // Our current NewsAPI key
    private const string newsApiBase = "https://newsapi.org/v2/everything";

    void Start()
    {
        fetchButton.onClick.AddListener(() => StartCoroutine(FetchNews(zipCodeInput.text)));
    }

   IEnumerator FetchNews(string zip)
{
    newsText.text = "Fetching location info...";

    UnityWebRequest locationRequest = UnityWebRequest.Get(zipApi + zip);
    yield return locationRequest.SendWebRequest();

    if (locationRequest.result != UnityWebRequest.Result.Success)
    {
        newsText.text = "Failed to get location info.";
        yield break;
    }

    var locationJson = JSON.Parse(locationRequest.downloadHandler.text);
    string city = locationJson["places"][0]["place name"];
    string state = locationJson["places"][0]["state abbreviation"];
    string county = locationJson["places"][0]["county"] ?? ""; // Note: not all ZIP APIs include county

    newsText.text = "Fetching news...";

    string[] searchScopes = new string[]
    {
        $"(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND \"{city}\" AND \"{state}\"",
        county != "" ? $"(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND \"{county}\" AND \"{state}\"" : null,
        $"(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND \"{state}\"",
        "(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND United States"
    };

    bool foundNews = false;

    foreach (var query in searchScopes)
    {
        if (query == null) continue;

        string newsUrl = $"{newsApiBase}?q={UnityWebRequest.EscapeURL(query)}&apiKey={newsApiKey}&pageSize=5&sortBy=publishedAt";
        UnityWebRequest newsRequest = UnityWebRequest.Get(newsUrl);
        yield return newsRequest.SendWebRequest();

        if (newsRequest.result != UnityWebRequest.Result.Success) continue;

        var newsJson = JSON.Parse(newsRequest.downloadHandler.text);
        var articles = newsJson["articles"];

        if (articles.Count > 0)
        {
            newsText.text = "";
            foreach (var article in articles.Children)
            {
                string title = article["title"];
                string url = article["url"];
                string rawDate = article["publishedAt"];
                string formattedDate = "";

                if (System.DateTime.TryParse(rawDate, out var parsedDate))
                {
                    formattedDate = parsedDate.ToString("MMMM yyyy");
                }

                newsText.text += $"<link={url}><color=#0000EE><u>{title}</u></color></link>\n{formattedDate}\n\n";
            }

            foundNews = true;
            break;
        }
    }

    // Fallback: If no wildfire news for zip-specific queries, get wildfire news for California state
    if (!foundNews)
    {
        string fallbackQuery = "(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND California";

        string fallbackUrl = $"{newsApiBase}?q={UnityWebRequest.EscapeURL(fallbackQuery)}&apiKey={newsApiKey}&pageSize=5&sortBy=publishedAt";
        UnityWebRequest fallbackRequest = UnityWebRequest.Get(fallbackUrl);
        yield return fallbackRequest.SendWebRequest();

        if (fallbackRequest.result == UnityWebRequest.Result.Success)
        {
            var fallbackJson = JSON.Parse(fallbackRequest.downloadHandler.text);
            var fallbackArticles = fallbackJson["articles"];

            if (fallbackArticles.Count > 0)
            {
                newsText.text = "";
                foreach (var article in fallbackArticles.Children)
                {
                    string title = article["title"];
                    string url = article["url"];
                    string rawDate = article["publishedAt"];
                    string formattedDate = "";

                    if (System.DateTime.TryParse(rawDate, out var parsedDate))
                    {
                        formattedDate = parsedDate.ToString("MMMM yyyy");
                    }

                    newsText.text += $"<link={url}><color=#0000EE><u>{title}</u></color></link>\n{formattedDate}\n\n";
                }
                foundNews = true;
            }
        }
    }

    if (!foundNews)
    {
        newsText.text = "Unable to fetch news at this time.";
    }
}


}
