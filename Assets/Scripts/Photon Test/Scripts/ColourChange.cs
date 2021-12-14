using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourChange : MonoBehaviour
{

    private PhotonView pv;
    public Color[] colors;
    public Image image;
    
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    

    public void ChangeColour()
    {
        int colorIndex = Random.Range(0, colors.Length);
        pv.RPC("RPC_ChangeColour", RpcTarget.All, colorIndex);    
    }

    [PunRPC]
    public void RPC_ChangeColour(int colorindex)
    {
        image.color = colors[colorindex];
    }
}
