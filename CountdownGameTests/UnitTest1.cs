using Microsoft.VisualStudio.TestTools.UnitTesting;
using CountdownGame;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountdownGameTests
{
    [TestClass]
    public class CountdownGameTests
    {
        private readonly List<char> _letters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

        [TestMethod]
        public void TestCanFormWord_Valid()
        {
            string word = "FACE";
            bool result = Program.CanFormWord(word, _letters);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCanFormWord_Invalid()
        {
            string word = "JUMP";
            bool result = Program.CanFormWord(word, _letters);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task TestFetchWordsDictionaryAsync()
        {
            var dictionary = await Program.FetchWordsDictionaryAsync();
            Assert.IsNotNull(dictionary);
            Assert.IsTrue(dictionary.ContainsKey("hello"));
        }

        [TestMethod]
        public void TestFindLongestWord()
        {
            var wordsDictionary = new Dictionary<string, int>
            {
                { "hello", 1 },
                { "world", 1 },
                { "code", 1 },
                { "face", 1 }
            };

            List<char> letters = new List<char> { 'C', 'O', 'D', 'E' };
            string longestWord = Program.FindLongestWord(letters, wordsDictionary);
            Assert.AreEqual("code", longestWord);
        }
    }
}
