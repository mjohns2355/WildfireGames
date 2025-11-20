using System;
using System.Collections.Generic;
using UnityEngine;

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
    public List<Question> questions;
    private List<int> unusedQuestionIndices;

    void Awake()
    {
        LoadQuestionsFromResources();
        InitializeUnusedIndices();
    }

    private void LoadQuestionsFromResources()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("QuizQuestions");

        if (jsonAsset == null)
        {
            Debug.LogError("QUIZ ERROR: QuizQuestions.json NOT FOUND in Resources!");
            questions = new List<Question>();
            return;
        }

        try
        {
            QuestionList wrapper = JsonUtility.FromJson<QuestionList>(jsonAsset.text);
            questions = wrapper.questions;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed parsing quiz JSON: " + e.Message);
            questions = new List<Question>();
        }

        Debug.Log("Quiz loaded: " + questions.Count + " questions.");
    }

    private void InitializeUnusedIndices()
    {
        unusedQuestionIndices = new List<int>(questions.Count);
        for (int i = 0; i < questions.Count; i++)
        {
            unusedQuestionIndices.Add(i);
        }
    }

    public Question GetRandomQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions loaded!");
            return null;
        }

        if (unusedQuestionIndices.Count == 0)
        {
            InitializeUnusedIndices();
        }

        int randomListIndex = UnityEngine.Random.Range(0, unusedQuestionIndices.Count);
        int questionIndex = unusedQuestionIndices[randomListIndex];

        unusedQuestionIndices.RemoveAt(randomListIndex);

        return questions[questionIndex];
    }
}
