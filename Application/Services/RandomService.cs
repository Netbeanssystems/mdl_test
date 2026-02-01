using Application.ServiceInterfaces;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Services
{
    public class RandomService : IRandomService
    {
        private readonly Random _random = new Random();
        public Task<int> RandomNumber(int min, int max)
        {
            return Task.FromResult(_random.Next(min, max));
        }
        public async Task<string> RandomPassword()
        {
            var passwordBuilder = new StringBuilder();
            // 2-Letters upper case
            var str = await RandomString(2, false).ConfigureAwait(false);
            passwordBuilder.Append(str);
            // 2-Letters lower case
            var randString = await RandomString(2).ConfigureAwait(false);
            passwordBuilder.Append(randString);
            // 2-Digits between 10 and 99
            var randNum = await RandomNumber(10, 99).ConfigureAwait(false);
            passwordBuilder.Append(randNum);
            // 2-Special Characters
            var randAlpha = await RandomSpecialChars().ConfigureAwait(false);
            passwordBuilder.Append(randAlpha);
            Random r = new Random();
            string password = new string(passwordBuilder.ToString().OrderBy(s => (r.Next(2) % 2) == 0).ToArray());
            return password;
        }
        public Task<string> RandomString(int size, bool lowerCase = true)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return Task.FromResult(lowerCase ? builder.ToString().ToLower() : builder.ToString());
        }
        public Task<string> RandomSpecialChars(int size = 2)
        {
            var builder = new StringBuilder(size);
            //var validChars = "!@#$%^&*?_-";
            var validChars = "@#$";
            var random = new Random();
            var chars = new char[size];
            for (var i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
                builder.Append(chars[i]);
            }
            return Task.FromResult(builder.ToString());
        }
        public async Task<string> GetUserName(string str, bool rnd = false)
        {
            var subName = str.Length >= 8 ? str : str + await RandomString(8 - str.Length).ConfigureAwait(false);
            var userName = subName.Replace(" ", "").Replace(".", "").Substring(0, 7).ToUpper();
            if (!rnd) return $"{userName}";
            var randomNum = await RandomNumber(100, 999).ConfigureAwait(false);
            return $"{userName}{randomNum}";
        }
    }
}
