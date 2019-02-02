using System;

namespace MyConfig
{
    [Serializable]
    public class Ui
    {
        public string color;
        public string name;
        public string headText;
    }
    [Serializable]
    public class Chess
    {
        public string keyName;
        public int hp;
        public int atk;
        public Ui ui;
    }
    [Serializable]
    public class Shop
    {
        public string keyName;
        public int count;
    }

    [Serializable]
    public class Rule
    {
        public int playerShopItemCount;
    }
    [Serializable]
    public class Config
    {
        public Chess[] chess;
        public Shop[] shop;
        public Rule rule;
    }
}