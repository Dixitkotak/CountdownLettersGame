using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CountdownGame
{
    public class Program
    {
        private const string Consonants = "BCDFGHJKLMNPQRSTVWXYZ";
        private const string Vowels = "AEIOU";
        private const int TotalRounds = 4;
        private const int TotalLetters = 9;
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Random Random = new Random();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Please wait.....");
            var wordsDictionary = await FetchWordsDictionaryAsync();
            if (wordsDictionary == null)
            {
                Console.WriteLine("Failed to load the words dictionary.");
                return;
            }

            int totalScore = 0;
            for (int round = 1; round <= TotalRounds; round++)
            {
                Console.WriteLine($"Round {round}:");
                List<char> letters = GenerateLetters();
                Console.WriteLine("Letters: " + string.Join(" ", letters));

                string longestWord = FindLongestWord(letters, wordsDictionary);
                int score = longestWord.Length;
                totalScore += score;

                Console.WriteLine($"Longest Word: {longestWord}, Score: {score}\n");
            }

            Console.WriteLine($"Total Score: {totalScore}");
        }

        public static List<char> GenerateLetters()
        {
            List<char> letters = new List<char>();
            for (int i = 0; i < TotalLetters; i++)
            {
                char choice = GetLetterChoice();
                letters.Add(choice == 'C' ? Consonants[Random.Next(Consonants.Length)] : Vowels[Random.Next(Vowels.Length)]);
                Console.WriteLine("Letters: " + string.Join(" ", letters));
            }
            return letters;
        }

        public static char GetLetterChoice()
        {
            while (true)
            {
                Console.Write("Choose (C)onsonant or (V)owel: ");
                char choice = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();
                if (choice == 'C' || choice == 'V') return choice;
                Console.WriteLine("Invalid choice, try again.");
            }
        }

        public static async Task<Dictionary<string, int>> FetchWordsDictionaryAsync()
        {
            const string apiUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_dictionary.json";
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            catch
            {
                return null;
            }
        }

        public static string FindLongestWord(List<char> letters, Dictionary<string, int> wordsDictionary)
        {
            return wordsDictionary.Keys
                .Where(word => word.Length > 1 && CanFormWord(word, letters))
                .OrderByDescending(word => word.Length)
                .FirstOrDefault() ?? string.Empty;
        }

        public static bool CanFormWord(string word, List<char> letters)
        {
            int[] charCount = new int[26];
            foreach (var letter in letters)
                charCount[letter - 'A']++;

            foreach (var letter in word.ToUpper())
                if (--charCount[letter - 'A'] < 0)
                    return false;

            return true;
        }
    }
}
