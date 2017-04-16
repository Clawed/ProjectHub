namespace Plus.HabboHotel.Catalog.Pets
{
    public class PetRace
    {
        private int RaceId;
        private int PrimaryColour;
        private int SecondaryColour;
        public bool HasPrimaryColour;
        public bool HasSecondaryColour;

        public PetRace(int _RaceId, int _PrimaryColour, int _SecondaryColour, bool _HasPrimaryColour, bool _HasSecondaryColour)
        {
            RaceId = _RaceId;
            PrimaryColour = _PrimaryColour;
            SecondaryColour = _SecondaryColour;
            HasPrimaryColour = _HasPrimaryColour;
            HasSecondaryColour = _HasSecondaryColour;
        }

        public int GetRaceId
        {
            get { return RaceId; } set { RaceId = value; }
        }

        public int GetPrimaryColour
        {
            get { return PrimaryColour; } set { PrimaryColour = value; }
        }

        public int GetSecondaryColour
        {
            get { return SecondaryColour; } set { SecondaryColour = value; }
        }

        public bool GetHasPrimaryColour
        {
            get { return HasPrimaryColour; } set { HasPrimaryColour = value; }
        }

        public bool GetHasSecondaryColour
        {
            get { return HasSecondaryColour; } set { HasSecondaryColour = value; }
        }
    }
}
