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
        string query = $"(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND \"{city}\" AND \"{state}\"";

        string newsUrl = $"{newsApiBase}?q={UnityWebRequest.EscapeURL(query)}&apiKey={newsApiKey}&pageSize=5&sortBy=publishedAt";

        newsText.text = "Fetching news...";

        UnityWebRequest newsRequest = UnityWebRequest.Get(newsUrl);
        yield return newsRequest.SendWebRequest();

        if (newsRequest.result != UnityWebRequest.Result.Success)
        {
            newsText.text = "Failed to fetch news.";
            yield break;
        }

        var newsJson = JSON.Parse(newsRequest.downloadHandler.text);
        var articles = newsJson["articles"];

        if (articles.Count == 0)
        {
            newsText.text = "No news found related to wildfires in your area.";
            yield break;
        }

        newsText.text = "";

        foreach (var article in articles.Children)
        {
            string title = article["title"];
            string url = article["url"];
            string date = article["publishedAt"];
            newsText.text += $"<link={url}><color=#0000EE><u>{title}</u></color></link>\n{date}\n\n";

        }
    }
}
