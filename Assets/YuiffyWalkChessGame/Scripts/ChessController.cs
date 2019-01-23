﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonUtil;

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
    public int maxHp = 500;
    public int atkRange = 300;
    public int jumpRange = 1;
    public int jumpSpeed = 500;
    public int team;
    public int enemyTeam;
    public float atkCooldown = 1.0f;
    public float beforeAtkTime = 0.2f;
    public float afterAtkTime = 0.3f;
    public int bulletSpeed = 500;
    public Color bulletColor = Color.red;
    public Color chessColor = Color.white;

    private int hp;

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
    private GameObject hpBar;

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
        hp = maxHp;
        attr = new BattleUnitAttr();
        getAttrs();
        GetComponent<MeshRenderer>().material.color = chessColor;
        GameObject HpBar = Resources.Load("Prefabs/HpBar") as GameObject;
        hpBar = GameObject.Instantiate(HpBar);
        hpBar.transform.parent = GameObject.Find("HpCanvas").transform;
        Update2DObj();
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
                    if (!aim || !aim.gameObject) {
                        SearchAim();
                        if (!aim) return;//没目标就不用动了
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
                        //Debug.Log("Fight!"+tf.position+","+aim.tf.position);
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
                    float jumpingProgress = jumpingTimeCost * jumpSpeed / 100;
                    if (jumpingProgress >= 1.0f) {
                        SitDown();
                    }
                    else
                    {
                        Vector3 newPos = Vector3.Lerp(board.GetChessPosition(x, y), board.GetChessPosition(jumpingTo.x, jumpingTo.y), jumpingProgress);
                        //Debug.Log("newPos="+newPos);
                        tf.SetPositionAndRotation(newPos, tf.rotation);
                    }
                    Update2DObj();
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
        aim = null;
        ChessController[] chesses = board.chesses;
        foreach (ChessController chess in chesses) {
            if (chess && chess.gameObject && chess.team == enemyTeam)
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
        shoot();

        //aim.BeAttacked(new AtkAttr(atk), this);
        nowAttackCooldown = atkCooldown;
    }

    void shoot() {
        GameObject Bullet = Resources.Load("Prefabs/Bullet") as GameObject;
        GameObject bullet = GameObject.Instantiate(Bullet);
        bullet.transform.parent = GameObject.Find("Bullets").transform;
        bullet.transform.position = tf.position;
        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        bulletCtrl.aim = aim.gameObject;
        bulletCtrl.atkAttr = new AtkAttr(atk);
        bulletCtrl.speed = bulletSpeed;
        bulletCtrl.SetAttr(bulletColor);
        bulletCtrl.fromObj = this.gameObject;
    }

    public void BeAttacked(AtkAttr atkAttr, System.Object fromObj) {
        hp -= atkAttr.atk;
        Update2DObj();
        if (hp <= 0) {
            Destroy(hpBar.gameObject);
            Destroy(this.gameObject);
        }
    }

    void Update2DObj() {
        Vector2 pos2d = RectTransformUtility.WorldToScreenPoint(GameObject.Find("Main Camera").GetComponent<Camera>(), tf.position + new Vector3(0,1.5f,0));
        hpBar.transform.position = pos2d;
        hpBar.GetComponent<Image>().fillAmount = 1.0f*hp/maxHp;
    }
}