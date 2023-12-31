﻿using System.Collections.Generic;
using System.Linq;
using Engine.Services;

namespace Engine.Models
{
    public class Inventory
    {
        #region Backing Variables

        private readonly List<GameItem> _backingInventory =
            new List<GameItem>();

        private readonly List<GroupedInventoryItem> _backingGroupedInventoryItems =
            new List<GroupedInventoryItem>();
        #endregion

        #region Properties

        public IReadOnlyList<GameItem> Items => _backingInventory.AsReadOnly();

        public IReadOnlyList<GroupedInventoryItem> GroupedInventory =>
           _backingGroupedInventoryItems.AsReadOnly();

        public IReadOnlyList<GameItem> Weapons => 
             _backingInventory.ItemsThatAre(GameItem.ItemCategory.Weapon).AsReadOnly();

        public IReadOnlyList<GameItem> Consumables =>
            _backingInventory.ItemsThatAre(GameItem.ItemCategory.Consumable).AsReadOnly();

        public bool HasConsumable => Consumables.Any();

        #endregion

        #region Constructors

        public Inventory(IEnumerable<GameItem> items = null)
        {
            if (items == null)
            {
                return;
            }

            foreach(GameItem item in items)
            {
                _backingInventory.Add(item);

                AddItemToGroupedInventory(item);
            }
        }
        #endregion

        #region Public Functions

        public bool HasAllTheseItems(IEnumerable<ItemQuantity> items)
        {
            return items.All(item => Items.Count(i => i.ItemTypeID == item.ItemID) >= item.Quantity);
        }
        #endregion

        #region Priveat Functions

        //REFACTOR: look for a better way to do this (extension method?)
        private void AddItemToGroupedInventory(GameItem item)
        {
            if (item.IsUnique)
            {
                _backingGroupedInventoryItems.Add(new GroupedInventoryItem(item, 1));
            }
            else
            {
                if (_backingGroupedInventoryItems.All(gi => gi.Item.ItemTypeID != item.ItemTypeID))
                {
                    _backingGroupedInventoryItems.Add(new GroupedInventoryItem(item, 0));
                }
                _backingGroupedInventoryItems.FirstOrDefault(gi => gi.Item.ItemTypeID == item.ItemTypeID).Quantity++;
            }
        }

        #endregion

    }
}
