using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class Question
{
    public string questionText_en;
    public string questionText_es;
    public string[] options_en;
    public string[] options_es;
    public int correctAnswerIndex;

    public string GetLocalizedQuestion(string lang)
    {
        return lang == "es" ? questionText_es : questionText_en;
    }

    public string[] GetLocalizedOptions(string lang)
    {
        return lang == "es" ? options_es : options_en;
    }
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
    private List<int> unusedQuestionIndices;

    // Start is called before the first frame update
    void Start()
    {
        LocalizedFileLoader.Load<QuestionList>("QuizQuestions.json", (data) =>
        {
            if (data != null)
            {
                questions = data.questions;
                InitializeUnusedIndices(); 
                Debug.Log($"Loaded {questions.Count} questions using LocalizedFileLoader.");
            }
            else
            {
                Debug.LogError("Failed to load quiz questions.");
            }
        });

        //Original condition where if JSON file not found, get it from the Google sheet
        /*jsonFilePath = Path.Combine(Application.persistentDataPath, "QuizQuestions.json");
        if (File.Exists(jsonFilePath))
        {
            LoadQuestionsFromFile();
            return;
        }
        StartCoroutine(LoadQuestionsFromWeb());*/
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
        for (int i = 0; i < questions.Count; i++)
            unusedQuestionIndices.Add(i);
    }

    public Question ReturnRandomQuestion()
    {
        if (unusedQuestionIndices.Count == 0)
        {
            InitializeUnusedIndices();
        }

        int randomIndex = UnityEngine.Random.Range(0, unusedQuestionIndices.Count);
        int questionIndex = unusedQuestionIndices[randomIndex];
        unusedQuestionIndices.RemoveAt(randomIndex);

        return questions[questionIndex];
    }
}
