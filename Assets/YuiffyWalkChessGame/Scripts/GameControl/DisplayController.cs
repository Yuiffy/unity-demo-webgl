using System.Collections;
using UnityEngine;
using static MyUtil.CommonUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using DataEntity;
using MyGameController;
using Player;
using UnityEngine.UI;

namespace unity_demo_webgl.Assets.YuiffyWalkChessGame.Scripts.GameControl {

    public class DisplayController : MonoBehaviour {

        private List<PlayerInfo> players;
        private PlayerInfo localPlayer;
        private DataController dataController;
        public Text scoreBoard;
        public Text localMoney;
        void Start () {

        }

        public void DataReady (DataController _dataController, PlayerInfo localPlayerInfo) {
            dataController = _dataController;
            players = dataController.players;
            localPlayer = localPlayerInfo;
            dataController.OnPlayerHpChange += PlayerHpChangeHandler;
            Debug.Log("!");
            UpdateDisplay ();
        }

        private void PlayerHpChangeHandler (List<PlayerInfo> _players) {
            players = _players;
            UpdateDisplay ();
        }

        void UpdateDisplay () {
            var nameAndScore = players.Select (person => person.name + " " + person.hp).ToArray ();
            string scoreBoardString = string.Join ("\n", nameAndScore);
            Debug.Log ("update display " + scoreBoardString);
            scoreBoard.text = scoreBoardString;
            localMoney.text = "$"+localPlayer.money;
        }
    }
}