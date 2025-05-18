using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using System;
using System.Web;



public class NewsFetcher : MonoBehaviour
{
    public InputField zipCodeInput;
    public Button fetchButton;
    public TMP_Text newsText;


    private const string zipApi = "https://api.zippopotam.us/us/";
    private const string newsApiKey = "931206936dfa40109f16f10b1813c803"; // NewsAPI key
    private const string newsApiBase = "https://newsapi.org/v2/everything";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        fetchButton.onClick.AddListener(() => StartCoroutine(FetchNews(zipCodeInput.text)));
    }

    IEnumerator FetchNews(string zip)
    {
        newsText.text = "Fetching location info...";

        // Get state from ZIP
        UnityWebRequest locationRequest = UnityWebRequest.Get(zipApi + zip);
        yield return locationRequest.SendWebRequest();

        if (locationRequest.result != UnityWebRequest.Result.Success)
        {
            newsText.text = "Failed to get location info.";
            yield break;
        }

        var locationJson = JSON.Parse(locationRequest.downloadHandler.text);
        string state = locationJson["places"][0]["state abbreviation"];

        // Only do state-level wildfire news
        yield return StartCoroutine(FetchWildfireNews(state));
    }

    IEnumerator FetchWildfireNews(string state)
    {
        string query = $"(wildfire OR \"wild fire\" OR \"forest fire\" OR evacuation) AND {state}";
        int randomPage = UnityEngine.Random.Range(1, 6); // Page 1 to 5
        string newsUrl = $"{newsApiBase}?q={UnityWebRequest.EscapeURL(query)}&apiKey={newsApiKey}&pageSize=10&sortBy=publishedAt&page={randomPage}";

        newsText.text = "Fetching wildfire news...";

        UnityWebRequest newsRequest = UnityWebRequest.Get(newsUrl);
        yield return newsRequest.SendWebRequest();

        if (newsRequest.result != UnityWebRequest.Result.Success)
        {
            newsText.text = "Failed to fetch news.";
            yield break;
        }

        var newsJson = JSON.Parse(newsRequest.downloadHandler.text);
        var articles = newsJson["articles"];

        // Filter wildfire-related articles
        string[] keywords = { "wildfire", "wild fire", "forest fire", "evacuation", "fire" };
        newsText.text = "";
        int addedCount = 0;
        string[] blockedDomains = { "breitbart.com", "forbes.com", "nbcsports.com" };

        foreach (var article in articles.Children)
        {
            string titleRaw = article["title"];
            string url = article["url"];
            string dateRaw = article["publishedAt"];

            if (string.IsNullOrEmpty(url) || !url.StartsWith("http") || string.IsNullOrEmpty(titleRaw))
                continue;

            // Skip articles from Breitbart
            bool isBlocked = false;
            foreach (var domain in blockedDomains)
            {
                if (url.Contains(domain))
                {
                    isBlocked = true;
                    break;
                }
            }
            if (isBlocked)
                continue;


            string titleLower = titleRaw.ToLower();
            bool containsKeyword = false;
            foreach (var keyword in keywords)
            {
                if (titleLower.Contains(keyword))
                {
                    containsKeyword = true;
                    break;
                }
            }
            if (!containsKeyword)
                continue;

            // Decode and clean title
            string cleanTitle = HttpUtility.HtmlDecode(titleRaw).Trim('"');

            // Skip titles with suspicious characters or formatting
            if (cleanTitle.Contains("<") || cleanTitle.Contains(">") || cleanTitle.Contains("&") || cleanTitle.Length > 300)
                continue;

            // Format date
            string formattedDate = "";
            if (DateTime.TryParse(dateRaw, out DateTime parsedDate))
            {
                formattedDate = parsedDate.ToString("MMMM yyyy");
            }

            // Add to display
            newsText.text += $"<link=\"{url}\"><color=#0000EE><u>{cleanTitle}</u></color></link>\n{formattedDate}\n\n";
            addedCount++;

            if (addedCount >= 5)
                break;
        }


    }

}
