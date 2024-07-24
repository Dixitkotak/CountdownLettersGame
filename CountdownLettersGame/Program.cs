using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CountdownGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int totalScore = 0;

            for (int round = 1; round <= 4; round++)
            {
                Console.WriteLine($"Round {round}:");
                List<char> letters = GetLetters();
                Console.WriteLine("Letters: " + string.Join(" ", letters));

                string longestWord =await FindLongestWordAsync(letters);
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

        static async Task<string> FindLongestWordAsync(List<char> letters)
        {
            string letterString = new string(letters.ToArray());
            string apiUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_dictionary.json";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var wordsDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(responseBody);

                var validWords = wordsDictionary.Keys
                    .Where(word => word.Length > 1) // Filtering out very short words
                    .Where(word => CanFormWord(word, letters))
                    .OrderByDescending(word => word.Length)
                    .ToList();

                string longestWord = validWords.FirstOrDefault();
                return longestWord ?? "";
            }
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
