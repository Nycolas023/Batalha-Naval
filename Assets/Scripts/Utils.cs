using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Utils {
    public class ThemeShopItem {
        public string Name { get; set; }
        public string Price { get; set; }
        public string PreviewImagePath { get; set; }
        public bool IsPurchased { get; set; }
        public Sprite PreviewImage { get; set; }
    }

    public class BombShopItem {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string PreviewImagePath { get; set; }
        public int Type { get; set; }
        public int Quantity { get; set; }
        public Sprite PreviewImage { get; set; }
    }

    public int[,] RotateMatrix(int[,] matrix) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[,] rotatedMatrix = new int[cols, rows];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                rotatedMatrix[j, rows - 1 - i] = matrix[i, j];
            }
        }

        return rotatedMatrix;
    }

    public PlayerModel ParseFromJson(SimpleJSON.JSONNode json) {
        return new PlayerModel(
            json["user_id"].AsInt,
            json["user_nickname"],
            json["user_login"],
            json["user_money_amount"].AsInt,
            json["user_present_theme"],
            json["user_match_victory"].AsInt,
            json["user_match_defeat"].AsInt,
            json["user_match_boats_sunk"].AsInt
        );
    }

    public List<ThemeShopItem> ParseThemeShopItems(JSONNode json) {
        var shopItems = new List<ThemeShopItem>();

        foreach (var item in json.AsArray) {
            var node = item.Value;
            var shopItem = new ThemeShopItem {
                Name = node["name"],
                Price = node["price"],
                PreviewImagePath = node["previewImagePath"],
                IsPurchased = node["isPurchased"].AsBool
            };
            shopItems.Add(shopItem);
        }

        return shopItems;
    }

    public List<BombShopItem> ParseBombShopItems(JSONNode json) {
        var shopItems = new List<BombShopItem>();

        foreach (var item in json.AsArray) {
            var node = item.Value;
            var shopItem = new BombShopItem {
                Id = node["bomb_id"].AsInt,
                Name = node["bomb_name"],
                Price = node["bomb_price"],
                PreviewImagePath = node["bomb_image"],
                Type = node["bomb_type"].AsInt,
                Quantity = node["stored_quantity"].AsInt,
            };
            shopItems.Add(shopItem);
        }

        return shopItems;
    }
}
