using UnityEngine;

namespace DataEntity {
    public enum PlayerType {
        LOCAL,
        NET,
        AI
        };
    public class PlayerInfo {
        public string name;
        public PlayerType type;

        public int money = 1;
        public int hp = 50;

        public PlayerInfo() { }

        public PlayerInfo(string _name, PlayerType _type) {
            name = _name;
            type = _type;
        }
    }
}