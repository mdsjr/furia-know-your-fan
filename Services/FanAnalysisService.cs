using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FuriaKnowYourFan.Models;

namespace FuriaKnowYourFan.Services
{
    public class FanAnalysisService
    {
        private readonly List<string> _positiveWords = new List<string>
        {
            "vitória", "venceu", "foda", "pantera", "monstro", "brabo", "cravou",
            "lendário", "mestre", "campeão", "ownou", "mitou", "arrebentou",
            "insano", "god", "jogaço", "clutch", "hype", "orgulho", "raiz", "vamo"
        };

        private readonly List<string> _negativeWords = new List<string>
        {
            "derrota", "perdeu", "ruim", "fraco", "decepção", "vexame",
            "afundou", "tiltou", "choke", "errou", "pipoqueiro", "fracassou",
            "vergonha", "desastre", "lento", "noob"
        };

        private readonly List<string> _neutralWords = new List<string>
        {
            "jogo", "partida", "competição", "torneio", "disputa", "placar",
            "treino", "estratégia", "elenco", "time", "jogador", "furia",
            "esports", "campeonato"
        };

        private readonly List<string> _stopWords = new List<string>
        {
            "https", "http", "t.co", "rt", "pra", "que", "com", "por", "em", "de", "e",
            "a", "o", "na", "no", "como", "sobre"
        };

        private readonly Dictionary<string, string> _wordNormalization = new Dictionary<string, string>
        {
            { "yek1ndar", "yekindar" }
        };

        public AnalysisResult AnalyzeTweets(List<Tweet>? tweets)
        {
            var result = new AnalysisResult
            {
                TopWords = new Dictionary<string, int>(),
                Sentiment = new Sentiment { Positive = 0, Negative = 0, Neutral = 0 },
                PostsByDay = new Dictionary<string, int>(),
                TopWordsTweets = new Dictionary<string, List<string>>()
            };

            if (tweets == null || !tweets.Any())
            {
                return result;
            }

            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var wordTweets = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var tweet in tweets)
            {
                // Contagem de palavras
                if (!string.IsNullOrEmpty(tweet.Text))
                {
                    var words = Regex.Split(tweet.Text.ToLower(), @"\W+")
                        .Where(w => w.Length > 2 && !string.IsNullOrWhiteSpace(w) && !_stopWords.Contains(w.ToLower()))
                        .Select(w => _wordNormalization.ContainsKey(w) ? _wordNormalization[w] : w)
                        .ToList();

                    foreach (var word in words)
                    {
                        wordCounts[word] = wordCounts.GetValueOrDefault(word, 0) + 1;
                        if (!wordTweets.ContainsKey(word))
                        {
                            wordTweets[word] = new List<string>();
                        }
                        if (!wordTweets[word].Contains(tweet.Text))
                        {
                            wordTweets[word].Add(tweet.Text);
                        }
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
                if (!string.IsNullOrEmpty(tweet.CreatedAt))
                {
                    var date = DateTime.Parse(tweet.CreatedAt).ToUniversalTime().Date;
                    Console.WriteLine($"Processando tweet com CreatedAt: {tweet.CreatedAt}, Date UTC: {date}");
                    var dateKey = date.ToString("o");
                    result.PostsByDay[dateKey] = result.PostsByDay.GetValueOrDefault(dateKey, 0) + 1;
                }
            }

            // Selecionar top 5 palavras
            result.TopWords = wordCounts.OrderByDescending(w => w.Value)
                .Take(5)
                .ToDictionary(w => w.Key, w => w.Value);

            // Mapear tweets para as top palavras
            foreach (var topWord in result.TopWords.Keys)
            {
                if (wordTweets.ContainsKey(topWord))
                {
                    result.TopWordsTweets[topWord] = wordTweets[topWord];
                }
            }

            return result;
        }
    }

    public class AnalysisResult
    {
        public Dictionary<string, int> TopWords { get; set; } = new Dictionary<string, int>();
        public Sentiment Sentiment { get; set; } = new Sentiment();
        public Dictionary<string, int> PostsByDay { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, List<string>> TopWordsTweets { get; set; } = new Dictionary<string, List<string>>();
    }

    public class Sentiment
    {
        public int Positive { get; set; }
        public int Negative { get; set; }
        public int Neutral { get; set; }
    }
}