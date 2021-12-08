using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    #region Singleton
    public static Tooltip instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Tooltip found");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] RectTransform popupObject;
    [SerializeField] Text infoText;
    [SerializeField] Vector3 offset;
    [SerializeField] float padding;
    [SerializeField] Canvas canvas;

    private void Update()
    {
        FollowCursor();
    }

    private void FollowCursor()
    {
        if(!popupObject.gameObject.activeSelf) { return; }

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;

        //make sure it doesnt go off screen
        float rightEdgeToScreenDistance = Screen.width - (newPos.x + popupObject.rect.width * canvas.scaleFactor / 2) - padding;
        if(rightEdgeToScreenDistance <0)
        {
            newPos.x += rightEdgeToScreenDistance;
        }

        float leftEdgeToScreenDistance = 0 - (newPos.x + popupObject.rect.width * canvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenDistance > 0)
        {
            newPos.x += leftEdgeToScreenDistance;
        }

        float topEdgeToScreenDistance = Screen.height - (newPos.y + popupObject.rect.height * canvas.scaleFactor) - padding;
        if (topEdgeToScreenDistance < 0)
        {
            newPos.y += topEdgeToScreenDistance;
        }
        popupObject.transform.position = newPos;
    }

    public void DisplayActionInfo(Move move)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=20>").Append(move.moveName).Append("</size>").AppendLine();
        builder.Append(move.GetInfoText());

        infoText.text = builder.ToString();
        popupObject.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    public void HideInfo()
    {
        popupObject.gameObject.SetActive(false);
    }

    
}
