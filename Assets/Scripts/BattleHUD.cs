using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text healthTracker;
  
    public Slider hpSlider;
   
    public void SetHUD(UnitScript unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.stats.maxHP;
        hpSlider.value = unit.stats.currHP;        
        
        SetHP(unit.stats.currHP);
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
        healthTracker.text = hp + "/" + hpSlider.maxValue;
    }

    public IEnumerator FlashText(Text text, Color originalColor, Color newColor, int flashTimes)
    {
        for (int i = 0; i < flashTimes; i++)
        {
            text.color = newColor;
            yield return new WaitForSeconds(.1f);
            text.color = originalColor;
            yield return new WaitForSeconds(.1f);

        }
        text.color = originalColor;
    }
}
