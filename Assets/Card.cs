using UnityEngine;
using System.Collections;

public class Card
{

    public enum Egara
    {
        Spade, Club, Heart, Diamond
    }

    public enum Number
    {
        Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }

    private Egara egara;
    private Number num;

    public Egara EGR
    {
        get
        {
            return egara;
        }
    }

    public Number Num
    {
        get
        {
            return num;
        }
    }

    public Color Color
    {
        get
        {
            return egara >= Egara.Heart ? Color.red : Color.black;
        }
    }

    public string EGRText
    {
        get
        {
            return egara.ToString()[0].ToString();
        }
    }

    public string NumText
    {
        get
        {
            return num >= Number.Two && num <= Number.Ten ? ((int)num).ToString() : num.ToString()[0].ToString();
        }
    }

    public Card(Egara egara, Number num)
    {
        this.egara = egara;
        this.num = num;
    }

    public override string ToString()
    {
        return egara + " " + num;
    }
}
