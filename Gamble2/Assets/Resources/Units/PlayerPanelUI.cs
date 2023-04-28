using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanelUI : UIElement
{
    [SerializeField] PlayerPanel playerPanelPrefab;

    List<PlayerPanel> panels;

    [SerializeField] Transform verticalList;

    // The static instance of the UI
    static PlayerPanelUI instance;

    // ----------------------------------------------------- Public Functions ----------------------------------------------

    private void Awake()
    {
        instance = this;
        panels = new List<PlayerPanel>();
    }

    public static PlayerPanel SpawnPlayerPanel(Player player)
    {
        // Add the panel to the list
        instance.panels.Add(Instantiate(instance.playerPanelPrefab, instance.verticalList));
        // Initialize the panel
        instance.panels[instance.panels.Count - 1].Setup(player);

        // Return the new panel
        return instance.panels[instance.panels.Count - 1];
    }
}
