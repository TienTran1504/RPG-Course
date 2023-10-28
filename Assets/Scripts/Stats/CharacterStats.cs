using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public EntityFX fx;

    [Header("Major stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power  1%
    public Stat agility;  // 1 increase evasion by 1% and crit.chance  1%
    public Stat intelligence; // 1 increase magic damage 1 and magic resistance 3
    public Stat vitality; // 1 increase health by 3-5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower; // default value 150%


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // make damge overtime
    public bool isChilled; // reduce armor by 20%
    public bool isShocked; // reduce accuracy by 20%

    [SerializeField] private float ailmentsDuaration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float ailmentTimer;


    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx= GetComponent<EntityFX>();

    }

    protected virtual void Update() {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;


        if(ignitedTimer <0){
            isIgnited = false;
        }
        if(chilledTimer <0){
            isChilled = false;
        }
        if(shockedTimer <0){
            isShocked = false;
        }
        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify){
        // start coroutine for stat increase
        StartCoroutine(StatModCoroutine(_modifier,_duration,_statToModify));
        
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify){
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }


    public virtual void DoDamage(CharacterStats _targetStats){

        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(CanCrit()){
            totalDamage = CalculateCriticalDamage(totalDamage);
        }


        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        //if inventory current weapon has fire(ice,lightning) effect then add line code below
        DoMagicalDamage(_targetStats);
    }
    
    #region MagicalDamage and ailments
   public virtual void DoMagicalDamage(CharacterStats _targetStats){
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);


        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;


        AttemptToApplyAliments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

   }

   private void AttemptToApplyAliments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage){
    bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while(!canApplyIgnite && !canApplyChill && !canApplyShock){
            if(Random.value < .3f && _fireDamage > 0){
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                // Debug.Log("Applied fire");
                return;
            }

            if(Random.value < .4f && _iceDamage > 0){
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                // Debug.Log("Applied ice");
                return;
            }

            if(Random.value < .5f && _lightingDamage > 0){
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                // Debug.Log("Applied electric");
                return;
            }
        }
        if(canApplyIgnite){
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }
        if(canApplyShock){
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
   }
   

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

   public void ApplyAilments(bool _ignite, bool _chill, bool _shock){
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;
        if(_ignite && canApplyIgnite){
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuaration;

            fx.IgniteFxFor(ailmentsDuaration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuaration;

            float slowPercentage = .25f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage,ailmentsDuaration);
            fx.ChillFxFor(ailmentsDuaration);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null) return;


                HitNearestTargetWithShockStrike();


            }
        }
   }
    public void ApplyShock(bool _shock)
    {
        if (isShocked) return;

        shockedTimer = ailmentsDuaration;
        isShocked = _shock;
        fx.ShockFxFor(ailmentsDuaration);
    }
    private void HitNearestTargetWithShockStrike()
    {
        //find closest target enemy
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }
        }


        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            // Debug.Log("Take burn damage " + igniteDamage);
            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    #endregion
    public virtual void TakeDamage(int _damage){
        DecreaseHealthBy(_damage);
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        if(currentHealth <= 0 && !isDead){
            Die();
        }
    }
    public virtual void IncreaseHealthBy(int _amount){
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue()){
            currentHealth = GetMaxHealthValue();
        }
        if(onHealthChanged !=null) {
            onHealthChanged();
        }
    }
    protected virtual void DecreaseHealthBy(int _damage){
        currentHealth -= _damage;
        if(onHealthChanged != null){
            onHealthChanged();
        }
    }
    protected virtual void Die() {
        isDead = true;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats){
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if(isShocked){
            totalEvasion += 20;
        }

        if(Random.Range(0,100) < totalEvasion){
            return true;
        }
        return false;
    }

    #region Stat calculations
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) {

        if(_targetStats.isChilled){
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else{
            totalDamage -= _targetStats.armor.GetValue();
        }
        totalDamage = Mathf.Clamp(totalDamage, 1, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 1, int.MaxValue);
        return totalMagicalDamage;
    }
    private bool CanCrit() {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if(Random.Range(0,100) <= totalCriticalChance){
            return true;
        }
        return false;
    }

    private int CalculateCriticalDamage(int _damage){
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion
}
