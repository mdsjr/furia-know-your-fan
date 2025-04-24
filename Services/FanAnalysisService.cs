// FuriaKnowYourFan/Services/FanAnalysisService.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuriaKnowYourFan.Services
{
    public class FanAnalysisService
    {
        public AnalysisResult AnalyzeTweets(List<Tweet>? tweets)
        {
            var wordCount = new Dictionary<string, int>();
            var sentimentScores = new List<int>();
            var postsByDay = new Dictionary<DateTime, int>();
            var stopWords = new List<string> { "de", "a", "o", "e", "em" };
            var positiveWords = new List<string> {
                "vitória", "venceu", "foda", "pantera", "monstro", "brabo", "cravou",
                "lendário", "mestre", "campeão", "ownou", "mitou", "arrebentou",
                "insano", "god", "jogaço", "clutch", "hype", "orgulho", "raiz"};

            var negativeWords = new List<string> {
                 "derrota", "perdeu", "ruim", "fraco", "decepção", "vexame",
                 "afundou", "tiltou", "choke", "errou", "pipoqueiro", "fracassou",
                 "vergonha", "desastre", "lento", "noob", "desastre"};

            if (tweets == null || !tweets.Any())
            {
                return new AnalysisResult
                {
                    TopWords = new Dictionary<string, int>(),
                    Sentiment = new { Positive = 0, Negative = 0, Neutral = 0 },
                    PostsByDay = new Dictionary<DateTime, int>()
                };
            }

            foreach (var tweet in tweets)
            {
                if (tweet?.Text == null) continue;

                // Contagem de palavras
                var words = tweet.Text.ToLower().Split(' ').Where(w => !stopWords.Contains(w) && w.Length > 3);
                foreach (var word in words)
                {
                    wordCount[word] = wordCount.GetValueOrDefault(word) + 1;
                }

                // Sentimento
                int sentiment = 0;
                if (positiveWords.Any(w => tweet.Text.ToLower().Contains(w))) sentiment = 1;
                else if (negativeWords.Any(w => tweet.Text.ToLower().Contains(w))) sentiment = -1;
                sentimentScores.Add(sentiment);

                // Posts por dia
                var date = tweet.CreatedAt.Date;
                postsByDay[date] = postsByDay.GetValueOrDefault(date) + 1;
            }

            return new AnalysisResult
            {
                TopWords = wordCount.OrderByDescending(kv => kv.Value).Take(5).ToDictionary(kv => kv.Key, kv => kv.Value),
                Sentiment = new
                {
                    Positive = sentimentScores.Count(s => s > 0),
                    Negative = sentimentScores.Count(s => s < 0),
                    Neutral = sentimentScores.Count(s => s == 0)
                },
                PostsByDay = postsByDay
            };
        }
    }

    public class AnalysisResult
    {
        public Dictionary<string, int>? TopWords { get; set; }
        public object? Sentiment { get; set; }
        public Dictionary<DateTime, int>? PostsByDay { get; set; }
    }
}