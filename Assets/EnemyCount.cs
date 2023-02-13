using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCount : MonoBehaviour
{
    public int enemyCnt = 3;
    public TextMeshProUGUI enemyText;
    // Start is called before the first frame update
    public void EnemyDown()
    {
        enemyCnt--;
        ShowEnemy();
    }

    // Update is called once per frame
    public void ShowEnemy()
    {
        if(enemyCnt < 1)
        {
            enemyText.text = "Enemies Defeated!";
        }
        else
        {
            enemyText.text = "Enemies: " + enemyCnt.ToString();
        }
        
    }
}
