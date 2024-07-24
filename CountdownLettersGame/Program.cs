using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CountdownGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Please wait.....");
            var wordsDictionary = await GetWordsDictionaryAsync();
            if (wordsDictionary == null)
            {
                Console.WriteLine("Failed to load the words dictionary.");
                return;
            }
            Console.Clear();
            int totalScore = 0;

            for (int round = 1; round <= 4; round++)
            {
                Console.WriteLine($"Round {round}:");
                List<char> letters = GetLetters();
                Console.WriteLine("Letters: " + string.Join(" ", letters));

                string longestWord = FindLongestWord(letters, wordsDictionary);
                int score = longestWord.Length;
                totalScore += score;

                Console.WriteLine($"Longest Word: {longestWord}, Score: {score}");
                Console.WriteLine();
            }

            Console.WriteLine($"Total Score: {totalScore}");
        }

        static List<char> GetLetters()
        {
            List<char> letters = new List<char>();
            Random rand = new Random();
            string consonants = "BCDFGHJKLMNPQRSTVWXYZ";
            string vowels = "AEIOU";

            for (int i = 0; i < 9; i++)
            {
                Console.Write("Choose (C)onsonant or (V)owel: ");
                char choice = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();

                if (choice == 'C')
                {
                    letters.Add(consonants[rand.Next(consonants.Length)]);
                }
                else if (choice == 'V')
                {
                    letters.Add(vowels[rand.Next(vowels.Length)]);
                }
                else
                {
                    i--; // invalid choice, redo the iteration
                    Console.WriteLine("Invalid choice, try again.");
                }
                Console.WriteLine("Letters: " + string.Join(" ", letters));
            }

            return letters;
        }

        static async Task<Dictionary<string, int>> GetWordsDictionaryAsync()
        {
            string apiUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_dictionary.json";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(responseBody);
            }
        }

        static string FindLongestWord(List<char> letters, Dictionary<string, int> wordsDictionary)
        {
            // Filter words: only include those longer than 1 character
            var validWords = wordsDictionary.Keys
                .Where(word => word.Length > 1)
                .OrderByDescending(word => word.Length)
                .ToList();

            // Check each word in descending order of length
            foreach (var word in validWords)
            {
                if (CanFormWord(word, letters))
                {
                    return word;
                }
            }

            return "";
        }

        static bool CanFormWord(string word, List<char> letters)
        {
            int[] charCount = new int[26];
            foreach (var letter in letters)
            {
                charCount[letter - 'A']++;
            }

            foreach (var letter in word)
            {
                if (--charCount[letter - 'a'] < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
