using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GOIS : MonoBehaviour
{
    private int selectNum = 2;
    private float reverseTime = 0f, selectTime = 0f;
    private bool next = true;
    private State state = State.Start;
    private List<Card> yamafuda;
    public GameObject card;
    public Image nextPanel;
    private Image select;
    private GameCard[] tefuda;

    public enum State
    {
        Start, Select, Change, End, Restart
    }

    public enum Yaku
    {
        NoPair, OnePair, TwoPair, ThreeCard, Straight, Flash, FullHouse, FourCard, StraightFlash, RoyalStraightFlash
    }

    GameCard SelectCard
    {
        get
        {
            return tefuda[selectNum];
        }
    }

    // Use this for initialization
    void Start()
    {
        yamafuda = new List<Card>();

        for (Card.Egara e = Card.Egara.Spade; e < Card.Egara.Diamond + 1; e++)
            for (Card.Number n = Card.Number.Ace; n < Card.Number.King + 1; n++)
                yamafuda.Add(new Card(e, n));

        Shuffle();

        select = nextPanel;

        tefuda = new GameCard[5];

        int c = tefuda.Length;

        for (int i = 0; i < c; i++)
        {
            Vector3 p = Vector3.right * (i - c / 2) * 4f;
            Card dc = yamafuda[0];
            GameCard gc = ((GameObject)Instantiate(card, p, Quaternion.identity)).GetComponent<GameCard>();

            gc.card = dc;
            yamafuda.Remove(dc);

            tefuda[i] = gc;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Start:
                GameStart();
                break;
            case State.Select:
                Select();
                break;
            case State.Change:
                Change();
                break;
            case State.End:
                GameEnd();
                break;
            case State.Restart:
                Restart();
                break;
        }
    }

    void GameStart()
    {
        SelectTimer();

        if (ButtonDown())
            ToSelect();
    }

    void Select()
    {
        SelectHorizontal();
        SelectVertical();
        SelectTimer();

        if (!ButtonDown())
            return;

        if (!next)
        {
            SelectCard.SetChange();
            return;
        }

        state = State.Change;

        foreach (var c in tefuda)
        {
            if (c.Change)
            {
                c.Ura();

                ChangeCard(c);
            }

            c.Select(false);
        }

        SetSelect();
    }

    void SelectHorizontal()
    {
        string horizontal = "Horizontal";

        if (!Input.GetButtonDown(horizontal))
            return;

        int maxNum = tefuda.Length - 1;
        float axis = Input.GetAxisRaw(horizontal);

        if (axis < 0f)
        {
            selectNum -= 1;

            if (selectNum < 0)
                selectNum = maxNum;
        }
        else if (axis > 0f)
        {
            selectNum += 1;

            if (selectNum > maxNum)
                selectNum = 0;
        }

        SetSelect();
    }

    void SelectVertical()
    {
        string vertical = "Vertical";

        if (!Input.GetButtonDown(vertical))
            return;

        float axis = Input.GetAxisRaw(vertical);

        if (axis < 0f)
            next = true;
        else if (axis > 0f)
            next = false;

        SetSelect();
    }

    void ToSelect()
    {
        state = State.Select;

        foreach (var c in tefuda)
        {
            c.Omote();
            c.Select(true);

            selectNum = 2;
            next = false;
        }

        SetSelect();
    }

    void SetSelect()
    {
        selectTime = 0f;

        select.color = SelectColor();

        select = next ? nextPanel : SelectCard.changeMark;
    }

    void Change()
    {
        if (!Reverse())
            return;

        int[] num = new int[(int)Card.Number.King];

        foreach (var c in tefuda)
        {
            if (c.Change)
                c.Omote();

            SetSelect();

            num[c.card.Num - Card.Number.Ace]++;
        }

        Debug.Log("NUM");

        for (int i = 0; i < num.Length; i++)
            Debug.Log((Card.Number.Ace + i) + " " + num[i]);

        Debug.Log((Yaku)Mathf.Max((int)Pair(num), (int)StraightFlash(num)));

        state = State.End;
    }

    void GameEnd()
    {
        SelectTimer();

        if (!ButtonDown())
            return;

        state = State.Restart;

        foreach (var c in tefuda)
        {
            c.ChangeInit();
            c.Ura();

            ChangeCard(c);
        }

        SetSelect();
    }

    void Restart()
    {
        if (Reverse())
            ToSelect();
    }

    void SelectTimer()
    {
        selectTime += Time.deltaTime;

        float maxTime = 0.5f;

        if (selectTime >= maxTime)
            selectTime -= maxTime;

        select.color = selectTime < maxTime / 2f ? new Color(1f, 1f, 0f, 1f) : SelectColor();
    }

    void ChangeCard(GameCard card)
    {
        yamafuda.Add(card.card);

        card.card = yamafuda[0];

        yamafuda.RemoveAt(0);
    }

    void Shuffle()
    {
        for (int i = 0; i < yamafuda.Count; i++)
        {
            int num = Random.Range(0, yamafuda.Count);
            Card s = yamafuda[num];

            yamafuda[num] = yamafuda[i];
            yamafuda[i] = s;
        }
    }

    Yaku Pair(int[] num)
    {
        int p = 0;
        bool t = false;

        foreach (var n in num)
            if (n == 4)
                return Yaku.FourCard;
            else if (n == 3)
                t = true;
            else if (n == 2)
                p++;

        if (t)
            return p == 1 ? Yaku.FullHouse : Yaku.ThreeCard;

        if (p > 0)
            return p == 2 ? Yaku.TwoPair : Yaku.OnePair;

        return Yaku.NoPair;
    }

    Yaku StraightFlash(int[] num)
    {
        int c = 0;
        bool flash = IsFlash();

        if (IsRoyalStraight(num))
            return flash ? Yaku.RoyalStraightFlash : Yaku.Straight;

        foreach (var n in num)
            if (n == 1)
                c++;
            else if (c > 0 || n > 1)
                break;

        if (c == 5)
            return flash ? Yaku.StraightFlash : Yaku.Straight;

        return flash ? Yaku.Flash : Yaku.NoPair;
    }

    bool IsRoyalStraight(int[] num)
    {
        if (num[0] != 1)
            return false;

        for (int i = Card.Number.King - Card.Number.Ace; i > (int)Card.Number.King - 5; i--)
            if (num[i] != 1)
                return false;

        return true;
    }

    bool IsFlash()
    {
        Card.Egara e = tefuda[0].card.EGR;

        foreach (var c in tefuda)
            if (c.card.EGR != e)
                return false;

        return true;
    }

    bool Reverse()
    {
        reverseTime += Time.deltaTime;

        if (reverseTime < 1f)
            return false;

        reverseTime = 0f;
        return true;
    }

    bool ButtonDown()
    {
        return Input.GetButtonDown("Jump");
    }

    Color SelectColor()
    {
        return select == nextPanel ? Color.white : select.GetComponentInParent<GameCard>().ChangeColor;
    }
}
