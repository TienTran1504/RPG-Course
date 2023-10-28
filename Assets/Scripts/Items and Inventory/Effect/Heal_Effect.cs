using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;
    public override void ExecuteEffect(Transform _respawnPosition)
    {
        //player stats
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        // how much to heal
        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
        
        Debug.Log("Heal Amount: " + healAmount);

        //heal
        playerStats.IncreaseHealthBy(healAmount);
    }
}
