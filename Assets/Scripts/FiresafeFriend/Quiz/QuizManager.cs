using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] options;
    public int correctAnswerIndex;
}
[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}
public class QuizManager : MonoBehaviour
{
    public string sheetURL = "https://script.google.com/macros/s/AKfycbwIkFFWK7Y5yg5JcYjyqOVRdR4Nkslo6VUO6JE1oqjAbe30xcGHK5_fFmPvTnpOk3Y8/exec";
    public List<Question> questions;

    private string jsonFilePath;
    // Start is called before the first frame update
    void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "QuizQuestions.json");
        if (File.Exists(jsonFilePath))
        {
            LoadQuestionsFromFile();
            return;
        }
        StartCoroutine(LoadQuestionsFromWeb());
    }

    private void LoadQuestionsFromFile()
    {
        string json = File.ReadAllText(jsonFilePath);
        questions = JsonUtility.FromJson<QuestionList>(WrapJsonArray(json)).questions;
        Debug.Log($"Loaded {questions.Count} questions from local cache.");
    }

    IEnumerator LoadQuestionsFromWeb()
    {
        UnityWebRequest request = UnityWebRequest.Get(sheetURL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // Deserialize the JSON data
            questions = JsonUtility.FromJson<QuestionList>(WrapJsonArray(json)).questions;

            Debug.Log($"Loaded {questions.Count} questions.");
        }
        else
        {
            Debug.LogError($"Failed to load questions: {request.error}");
        }
        
    }

    private string WrapJsonArray(string json)
    {
        return $"{{\"questions\": {json}}}";
    }

    public Question ReturnRandomQuestion()
    {
        return questions.OrderBy(x => Guid.NewGuid()).First();
    }
}
