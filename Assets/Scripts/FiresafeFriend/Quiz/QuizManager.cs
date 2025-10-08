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
    //public bool isAnswered;
}
[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}
public class QuizManager : MonoBehaviour
{
    //public string sheetURL = "https://script.google.com/macros/s/AKfycbwIkFFWK7Y5yg5JcYjyqOVRdR4Nkslo6VUO6JE1oqjAbe30xcGHK5_fFmPvTnpOk3Y8/exec";
    public List<Question> questions;

    private string jsonFilePath;
    private List<int> unusedQuestionIndices;

    // Start is called before the first frame update
    void Start()
    {
        LoadQuestionsFromFile();
        InitializeUnusedIndices();

        //Original condition where if JSON file not found, get it from the Google sheet
        /*jsonFilePath = Path.Combine(Application.persistentDataPath, "QuizQuestions.json");
        if (File.Exists(jsonFilePath))
        {
            LoadQuestionsFromFile();
            return;
        }
        StartCoroutine(LoadQuestionsFromWeb());*/
    }

    private void LoadQuestionsFromFile()
    {
       jsonFilePath = Path.Combine(Application.dataPath, "Question Repo/QuizQuestions.json");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("There's no resource file");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        questions = JsonUtility.FromJson<QuestionList>(json).questions;
        Debug.Log($"Loaded {questions.Count} questions from local JSON file.");

        /*string json = File.ReadAllText(jsonFilePath);
        questions = JsonUtility.FromJson<QuestionList>(WrapJsonArray(json)).questions;
        Debug.Log($"Loaded {questions.Count} questions from local cache.");*/
    }

    //Google Sheet into Unity method
    /*IEnumerator LoadQuestionsFromWeb()
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
        InitializeUnusedIndices();
    }*/

    private void InitializeUnusedIndices()
    {
        unusedQuestionIndices = new List<int>(questions.Count);
        for (int i = 0; i < questions.Count; i++) { 
            unusedQuestionIndices.Add(i);
        }
    }
    
    private string WrapJsonArray(string json)
    {
        return $"{{\"questions\": {json}}}";
    }

    public Question ReturnRandomQuestion()
    {
        if (unusedQuestionIndices.Count == 0)
        {
            Debug.Log("All questions have been used. Resetting unused questions.");
            InitializeUnusedIndices();
        }
        int randomIndex = UnityEngine.Random.Range(0, unusedQuestionIndices.Count);
        int questionIndex = unusedQuestionIndices[randomIndex];

        unusedQuestionIndices.RemoveAt(randomIndex);

        return questions[questionIndex];
    }
}
