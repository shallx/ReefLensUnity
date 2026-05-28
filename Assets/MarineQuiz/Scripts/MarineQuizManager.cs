using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarineQuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
    }

    public QuizQuestion[] questions;

    public TMP_Text questionText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    public Button[] answerButtons;
    public TMP_Text[] answerTexts;

    public GameObject feedbackPanel;
    public TMP_Text feedbackText;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private int highScore = 0;
    private bool canAnswer = true;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("MarineQuizHighScore", 0);

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }

        ShowQuestion();
    }

    void ShowQuestion()
    {
        canAnswer = true;

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }

        if (currentQuestionIndex >= questions.Length)
        {
            EndQuiz();
            return;
        }

        QuizQuestion q = questions[currentQuestionIndex];

        questionText.text = q.question;
        scoreText.text = "Score: " + score + " / " + questions.Length;
        highScoreText.text = "High Score: " + highScore + " / " + questions.Length;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;

            answerTexts[i].text = q.answers[i];
            answerButtons[i].gameObject.SetActive(true);

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        if (!canAnswer)
        {
            return;
        }

        canAnswer = false;

        bool isCorrect = selectedIndex == questions[currentQuestionIndex].correctAnswerIndex;

        if (isCorrect)
        {
            score++;
        }

        StartCoroutine(ShowFeedbackThenNext(isCorrect));
    }

    IEnumerator ShowFeedbackThenNext(bool isCorrect)
    {
        QuizQuestion q = questions[currentQuestionIndex];
        string correctAnswer = q.answers[q.correctAnswerIndex];

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        questionText.text = "";
        scoreText.text = "";
        highScoreText.text = "";

        feedbackPanel.SetActive(true);

        if (isCorrect)
        {
            feedbackText.color = Color.green;
            feedbackText.text = " Correct!";
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text = " Wrong!\nCorrect answer: " + correctAnswer;
        }

        yield return new WaitForSeconds(1.2f);

        currentQuestionIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("MarineQuizHighScore", highScore);
            PlayerPrefs.Save();
        }

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }

        questionText.text = "Quiz Complete!";

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        scoreText.text = "Final Score: " + score + " / " + questions.Length;
        highScoreText.text = "High Score: " + highScore + " / " + questions.Length;
    }
}