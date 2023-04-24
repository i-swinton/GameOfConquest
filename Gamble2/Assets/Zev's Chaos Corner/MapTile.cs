using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MapTile : MonoBehaviour
{
    public Player Player = null;
    private string Name;

    public int Units = 1;


    public MapSystem.BoardTile NodeRef;
    PolygonCollider2D Colid;
    public Vector3 Center = new Vector3(1,1,1);

    public Transform UIDisplays;
    public TextMeshPro NameDisplay;
    public TextMeshProUGUI UnitsDisplay;


    // Start is called before the first frame update
    public void GenTile(MapSystem.BoardTile nodeRef, string name)
    {
        NodeRef = nodeRef;
        Name = name;

        Colid = GetComponent<PolygonCollider2D>();

        Center = Colid.bounds.center;

        UIDisplays.position = Center + Vector3.back;
        NameDisplay.text = Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Center, 0.5f);
    }
}
