namespace Ziggio.Csv;

public static class Constants {
  public static class RegExCheat {
    public const string AllWhitespace = @"\s+";
    public const string DoubleQuote = "\"";
    public const string FieldQuote = "[\'\"]";
    public const string FieldValue = "(.*?)";
    public const string MatchEnd = "$";
    public const string MatchStart = "^";
    public const string SingleQuote = "\'";

    public static string NegativeLookahead(string match) {
      return $"(?!{match})";
    }

    public static string NegativeLookbehind(string match) {
      return $"(?<!{match})";
    }

    public static string OneOrMoreTimes(string match) {
      return $"{match}+";
    }

    public static string PositiveLookahead(string match) {
      return $"(?={match})";
    }

    public static string PositiveLookbehind(string match) {
      return $"(?<={match})";
    }
  }

  public static class RegEx {
    // \"(.*?)\"
    public static string HeaderNames = RegExCheat.FieldQuote
      + RegExCheat.FieldValue
      + RegExCheat.FieldQuote;

    // (?<![\s\w])\"+((?!,).*?)\"+(?![\s\w\"])
    public static string DefaultFieldValues = RegExCheat.NegativeLookbehind(@"[\w\s\""]")
      + RegExCheat.OneOrMoreTimes(RegExCheat.DoubleQuote)
      + "((?!,).*?)"
      + RegExCheat.OneOrMoreTimes(RegExCheat.DoubleQuote)
      + RegExCheat.NegativeLookahead(@"[\w\s\""]");

    public static string FieldValues(string quoteCharacter) => RegExCheat.NegativeLookbehind(@"[\w\s\""]")
      + RegExCheat.OneOrMoreTimes(quoteCharacter)
      + "(.*?)"
      + RegExCheat.OneOrMoreTimes(quoteCharacter)
      + RegExCheat.NegativeLookahead(@"[\w\s\""]");

    // (?<![\s\w])\"(.*?)\"(?![\s\w])
    //public static string FieldValues = RegExCheat.NegativeLookbehind(@"[\w\s]")
    //  + RegExCheat.FieldQuote
    //  + RegExCheat.FieldValue
    //  + RegExCheat.FieldQuote
    //  + RegExCheat.NegativeLookahead(@"[\w\s]");
  }
}