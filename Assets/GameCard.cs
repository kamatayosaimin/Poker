using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameCard : MonoBehaviour
{
    private bool change = false, isReverse = false;
    public GameObject omote, ura;
    public Image changeMark;
    public Text num, egara;
    public Card card;

    public bool IsReverse
    {
        get
        {
            return isReverse;
        }
    }

    public bool Change
    {
        get
        {
            return change;
        }
    }

    public Color ChangeColor
    {
        get
        {
            return change ? Color.red : Color.white;
        }
    }

    // Use this for initialization
    void Start()
    {
        Ura();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select(bool isSelect)
    {
        changeMark.gameObject.SetActive(isSelect);
    }

    public void SetChange()
    {
        change = !change;
    }

    public void ChangeInit()
    {
        change = false;

        changeMark.color = ChangeColor;
    }

    public void Omote()
    {
        isReverse = false;

        omote.SetActive(true);
        ura.SetActive(false);

        num.text = card.NumText;

        egara.text = card.EGRText;
        egara.color = card.Color;
    }

    public void Ura()
    {
        isReverse = true;

        omote.SetActive(false);
        ura.SetActive(true);
    }
}
