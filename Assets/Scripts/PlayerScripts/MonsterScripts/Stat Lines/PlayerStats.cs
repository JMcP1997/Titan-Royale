using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStatData", menuName = "PlayerData/PlayerStatData")]
public class PlayerStats : ScriptableObject
{
    public string monsterName;

    public int maxHealth;

    /* IF STAMINA HAS BEEN IMPLEMENTED, IF IMPLEMENTED, DONT FORGET TO ADD FIELD TO CONSTRUCTOR
    public int maxStamina;
    public int currentStamina;
    */
    public int currentEvolutionStage;
    //NOTE - THE FIRST EVOLUTION STAGE FOR EACH MONSTER SHOULD BE ENTERED AS STAGE 0 FOR CLARITY OF CODE

    public int evolutionCost;

    public int experienceOnKill;


}
