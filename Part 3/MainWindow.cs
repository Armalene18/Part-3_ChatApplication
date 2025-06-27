using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private readonly TaskManager taskManager = new();
        private readonly QuizManager quizManager = new();
        private readonly ActivityLog activityLog = new();
        private DispatcherTimer reminderTimer;
        private bool awaitingReminderConfirmation = false;
        private TaskItem lastAddedTask;

        public MainWindow()
        {
            InitializeComponent();
            reminderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserMessage(input);
            UserInput.Clear();

            string loweredInput = input.ToLower();

            if (awaitingReminderConfirmation)
            {
                HandleReminderConfirmation(loweredInput);
                return;
            }

            if (loweredInput.Contains("add a task") || loweredInput.StartsWith("remind me to"))
            {
                string title = loweredInput.Contains("add a task") ?
                    input.Substring(loweredInput.IndexOf("add a task to") + 14) :
                    input.Substring(loweredInput.IndexOf("remind me to") + 13);

                lastAddedTask = new TaskItem { Title = title, Description = title };
                taskManager.AddTask(lastAddedTask.Title, lastAddedTask.Description);
                UpdateTaskList();
                AddBotMessage($"Task added: '{title}'. Would you like to set a reminder?");
                awaitingReminderConfirmation = true;
                return;
            }

            if (loweredInput.Contains("start quiz"))
            {
                quizManager.StartQuiz();
                ShowNextQuestion();
                activityLog.Add("Quiz started");
                return;
            }

            if (quizManager.IsInQuiz)
            {
                HandleQuizAnswer(loweredInput);
                return;
            }

            if (loweredInput.Contains("activity log"))
            {
                AddBotMessage("📑 Here's your activity log:");
                foreach (var log in activityLog.GetLog())
                    ChatLog.Items.Add($"🕓 {log}");
                return;
            }

            AddBotMessage("I'm not sure what you meant. Try adding a task, starting a quiz, or checking the log.");
        }

        private void HandleReminderConfirmation(string input)
        {
            if (input.Contains("yes") || input.Contains("remind"))
            {
                int minutes = 1;
                if (input.Contains("in "))
                {
                    string[] parts = input.Split(' ');
                    if (int.TryParse(parts.FirstOrDefault(p => int.TryParse(p, out _)), out int parsedMinutes))
                        minutes = parsedMinutes;
                }

                lastAddedTask.ReminderDate = DateTime.Now.AddMinutes(minutes);
                UpdateTaskList();
                AddBotMessage($"Reminder set for '{lastAddedTask.Title}' in {minutes} minute(s).");
                activityLog.Add($"Reminder set: {lastAddedTask.Title} in {minutes} minutes");
            }
            else
            {
                AddBotMessage("Okay, no reminder set.");
            }

            awaitingReminderConfirmation = false;
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var dueTasks = taskManager.GetPendingTasks()
                .Where(t => t.ReminderDate != null && t.ReminderDate <= now && !t.IsCompleted).ToList();

            foreach (var task in dueTasks)
            {
                MessageBox.Show($"⏰ Reminder: '{task.Title}' is due now!", "Reminder Alert");
                AddBotMessage($"🔔 Reminder shown for: '{task.Title}'");
                activityLog.Add($"Reminder triggered: {task.Title}");
                task.IsCompleted = true;
                UpdateTaskList();
            }
        }

        private void ShowNextQuestion()
        {
            var question = quizManager.GetNextQuestion();
            if (question != null)
            {
                var q = question.Value;
                AddBotMessage($"❓ {q.Question}");
                for (int i = 0; i < q.Choices.Count; i++)
                    AddBotMessage($"{(char)('A' + i)}. {q.Choices[i]}");
            }
            else
            {
                AddBotMessage($"🎉 Quiz complete! You scored {quizManager.GetScore()} out of 10.");
                activityLog.Add("Quiz completed");
            }
        }

        private void HandleQuizAnswer(string input)
        {
            bool correct = quizManager.AnswerQuestion(input);
            AddBotMessage(correct ? "✅ Correct!" : "❌ Incorrect.");
            AddBotMessage($"📘 {quizManager.GetExplanation()}");
            ShowNextQuestion();
        }

        private void RefreshQuizHistory_Click(object sender, RoutedEventArgs e)
        {
            QuizHistoryListBox.ItemsSource = null;

            var history = quizManager.GetQuizHistory();

            if (history != null && history.Any())
            {
                QuizHistoryListBox.ItemsSource = history;
                AddBotMessage("🔄 Quiz history refreshed.");
            }
            else
            {
                AddBotMessage("No quiz history available.");
            }
        }

        private void AddBotMessage(string message)
        {
            ChatLog.Items.Add($"🤖 Bot: {message}");
        }

        private void AddUserMessage(string message)
        {
            ChatLog.Items.Add($"🧑 You: {message}");
        }

        private void UpdateTaskList()
        {
            TaskListView.ItemsSource = null;
            TaskListView.ItemsSource = taskManager.GetAllTasks();
        }

        private void TaskCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskItem task)
            {
                string status = task.IsCompleted ? "marked as completed" : "marked as not completed";
                AddBotMessage($"📌 Task '{task.Title}' {status}.");
                activityLog.Add($"Task '{task.Title}' {status}");
                UpdateTaskList();
            }
        }
    }
}
