using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExecuteEffect(Transform _respawnPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _respawnPosition.position,Quaternion.identity);

        Destroy(newThunderStrike, 1);
        // Setup new thunder strike
    }
}
