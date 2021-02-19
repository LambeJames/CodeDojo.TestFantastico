namespace CodeDojo.TestFantastico.Models
{
    public class TestCaseRandomStringsConfiguration
    {
        public bool ExcludeSpecialChars { get; set; }

        public bool ExcludeNumeric { get; set; }

        public bool ExcludeUpper { get; set; }

        public bool ExcludeLower { get; set; }

        public int Min { get; set; } = 0;

        public int Max { get; set; } = 50;
    }
}
