
namespace ProjectHub.HabboHotel.Rooms.Chat.WordFilter
{
    sealed class WordFilter
    {
        private string Word;
        private string Replacement;
        private bool Strict;
        private bool Bannable;

        public WordFilter(string _Word, string _Replacement, bool _Strict, bool _Bannable)
        {
            Word = _Word;
            Replacement = _Replacement;
            Strict = _Strict;
            Bannable = _Bannable;
        }

        public string GetWord
        {
            get { return Word; }
        }

        public string GetReplacement
        {
            get { return Replacement; }
        }
        public bool GetStrict
        {
            get { return Strict; }
        }
        public bool GetBannable
        {
            get { return Bannable; }
        }
    }
}
