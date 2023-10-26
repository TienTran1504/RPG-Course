using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePreFab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;
    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;

    [Header("Clone Can Duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of Clone")]
    public bool crystalInsteadOfClone;
    public void CreateClone(Transform _clonePostion, Vector3 _offset){

        if(crystalInsteadOfClone){
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePreFab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePostion,cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate);
    }

    public void CreateCloneOnDashStart() {
        if(createCloneOnDashStart){
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnDashOver(){
        if (createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }
    public void CreateCloneOnCounterAttack(Transform _enemyTransform){
        if (canCreateCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform transform,Vector3 offset) {
        yield return new WaitForSeconds(.4f);
        CreateClone(transform, offset);

    }

}
