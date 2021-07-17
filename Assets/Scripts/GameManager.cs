using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public TowerBtn ClickedBtn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickTower(TowerBtn towerBtn)
    {
        this.ClickedBtn = towerBtn;
        Hover.Instance.Activate(towerBtn.Sprite);
    }

    public void BuyTower()
    {
        ClickedBtn = null;
    }
}
