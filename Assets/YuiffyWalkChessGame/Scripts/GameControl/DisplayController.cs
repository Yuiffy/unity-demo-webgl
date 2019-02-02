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
        private DataController dataController;
        public Text scoreBoard;
        void Start () {

        }

        public void DataReady (DataController _dataController) {
            dataController = _dataController;
            players = dataController.players;
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
            string s = string.Join ("\n", nameAndScore);
            Debug.Log ("update display " + s);
            scoreBoard.text = s;
        }

    }
}