using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuriaKnowYourFan.Services
{
    public class FanAnalysisService
    {
        private readonly List<string> _positiveWords = new List<string> { "ótimo", "excelente", "fantástico", "incrível", "maravilhoso", "bom", "vitoria", "campeão", "prontos", "vamo" };
        private readonly List<string> _negativeWords = new List<string> { "ruim", "terrível", "péssimo", "derrota", "fraco", "decepção" };

        public AnalysisResult AnalyzeTweets(List<Tweet>? tweets)
        {
            var result = new AnalysisResult
            {
                TopWords = new Dictionary<string, int>(),
                Sentiment = new Sentiment { Positive = 0, Negative = 0, Neutral = 0 },
                PostsByDay = new Dictionary<string, int>()
            };

            if (tweets == null || !tweets.Any())
            {
                return result;
            }

            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var tweet in tweets)
            {
                // Contagem de palavras
                if (!string.IsNullOrEmpty(tweet.Text))
                {
                    var words = Regex.Split(tweet.Text.ToLower(), @"\W+")
                        .Where(w => w.Length > 2 && !string.IsNullOrWhiteSpace(w))
                        .ToList();

                    foreach (var word in words)
                    {
                        wordCounts[word] = wordCounts.GetValueOrDefault(word, 0) + 1;
                    }

                    // Análise de sentimento
                    bool hasPositive = _positiveWords.Any(pw => tweet.Text.ToLower().Contains(pw));
                    bool hasNegative = _negativeWords.Any(nw => tweet.Text.ToLower().Contains(nw));

                    if (hasPositive && !hasNegative)
                        result.Sentiment.Positive++;
                    else if (hasNegative && !hasPositive)
                        result.Sentiment.Negative++;
                    else
                        result.Sentiment.Neutral++;
                }

                // Posts por dia
                var date = tweet.CreatedAt.Date; // Usa a data local sem fuso horário
                var dateKey = date.ToString("o"); // Formato ISO 8601
                result.PostsByDay[dateKey] = result.PostsByDay.GetValueOrDefault(dateKey, 0) + 1;
            }

            // Selecionar top 5 palavras
            result.TopWords = wordCounts.OrderByDescending(w => w.Value)
                .Take(5)
                .ToDictionary(w => w.Key, w => w.Value);

            return result;
        }
    }

    public class AnalysisResult
    {
        public Dictionary<string, int> TopWords { get; set; } = new Dictionary<string, int>();
        public Sentiment Sentiment { get; set; } = new Sentiment();
        public Dictionary<string, int> PostsByDay { get; set; } = new Dictionary<string, int>();
    }

    public class Sentiment
    {
        public int Positive { get; set; }
        public int Negative { get; set; }
        public int Neutral { get; set; }
    }
}