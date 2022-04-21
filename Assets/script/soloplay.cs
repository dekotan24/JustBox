using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class soloplay : MonoBehaviour
{
    bool ongame = false;   //ゲーム実行中かどうか。
    bool canroll = false;   //ダイスを振れる状態か
    bool[] card;    //各カードの状態
    bool soundmute = false;  //BGMミュート
    bool cheat = false; //チートモード
    int dice1, dice2;   //各ダイスの値
    int total;  //ダイスの合計の目
    int remain; //残りの目
    int selected_dice;  //選択中のカードの値
    int already_selected;   //選択済みのカードの数
    int remain_card;    //残りのカードの数

    public GameObject card1_obj;
    public GameObject card2_obj;
    public GameObject card3_obj;
    public GameObject card4_obj;
    public GameObject card5_obj;
    public GameObject card6_obj;
    public GameObject card7_obj;
    public GameObject card8_obj;
    public GameObject card9_obj;
    public GameObject dice1_obj;
    public GameObject dice2_obj;
    public AudioSource bgm;
    public AudioSource se;
    public AudioClip dicese;
    public AudioClip cantse;
    public AudioClip stuckse;
    public AudioClip clearse;
    public AudioClip gameclear;
    public AudioClip gamefailed;
    public Material c1;
    public Material c2;
    public Material c3;
    public Material c4;
    public Material c5;
    public Material c6;
    public Material c7;
    public Material c8;
    public Material c9;
    public Material c;
    public Material d1;
    public Material d2;
    public Material d3;
    public Material d4;
    public Material d5;
    public Material d6;
    public Material d;
    public Text remain_obj;
    public Text selected_obj;
    public Text stat_obj;


    // Start is called before the first frame update
    void Start()
    {
        /* <--- 各変数の初期化ここから ---> */
        // <--- コマの初期化 --->
        card = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            card[i] = true;
        }

        // <--- 表示の初期化 --->
        card1_obj.GetComponent<Image>().material = c1;
        card2_obj.GetComponent<Image>().material = c2;
        card3_obj.GetComponent<Image>().material = c3;
        card4_obj.GetComponent<Image>().material = c4;
        card5_obj.GetComponent<Image>().material = c5;
        card6_obj.GetComponent<Image>().material = c6;
        card7_obj.GetComponent<Image>().material = c7;
        card8_obj.GetComponent<Image>().material = c8;
        card9_obj.GetComponent<Image>().material = c9;
        dice1_obj.GetComponent<Image>().material = d;
        dice2_obj.GetComponent<Image>().material = d;

        // <--- ダイスの初期化 --->
        dice1 = -1;
        dice2 = -1;
        total = 0;
        remain = 0;

        // <--- テキストの初期化 --->
        remain_obj.text = "残り：--";

        // <--- 変数の初期化 --->
        selected_dice = 1;
        remain_card = 9;
        already_selected = 0;
        ongame = true;
        canroll = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cheat = true;
            stat_obj.text = "CheatMode Enabled!";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Esc
            SceneManager.LoadScene("Title");    //タイトルに戻る
        }

        if (ongame)
        {
            /* <--- ゲームが実行されている場合 ---> */
            if (canroll)
            {
                /* <--- ダイスを振ることができる場合 ---> */
                if (Input.GetKeyDown(KeyCode.D))
                {
                    /* <--- Dキーが押された場合 ---> */
                    dice1 = diceroll();
                    dice2 = diceroll();
                    total = dice1 + dice2;
                    remain = total;
                    diceapply();
                    textupdate();
                    already_selected = 0;
                    if (isstuck())
                    {
                        /* スタックした場合、ゲーム終了 */
                        bgm.clip = gamefailed;
                        se.clip = stuckse;
                        bgm.Play();
                        se.Play();
                        stat_obj.text = "GAME OVER!!";
                        ongame = false;
                    }
                    canroll = false;
                }
            }
            else
            {
                /* <--- カードを選択する場合 ---> */
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    //←キー
                    if (selected_dice == 1)
                    {
                        selected_dice = 9;
                    }
                    else
                    {
                        selected_dice--;
                    }
                    selectchange(selected_dice);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //→キー
                    if (selected_dice == 9)
                    {
                        selected_dice = 1;
                    }
                    else
                    {
                        selected_dice++;
                    }
                    selectchange(selected_dice);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    //Space
                    if (challenge(selected_dice, remain))
                    {
                        //チャレンジに成功した場合
                        Debug.Log("チャレンジに成功！\nselected: " + selected_dice + " / remain: " + (remain - selected_dice) + " / remain_card: " + (remain_card - 1));
                        int selected_val = selected_dice;
                        remain = remain - selected_val;
                        remain_card--;
                        card[selected_dice] = false;
                        Debug.Log("カードを使用済みに設定しました。\ncard: " + selected_dice);
                        disactivate_card(selected_dice);
                        textupdate();
                        if (remain <= 0)
                        {
                            if (remain_card <= 0)
                            {
                                if (!soundmute)
                                {
                                    se.clip = clearse;
                                    bgm.clip = gameclear;
                                    bgm.Play();
                                    se.Play();
                                    stat_obj.text = "GAME CLEAR!!";
                                }
                                ongame = false;
                            }
                            canroll = true;
                        }
                    }
                    else
                    {
                        //チャレンジに失敗した場合
                        if (!soundmute)
                        {
                            se.clip = cantse;
                            se.Play();
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    //R
                    SceneManager.LoadScene("PvE");
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    //M
                    soundmute = !soundmute;
                    if (soundmute)
                    {
                        bgm.Pause();
                    }
                    else
                    {
                        bgm.Play();
                    }
                }
            }
        }
        else
        {
            /* <--- ゲーム終了イベントここ ---> */
            if (Input.GetKeyDown(KeyCode.R))
            {
                //R
                SceneManager.LoadScene("PvE");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                //M
                soundmute = !soundmute;
                if (soundmute)
                {
                    bgm.Pause();
                }
                else
                {
                    bgm.Play();
                }
            }
        }
    }

    private int diceroll()
    {
        int ans = -1;
        if (!soundmute)
        {
            se.clip = dicese;
            se.Play();
        }
        ans = Random.Range(0, 6);   //0から6の乱数取得
        ans++;  //1~6の範囲に変換
        return ans;
    }

    private void diceapply()
    {
        int diceA = dice1;
        int diceB = dice2;
        switch (diceA)
        {
            case 1:
                dice1_obj.GetComponent<Image>().material = d1;
                break;
            case 2:
                dice1_obj.GetComponent<Image>().material = d2;
                break;
            case 3:
                dice1_obj.GetComponent<Image>().material = d3;
                break;
            case 4:
                dice1_obj.GetComponent<Image>().material = d4;
                break;
            case 5:
                dice1_obj.GetComponent<Image>().material = d5;
                break;
            case 6:
                dice1_obj.GetComponent<Image>().material = d6;
                break;
        }

        switch (diceB)
        {
            case 1:
                dice2_obj.GetComponent<Image>().material = d1;
                break;
            case 2:
                dice2_obj.GetComponent<Image>().material = d2;
                break;
            case 3:
                dice2_obj.GetComponent<Image>().material = d3;
                break;
            case 4:
                dice2_obj.GetComponent<Image>().material = d4;
                break;
            case 5:
                dice2_obj.GetComponent<Image>().material = d5;
                break;
            case 6:
                dice2_obj.GetComponent<Image>().material = d6;
                break;
        }

        return;
    }

    private void textupdate()
    {
        int remainval = remain;
        remain_obj.text = "残り：" + remainval;
        return;
    }

    private void selectchange(int val)
    {
        int ti = val;
        switch (ti)
        {
            case 1:
                selected_obj.transform.localPosition = new Vector3(-440, 115, 0);
                break;
            case 2:
                selected_obj.transform.localPosition = new Vector3(-335, 115, 0);
                break;
            case 3:
                selected_obj.transform.localPosition = new Vector3(-225, 115, 0);
                break;
            case 4:
                selected_obj.transform.localPosition = new Vector3(-110, 115, 0);
                break;
            case 5:
                selected_obj.transform.localPosition = new Vector3(0, 115, 0);
                break;
            case 6:
                selected_obj.transform.localPosition = new Vector3(110, 115, 0);
                break;
            case 7:
                selected_obj.transform.localPosition = new Vector3(225, 115, 0);
                break;
            case 8:
                selected_obj.transform.localPosition = new Vector3(335, 115, 0);
                break;
            case 9:
                selected_obj.transform.localPosition = new Vector3(445, 115, 0);
                break;

        }
    }

    private bool challenge(int select, int remains)
    {
        bool ans = false;
        int remainvalue = remains - (select);

        if (card[select] == true)
        {
            /* 選択しているカードが使用済でないか */
            if (already_selected == 1)
            {
                /* もう一枚しか選択できない場合 */
                if (remains == (select))
                {
                    /* カードが残りの目か */
                    ans = true;
                    already_selected++;
                }
                else
                {
                    Debug.Log("選択不能。理由：2回目の選択がジャストでない為。\nalready_selected: " + already_selected + " / select_dice: " + (select) + " != remain: " + remains);
                    se.clip = cantse;
                    se.Play();
                }
            }
            else
            {
                /* 初回選択の場合 */
                if (remains >= (select))
                {
                    /* カードが残りの目の範囲内に入っているか */
                    if (remainvalue >= 1 && remainvalue <= 9)
                    {
                        if (card[(remainvalue)] == true && remainvalue != (select))
                        {
                            ans = true;
                            already_selected++;
                        }
                        else
                        {
                            Debug.Log("選択不能。理由：相方のカードが使用済みもしくは同値の為。\nalready_selected: " + already_selected + " / select_dice: " + (select) + " / remain: " + remains + " / remainvalue: " + remainvalue);
                            se.clip = cantse;
                            se.Play();
                        }
                    }
                    else
                    {
                        ans = true;
                        already_selected++;
                    }
                }
                else
                {
                    Debug.Log("選択不能。理由：目の範囲外の為。\nalready_selected: " + already_selected + " / select_dice: " + (select) + " <= remains: " + remains);
                    se.clip = cantse;
                    se.Play();
                }
            }
        }
        else
        {
            Debug.Log("選択不能。理由：選択したカードが使用済みの為。\ncard[" + select + "] = " + card[select]);
            se.clip = cantse;
            se.Play();
        }
        return ans;
    }

    private void disactivate_card(int val)
    {
        switch (val)
        {
            case 1:
                card1_obj.GetComponent<Image>().material = c;
                break;
            case 2:
                card2_obj.GetComponent<Image>().material = c;
                break;
            case 3:
                card3_obj.GetComponent<Image>().material = c;
                break;
            case 4:
                card4_obj.GetComponent<Image>().material = c;
                break;
            case 5:
                card5_obj.GetComponent<Image>().material = c;
                break;
            case 6:
                card6_obj.GetComponent<Image>().material = c;
                break;
            case 7:
                card7_obj.GetComponent<Image>().material = c;
                break;
            case 8:
                card8_obj.GetComponent<Image>().material = c;
                break;
            case 9:
                card9_obj.GetComponent<Image>().material = c;
                break;
        }
        return;
    }

    private bool isstuck()
    {
        if (cheat)
        {
            return false;
        }
        bool ans = false;
        int hasroute = 0;
        for (int i = 1; i <= 9; i++)
        {
            Debug.Log("[stuck check] i = " + i);
            // カードが使用済みの場合は除外
            if (card[i] == true)
            {
                // 同じカードを2回使う計算を除外
                if (remain != (remain - i) && i != (remain - i))
                {
                    // 計算結果がマイナスになるもしくは引いた結果が選択できなカードの場合は除外
                    if ((remain - i) >= 0 && (remain - i) < 9)
                    {
                        if (card[(remain - i)] == true)
                        {
                            hasroute++;
                            Debug.Log("移動可能なコマです。\ni = " + i + " ( " + card[i] + " ) / remain = " + (remain - i) + "( " + card[(remain - i)] + " )");
                        }
                        else
                        {
                            Debug.Log("残った数のカードは使用済みです。\ncard[(" + remain + " - " + i + " = " + card[(remain - i)]);
                        }
                    }
                    else
                    {
                        Debug.Log("残った数はオーバーフローします。\n" + (remain - i) + " >= 0 && " + (remain - i) + " <= 8");
                    }
                }
                else
                {
                    Debug.Log("[stuck check] 同数で割ろうとした。\n" + remain + " != " + (remain - i) + " && " + i + " != " + (remain - i));
                }
            }
            else
            {
                Debug.Log("[stuck check] card[" + i + "]は使用済み");
            }
        }

        if (hasroute == 0)
        {
            ans = true;
        }


        return ans;
    }
}
