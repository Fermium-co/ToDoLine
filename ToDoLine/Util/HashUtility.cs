using System;

namespace ToDoLine.Util
{
    public static class HashUtility
    {
        public static string Hash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return BCrypt.Net.BCrypt.EnhancedHashPassword(input);
        }

        public static bool VerifyHash(string input, string hashedInput)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (hashedInput == null)
                throw new ArgumentNullException(nameof(hashedInput));

            return BCrypt.Net.BCrypt.EnhancedVerify(input, hashedInput);
        }
    }
}
