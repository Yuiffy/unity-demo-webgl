using UnityEngine;
using UnityEditor;
namespace MyUtil
{

    static public class CommonUtil
    {
        static public float DEFAULT_JUMP_COOLDOWN = 0.5f;

        public class AtkAttr
        {
            public int atk;
            public int type;//TODO: 做成enum
                            //TODO: 增加更多东西？
            public AtkAttr(int _atk)
            {
                atk = _atk;
            }
        }

        public enum ChessState
        {
            MANAGE,
            BATTLE,
            JUMPING,
            ATTACKING,
            READY,
            FREEZE
        };

        public enum GameFlowState{
            MANAGE,
            BATTLE,
            READY
        };
    }
}