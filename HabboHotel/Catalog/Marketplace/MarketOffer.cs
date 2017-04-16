namespace ProjectHub.HabboHotel.Catalog.Marketplace
{
    public class MarketOffer
    {
        public int OfferID { get; set; }
        public int ItemType { get; set; }
        public int SpriteId { get; set; }
        public int TotalPrice { get; set; }
        public int LimitedNumber { get; set; }
        public int LimitedStack { get; set; }

        public MarketOffer(int _OfferID, int _SpriteId, int _TotalPrice, int _ItemType, int _LimitedNumber, int _LimitedStack)
        {
            OfferID = _OfferID;
            SpriteId = _SpriteId;
            ItemType = _ItemType;
            TotalPrice = _TotalPrice;
            LimitedNumber = _LimitedNumber;
            LimitedStack = _LimitedStack;
        }
    }
}
