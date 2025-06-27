using System;
using System.Collections.Generic;

public class QuizManager
{
    private int current = 0;
    private int score = 0;
    private List<(string Question, List<string> Choices, char Answer, string Explanation)> questions;
    public bool IsInQuiz { get; private set; } = false;
    private readonly List<string> quizHistory = new();

    public void StartQuiz()
    {
        IsInQuiz = true;
        current = 0;
        score = 0;
        questions = new List<(string, List<string>, char, string)>
        {
            ("What should you do if you receive an email asking for your password?",
             new() {"Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it"},
             'C',
             "Reporting phishing emails helps prevent scams."),

            ("Which of these is the strongest password?",
             new() {"123456", "Password1", "MyDog123", "G@2x!9Lm#1"},
             'D',
             "Strong passwords use symbols, numbers, and mixed case."),

            ("What is two-factor authentication (2FA)?",
             new() {"A type of firewall", "An antivirus software", "An extra layer of login security", "A network protocol"},
             'C',
             "2FA adds a second verification step for better security."),

            ("True or False: You should reuse passwords on multiple sites.",
             new() {"True", "False"},
             'B',
             "Passwords should be unique for each site."),

            ("Which is an example of social engineering?",
             new() {"Hacking Wi-Fi", "Tricking someone into giving a password", "Installing malware", "Encrypting files"},
             'B',
             "Social engineering manipulates people, not systems."),

            ("What does a padlock icon in the browser URL bar mean?",
             new() {"Website is safe", "Connection is encrypted", "The site is verified", "All of the above"},
             'D',
             "A padlock means a secure and encrypted connection."),

            ("Which of these should you avoid on public Wi-Fi?",
             new() {"Watching videos", "Checking weather", "Logging into banking", "Browsing news"},
             'C',
             "Avoid sensitive activities on public networks."),

            ("What is a common sign of phishing?",
             new() {"Poor spelling", "Strange links", "Urgent tone", "All of the above"},
             'D',
             "Phishing emails often include multiple red flags."),

            ("What should you do before clicking a link in an email?",
             new() {"Click it quickly", "Forward it", "Check the URL", "Ignore it"},
             'C',
             "Always hover to preview and verify the link."),

            ("Why should you keep your software updated?",
             new() {"To make it faster", "To improve appearance", "To fix bugs and security flaws", "To get new colors"},
             'C',
             "Updates patch vulnerabilities that hackers exploit.")
        };
    }

    public (string Question, List<string> Choices, char Answer, string Explanation)? GetNextQuestion()
    {
        if (current < questions.Count)
            return questions[current];

        IsInQuiz = false;
        string feedback = GetFeedback();
        quizHistory.Add($"{DateTime.Now}: Scored {score}/10 - {feedback}");
        return null;
    }

    public bool AnswerQuestion(string input)
    {
        char answer = char.ToUpper(input[0]);
        var correct = questions[current].Answer == answer;
        if (correct) score++;
        current++;
        return correct;
    }

    public string GetExplanation() => questions[Math.Max(0, current - 1)].Explanation;

    public int GetScore() => score;

    public string GetFeedback()
    {
        return score switch
        {
            >= 9 => "🌟 Cybersecurity Pro!",
            >= 7 => "👍 Great job, you're almost an expert!",
            >= 5 => "🛡️ Good effort, but keep learning!",
            _ => "🔐 Keep studying to stay safe online."
        };
    }

    public List<string> GetQuizHistory() => quizHistory;
}
