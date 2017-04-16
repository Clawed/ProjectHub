using ProjectHub.HabboHotel.Catalog.Clothing;
using ProjectHub.HabboHotel.Catalog.Marketplace;
using ProjectHub.HabboHotel.Catalog.Pets;
using System.Collections.Generic;

namespace ProjectHub.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private MarketplaceManager MarketplaceManager;
        private PetRaceManager PetRaceManager;
        private VoucherManager VoucherManager;
        private ClothingManager ClothingManager;

        private Dictionary<int, int> _itemOffers;
        private Dictionary<int, CatalogPage> _pages;
        private Dictionary<int, CatalogBot> _botPresets;
        private Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private Dictionary<int, Dictionary<int, CatalogDeal>> _deals;

        public CatalogManager()
        {
            MarketplaceManager = new MarketplaceManager();
            PetRaceManager = new PetRaceManager();
            VoucherManager = new VoucherManager();
            ClothingManager = new ClothingManager();

            this._itemOffers = new Dictionary<int, int>();
            this._pages = new Dictionary<int, CatalogPage>();
            this._botPresets = new Dictionary<int, CatalogBot>();
            this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            this._deals = new Dictionary<int, Dictionary<int, CatalogDeal>>();
        }
    }
}
