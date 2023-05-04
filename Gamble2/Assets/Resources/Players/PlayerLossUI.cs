using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLossUI : UIElement
{
    Player target;

    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] TMPro.TextMeshProUGUI nameText;

    [SerializeField] Image playerImage;

    static PlayerLossUI instance;

    float duration;

    bool isActive;

    public static void PlayPlayerLoss(Player player, float dur)
    {
        // Mark player as dead
        player.isAlive = false;

        instance.target = player;

        instance.duration = dur;

        instance.isActive = true;

        instance.IsVisible = true;

        instance.nameText.text = player.Name;

        instance.playerImage.sprite = player.playerIcon;
    }

    public static bool IsActive
    {
        get
        {
            return instance.isActive;
        }

    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsVisible)
        {
            duration -= Time.deltaTime;

            if(duration <= 0 || Input.GetMouseButtonDown(0))
            {
                IsVisible = false;

                if(!GameMaster.GetInstance().IsNetworked || (ClientPlayerController.IsCurrentPlayer(GameMaster.GetInstance())) )
                {
                    GameMaster.GetInstance().PlayWinSequence();
                }
            }
        }
        else
        {

        }
    }
}
