﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ChessState {
    MANAGE,
    BATTLE,
    JUMPING,
    ATTACKING
};

public class ChessController : MonoBehaviour
{
    public int x;
    public int y;
    public int atk = 50;
    public int hp = 500;
    public int atkRange = 300;
    public int jumpRange = 1;
    public int jumpSpeed = 500;
    public int team;
    public int enemyTeam;
    public float atkCooldown = 1.0f;
    public float beforeAtkTime = 0.2f;
    public float afterAtkTime = 0.3f;

    private ChessBoardController board;
    private Transform tf;

    private ChessController aim;
    private ChessState state = ChessState.BATTLE;
    private BattleUnitAttr attr;

    private Vector2Int jumpingTo;
    private float jumpingTimeCost = 0.0f;

    private float jumpingCooldown = 0.0f;
    private float nowAttackCooldown = 0.0f;

    private float attackDoingTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void BoardReady(ChessBoardController _board)
    {
        board = _board;
        tf = GetComponent<Transform>();
        Vector3 pos = board.GetChessPosition(x, y);
        tf.SetPositionAndRotation(pos, tf.rotation);
        attr = new BattleUnitAttr();
        getAttrs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Each physics step..
    void FixedUpdate()
    {
        if (nowAttackCooldown > 0.0f) nowAttackCooldown -= Time.deltaTime;
        switch (state) {
            case ChessState.BATTLE:
                {
                    //减少跳跃冷却
                    if (jumpingCooldown > 0.0f) jumpingCooldown -= Time.deltaTime;
                    //没目标就寻找目标
                    if (!aim) {
                        SearchAim();
                    }
                    if (Vector3.Distance(tf.position, aim.tf.position) > attr.realAtkRange)
                    {
                        //距离不够，找路跳过去打
                        if (jumpingCooldown > 0.0f)
                        {
                            //跳跃没冷却好，动不了。
                            //TODO: 可以在这重新寻找目标
                        }
                        else {
                            StartWalk();
                            Debug.Log("Start Walk!" + tf.position + "," + aim.tf.position + "," + x + y + "," + jumpingTo);
                        }
                    }
                    else {
                        //距离够，打
                        Debug.Log("Fight!"+tf.position+","+aim.tf.position);
                        if (nowAttackCooldown <= beforeAtkTime)
                        {
                            StartAttack();
                        }
                        else {
                            //攻击冷却没好，不能攻击
                        }
                    }
                    break;
                }
            case ChessState.JUMPING:
                {
                    jumpingTimeCost += Time.deltaTime;
                    if (jumpingTimeCost >= 1.0f) {
                        SitDown();
                    }
                    Vector3 newPos = Vector3.Lerp(board.GetChessPosition(x, y), board.GetChessPosition(jumpingTo.x, jumpingTo.y), jumpingTimeCost);
                    Debug.Log("newPos="+newPos);
                    tf.SetPositionAndRotation(newPos, tf.rotation);
                    break;
                }
            case ChessState.ATTACKING:
                {
                    attackDoingTime += Time.deltaTime;
                    if (attackDoingTime >= beforeAtkTime) {
                        DoAttack();
                    }
                    break;
                }
            default:
                break;
        }
    }

    void SearchAim() {
        ChessController[] chesses = board.chesses;
        foreach (ChessController chess in chesses) {
            if (chess.team == enemyTeam)
            {
                aim = chess;
                break;
            }
        }
    }

    void getAttrs() {
        attr.realAtkRange = (float)(atkRange / 100.0 * board.GetBlockRange());
    }

    void StartWalk() {
        jumpingTo = findPathNextBlock();
        state = ChessState.JUMPING;
        jumpingTimeCost = 0.0f;
    }

    void SitDown() {
        state = ChessState.BATTLE;
        x = jumpingTo.x;
        y = jumpingTo.y;
        Vector3 pos = board.GetChessPosition(x, y);
        tf.SetPositionAndRotation(pos, tf.rotation);
        jumpingCooldown = CommonUtil.DEFAULT_JUMP_COOLDOWN;
    }

    Vector2Int findPathNextBlock() {
        int xMore = aim.x - x;
        int yMore = aim.y - y;
        int nextX = x;
        int nextY = y;
        if (Math.Abs(yMore) >= Math.Abs(xMore))
        {
            nextY = y + (yMore / Math.Abs(yMore) * Math.Min(jumpRange, Math.Abs(yMore)));
        }
        else {
            nextX = x + (xMore / Math.Abs(xMore) * Math.Min(jumpRange, Math.Abs(xMore)));
        }
        //TODO: 斜着走？现在只能横竖走
        //TODO: 判断已经被占位置，自动寻路
        //TODO: 刺客式绕后大跳？
        return new Vector2Int(nextX, nextY);
    }

    void StartAttack() {
        state = ChessState.ATTACKING;
        attackDoingTime = 0.0f;
    }

    void DoAttack() {
        state = ChessState.BATTLE;
        //TODO: 放波
        aim.hp -= atk;
        nowAttackCooldown = atkCooldown;
    }
}