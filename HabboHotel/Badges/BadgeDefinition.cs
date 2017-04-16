namespace ProjectHub.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        private string Code;
        private string RequiredRight;

        public BadgeDefinition(string _Code, string _RequiredRight)
        {
            Code = _Code;
            RequiredRight = _RequiredRight;
        }

        public string GetCode
        {
            get { return Code; } set { Code = value; }
        }

        public string GetRequiredRight
        {
            get { return RequiredRight; } set { RequiredRight = value; }
        }
    }
}