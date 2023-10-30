using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePreFab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }


    [Header("Multiple Clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of Clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;


    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }
    #region Unlock region
    private void UnlockCloneAttack(){
        if(cloneAttackUnlockButton.unlocked){
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone(){
        if(aggresiveCloneUnlockButton.unlocked){
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultiClone(){
        if(multipleUnlockButton.unlocked){
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead(){
        if(crystalInsteadUnlockButton.unlocked){
            crystalInsteadOfClone = true;
        }
    }



    #endregion
    public void CreateClone(Transform _clonePostion, Vector3 _offset){

        if(crystalInsteadOfClone){
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePreFab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePostion,cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate,player,attackMultiplier);
    }


    public void CreateCloneWithDelay(Transform _enemyTransform){
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform transform,Vector3 offset) {
        yield return new WaitForSeconds(.4f);
        CreateClone(transform, offset);

    }

}
