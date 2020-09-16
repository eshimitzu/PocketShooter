using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Heyworks.PocketShooter.Meta.Utils
{
    public static class NicknameValidator
    {
        private static string[] CurseWords = { "fuck", "bitch", "хуй", "хуи", "пизд", "блять" };

        private static readonly Regex NicknameValidationRegex = new Regex(@"^(?!.{11,})[\p{L}\d\._\-\[\]]+(\s+[\p{L}\d\._\-\[\]]+)*$", RegexOptions.Compiled);

        public static bool IsValid(string nickname) =>
            !CurseWords.Any(_ => nickname.IndexOf(_, StringComparison.OrdinalIgnoreCase) >= 0) &&
            NicknameValidationRegex.IsMatch(nickname);
    }
}
