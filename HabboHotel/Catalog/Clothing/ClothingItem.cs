using System;
using System.Collections.Generic;

namespace ProjectHub.HabboHotel.Catalog.Clothing
{
    public class ClothingItem
    {
        public int Id { get; set; }
        public string ClothingName { get; set; }
        public List<int> PartIds { get; private set; }

        public ClothingItem(int _Id, string _ClothingName, string _PartIds)
        {
            Id = _Id;
            ClothingName = _ClothingName;
            PartIds = new List<int>();

            if (_PartIds.Contains(","))
            {
                foreach (string PartId in _PartIds.Split(','))
                {
                    PartIds.Add(int.Parse(PartId));
                }
            }
            else if (!String.IsNullOrEmpty(_PartIds) && (int.Parse(_PartIds)) > 0)
            {
                PartIds.Add(int.Parse(_PartIds));
            }
        }
    }
}
