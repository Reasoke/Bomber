using System;
using System.Collections.Generic;

namespace Ira.Game
{
    public enum ShopMenuItem
    {
        Life,
        BombCountBonus,
        BombPowerBonus,
        Armor,
        Nothing,
    }

    public static class ShopHelper
    {
        public static Dictionary<ShopMenuItem, int> Prices = new Dictionary<ShopMenuItem, int> {
            {ShopMenuItem.Life, 5},
            {ShopMenuItem.BombCountBonus, 3},
            {ShopMenuItem.BombPowerBonus, 3},
            {ShopMenuItem.Armor, 4},
            {ShopMenuItem.Nothing, 0},
        };
        
        //ShopMenuItem extension method
        public static string ToTitle(this ShopMenuItem item)
        {
            switch (item)
            {
                case ShopMenuItem.Life:
                    return "Life";
                case ShopMenuItem.BombCountBonus:
                    return "Bomb count";
                case ShopMenuItem.BombPowerBonus:
                    return "Bomb power";
                case ShopMenuItem.Armor:
                    return "Armor";
                case ShopMenuItem.Nothing:
                    return "Nothing";
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, null);
            }
        }
    }
}