using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    Scene scene;

    // GameObjects
    public GameObject HPBar;
    public GameObject MPBar;
    public GameObject EXPBar;
    public TextMeshProUGUI HPNum;
    public TextMeshProUGUI MPNum;
    public TextMeshProUGUI LVLNum;
    public ParticleSystem LvlUpParticle;
    [SerializeField]
    private Animator LvlUpController;

    // HP/MP/EXP Animation Time
    public float duration = 3f;
    public float lvlUPDuration = 0.5f;
    private float startTimeHP = 0.0f;
    private float startTimeMP = 0.0f;
    private float startTimeEXP = 0.0f;
    private float startTimeT = 0.0f;
    // Starting / End Point
    private float scaleMinXHP = 0.0f;
    private float scaleMaxXHP = 1.0f;
    private float scaleMinXMP = 0.0f;
    private float scaleMaxXMP = 1.0f;
    private float scaleMinXEXP = 0.0f;
    private float scaleMaxXEXP = 1.0f;
    private float scaleLVLXEXP = 1.0f;
    private int excessEXP = 0;

    // Status
    public bool PlayerAlive = true;
    public bool LosingHP = false;
    public bool GainingHP = false;
    public bool UsingMP = false;
    public bool GainingMP = false;
    public bool GainingEXP = false;
    public bool LevelingUP = false;
    public bool LeveledUP = false;
    public bool LevelTransition = false;

    // EXP per levl
    public PlayerLevelEXP neededEXP;
    // Stat points per level up
    private int LevelUpStatPoints = 5;

    // Bars
    public int PlayerCurrentHP = 50;
    public int PlayerTotalHP = 50;
    public int PlayerCurrentMP = 50;
    public int PlayerTotalMP = 50;
    public int PlayerCurrentEXP = 0;
    public int PlayerTotalEXP = 50;

    // Passively gained stats
    private int PasHPGain = (int)(50 * 0.1f);
    private int PasMPGain = (int)(50 * 0.05f);
    private float PasHPStartTime = 0.0f;
    private float PasMPStartTime = 0.0f;
    private float PasHPTime = 5.0f;
    private float PasMPTime = 4.0f;

    // Randomized stats on level up
    private int MinHPGainOnLvlUp = 15;
    private int MaxHPGainOnLvlUp = 20;
    private int MinMPGainOnLvlUp = 9;
    private int MaxMPGainOnLvlUp = 11;

    // Stats
    public int PlayerLevel = 1;
    public int STR;
    public int DEX;
    public int INT;
    public int LUK;
    public int DEF;
    public int MDEF;
    public int ATT = 20;
    public int CRIT;
    public int StatPoints = 0;

    // Start is called before the first frame update
    void Start()
    {
        LVLNum.text = "Lv. " + PlayerLevel;
        HPNum.text = PlayerCurrentHP + " / " + PlayerTotalHP;
        MPNum.text = PlayerCurrentMP + " / " + PlayerTotalMP;
        PasHPStartTime = Time.time;
        PasMPStartTime = Time.time;
        scene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {  
        // Check if player is losing HP
        if (LosingHP == true)
        {
            // Calculate the fraction of the total duration that has passed
            float tHP = (Time.time - startTimeHP) / duration;
            // Animate the HP Bar
            float sHP = Mathf.SmoothStep(scaleMaxXHP, scaleMinXHP, tHP);
            HPBar.GetComponent<RectTransform>().localScale = new Vector3(sHP, 1, 1);
            // Check if the animation is done
            if (sHP == scaleMinXHP)
            {
                LosingHP = false;
            }
        }
        // Check if player is gaining HP
        if (GainingHP == true)
        {
            // Calculate the fraction of the total duration that has passed
            float tHP = (Time.time - startTimeHP) / duration;
            // Animate the HP Bar
            float sHP = Mathf.SmoothStep(scaleMinXHP, scaleMaxXHP, tHP);
            HPBar.GetComponent<RectTransform>().localScale = new Vector3(sHP, 1, 1);
            // Check if the animation is done
            if (sHP == scaleMaxXHP)
            {
                GainingHP = false;
            }
        }
        // Check if player is using MP
        if (UsingMP == true)
        {
            // Calculate the fraction of the total duration that has passed
            float tMP = (Time.time - startTimeMP) / duration;
            // Animate the MP Bar
            float sMP = Mathf.SmoothStep(scaleMaxXMP, scaleMinXMP, tMP);
            MPBar.GetComponent<RectTransform>().localScale = new Vector3(sMP, 1, 1);
            // Check if the animation is done
            if (sMP == scaleMinXMP)
            {
                UsingMP = false;
            }
        }
        // Check if player is gaining MP
        if (GainingMP == true)
        {
            // Calculate the fraction of the total duration that has passed
            float tMP = (Time.time - startTimeMP) / duration;
            // Animate the MP Bar
            float sMP = Mathf.SmoothStep(scaleMinXMP, scaleMaxXMP, tMP);
            MPBar.GetComponent<RectTransform>().localScale = new Vector3(sMP, 1, 1);
            // Check if the animation is done
            if (sMP == scaleMaxXMP)
            {
                GainingMP = false;
            }
        }
        // Check if player is gaining EXP
        if (GainingEXP == true)
        {
            // Animate the EXP Bar
            // Check if not leveling up
            if (LevelingUP == false)
            {
                // If not leveling up
                if (LeveledUP == false)
                {
                    // If just a normal EXP gain
                    if (LevelTransition == false)
                    {
                        // Calculate the fraction of the total duration that has passed
                        float tEXP = (Time.time - startTimeEXP) / duration;
                        // Animate the EXP Bar
                        float sEXP = Mathf.SmoothStep(scaleMinXEXP, scaleMaxXEXP, tEXP);
                        EXPBar.GetComponent<RectTransform>().localScale = new Vector3(sEXP, 1, 1);
                        // Check if the animation is done
                        if (sEXP == scaleMaxXEXP)
                        {
                            GainingEXP = false;
                        }
                    }
                    // Play the level up transition animation
                    else
                    {
                        // Calculate the fraction of the total duration that has passed
                        float tT = (Time.time - startTimeT) / lvlUPDuration;
                        // Animate the EXP Bar
                        float sT = Mathf.SmoothStep(1.0f, 0.0f, tT);
                        EXPBar.GetComponent<RectTransform>().localScale = new Vector3(sT, 1, 1);
                        // Check if the animation is done
                        if (sT == 0.0f)
                        {
                            LevelTransition = false;
                            LeveledUP = true;
                            startTimeEXP = Time.time;
                        }
                    }
                }
                // Once leveled up, do the third animation
                else
                {
                    // Calculate the duration of time needed for 0 to MAX
                    float dEXP = (scaleLVLXEXP - scaleMinXEXP) / (scaleLVLXEXP - scaleMinXEXP + scaleMaxXEXP);
                    // Calculate the fraction of the total duration that has passed
                    float tEXP = (Time.time - startTimeEXP) / (duration * (1 - dEXP));
                    // Make a second animation from 0 to MAX
                    float sEXP = Mathf.SmoothStep(0.0f, scaleMaxXEXP, tEXP);
                    EXPBar.GetComponent<RectTransform>().localScale = new Vector3(sEXP, 1, 1);
                    // Check if the animation is done
                    if (sEXP == scaleMaxXEXP)
                    {
                        LeveledUP = false;
                        GainingEXP = false;
                    }
                }
            }    
            // If leveling up
            else
            {
                // Calculate the duration of time needed for MIN to EXP
                float dEXP = (scaleLVLXEXP - scaleMinXEXP) / (scaleLVLXEXP - scaleMinXEXP + scaleMaxXEXP);
                // Calculate the fraction of the total duration that has passed
                float tEXP = (Time.time - startTimeEXP) / (duration * dEXP);
                // Make a first animation from MIN to LVLUP
                float sEXP = Mathf.SmoothStep(scaleMinXEXP, scaleLVLXEXP, tEXP);
                EXPBar.GetComponent<RectTransform>().localScale = new Vector3(sEXP, 1, 1);
                // Check if the animation is done
                if (sEXP == scaleLVLXEXP)
                {
                    LevelingUP = false;
                    LevelTransition = true;
                    startTimeT = Time.time;
                }
            }
        }
        // Passive HP Regen / MP Regen
        if (Time.time - PasHPStartTime > PasHPTime)
        {
            GainHP(PasHPGain);
            PasHPStartTime = Time.time;
        }
        if (Time.time - PasMPStartTime > PasMPTime)
        {
            GainMP(PasMPGain);
            PasMPStartTime = Time.time;
        }
        if (PlayerAlive == false)
        {
            SceneManager.LoadScene(4);
        }
            
    }

    // Lose Health
    public void LoseHP(int damage)
    {
        // Set the starting point of the HP animation
        //scaleMaxXHP = PlayerCurrentHP / (float)PlayerTotalHP;
        // This will find the current point where HP is at, in case the player was in the middle of gaining HP, it can be interuptted back to losing HP
        scaleMaxXHP = HPBar.GetComponent<RectTransform>().localScale.x;
        // Apply the damage
        PlayerCurrentHP -= damage;
        // Check if overkill
        if (PlayerCurrentHP <= 0)
        {
            PlayerCurrentHP = 0;
            PlayerAlive = false;
        }
        // Update HPNum
        HPNum.text = PlayerCurrentHP + " / " + PlayerTotalHP;
        // Set the ending point of the HP animation
        scaleMinXHP = PlayerCurrentHP / (float)PlayerTotalHP;
        // Set Player status to losing HP
        LosingHP = true;
        GainingHP = false;
        startTimeHP = Time.time;
    }

    // Gain Health
    public void GainHP(int health)
    {
        // Set the starting point of the HP animation
        scaleMinXHP = HPBar.GetComponent<RectTransform>().localScale.x;
        // Apply healing
        PlayerCurrentHP += health;
        // Check if overheal
        if (PlayerCurrentHP > PlayerTotalHP)
        {
            PlayerCurrentHP = PlayerTotalHP;
        }
        // Update HPNum
        HPNum.text = PlayerCurrentHP + " / " + PlayerTotalHP;
        // Set the ending point of the HP animation
        scaleMaxXHP = PlayerCurrentHP / (float)PlayerTotalHP;
        // Set Player status to gaining HP
        LosingHP = false;
        GainingHP = true;
        startTimeHP = Time.time;
    }

    public bool CheckMP(int mana)
    {
        // Check if the player has enough MP to use
        if (PlayerCurrentMP - mana >= 0)
        {
            // Return the skill can be used
            return true;
        }
        // Animate message saying not enough mana
        // Return the skill can not be used
        return false;
    }

    // Lose Mana, returns true if player has enough mana for skill
    public void LoseMP(int mana)
    {
            // Set the starting point of the MP animation
            scaleMaxXMP = MPBar.GetComponent<RectTransform>().localScale.x;
            // Apply the mana usage
            PlayerCurrentMP -= mana;
            // Update MPNum
            MPNum.text = PlayerCurrentMP + " / " + PlayerTotalMP;
            // Set the ending point of the MP animation
            scaleMinXMP = PlayerCurrentMP / (float)PlayerTotalMP;
            // Set Player status to losing MP
            UsingMP = true;
            GainingMP = false;
            startTimeMP = Time.time;
    }

    // Test function for LostMP since its a bool
    public void TestLoseMP(int mana)
    {
        // Check if the player has enough MP to use
        if (PlayerCurrentMP - mana >= 0)
        {
            // Set the starting point of the MP animation
            scaleMaxXMP = MPBar.GetComponent<RectTransform>().localScale.x;
            // Apply the mana usage
            PlayerCurrentMP -= mana;
            // Update MPNum
            MPNum.text = PlayerCurrentMP + " / " + PlayerTotalMP;
            // Set the ending point of the MP animation
            scaleMinXMP = PlayerCurrentMP / (float)PlayerTotalMP;
            // Set Player status to losing MP
            UsingMP = true;
            GainingMP = false;
            startTimeMP = Time.time;
            // Return the skill can be used
        }
        // Animate message saying not enough mana
        // Return the skill can not be used
    }

    // Gain Mana
    public void GainMP(int mana)
    {
        // Set the starting point of the MP animation
        scaleMinXMP = MPBar.GetComponent<RectTransform>().localScale.x;
        // Apply mana gain
        PlayerCurrentMP += mana;
        // Check if over-mana
        if (PlayerCurrentMP > PlayerTotalMP)
        {
            PlayerCurrentMP = PlayerTotalMP;
        }
        // Update MPNum
        MPNum.text = PlayerCurrentMP + " / " + PlayerTotalMP;
        // Set the ending point of the MP animation
        scaleMaxXMP = PlayerCurrentMP / (float)PlayerTotalMP;
        // Set Player status to gaining MP
        UsingMP = false;
        GainingMP = true;
        startTimeMP = Time.time;
    }

    // Gain EXP
    public void GainEXP(int experience)
    {
        // Set the starting point of the EXP animation
        scaleMinXEXP = EXPBar.GetComponent<RectTransform>().localScale.x;
        // Apply EXP gain
        PlayerCurrentEXP += experience;
        // Check if player leveled up
        if (PlayerCurrentEXP >= PlayerTotalEXP)
        {
            LevelingUP = true;
            LevelUp();
        }
        // If not leveling up
        else
        {
            // Set the ending point of the EXP animation
            scaleMaxXEXP = PlayerCurrentEXP / (float)PlayerTotalEXP; 
            LevelingUP = false;
        }
        // Set Player status to gaining EXP
        GainingEXP = true;
        startTimeEXP = Time.time;
    }

    // Level Up
    public void LevelUp()
    {
        // Increment Player's Level
        PlayerLevel++;
        LVLNum.text = "Lv. " + PlayerLevel;
        // animate level up on player's character

        // Calculate Player's new HP and MP and fill them up
        PlayerTotalHP += UnityEngine.Random.Range(MinHPGainOnLvlUp, MaxHPGainOnLvlUp + 1);
        // animate hp bar
        GainHP(PlayerTotalHP);
        
        PlayerTotalMP += UnityEngine.Random.Range(MinMPGainOnLvlUp, MaxMPGainOnLvlUp + 1);
        // animate mp bar
        GainMP(PlayerTotalMP);

        // Calculate Player's new EXP
        excessEXP = PlayerCurrentEXP - PlayerTotalEXP;
        PlayerTotalEXP = neededEXP.totalEXPNeeded[PlayerLevel];
        PlayerCurrentEXP = excessEXP;

        // Set the ending point of the second EXP animation
        scaleMaxXEXP = excessEXP / (float)PlayerTotalEXP;

        // Give Player stats to use
        StatPoints += LevelUpStatPoints;

        // Play Particle Effect
        LvlUpParticle.Play(true);
        // Play LvlUp Text 
        LvlUpController.SetTrigger("LvlUpText");

        // Update Passive Gain
        PasHPGain = (int)(PlayerTotalHP * 0.1f);
        PasMPGain = (int)(PlayerTotalMP * 0.2f);

        ATT += 2;

    // Give Player skill points?
    // Update PlayerUI?

}
}
