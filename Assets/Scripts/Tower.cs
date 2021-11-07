using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TowerType {BASIC, NOTSOBASIC}

public abstract class Tower : MonoBehaviour
{
    [SerializeField] private string projectileType;

    public SpriteRenderer spriteRenderer;

    private Monster target;
    public Monster Target { get { return target;  } }

    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;
    private float attackTimer;
    [SerializeField] private float attackCooldown;

    public TowerType TypeOfTower { get; protected set; }

    [SerializeField] private int damage;
    public float accuracy;
    private System.Random rand = new System.Random();
    [SerializeField] private bool canSeeCamo;
    [SerializeField] private int upgradeDamage;
    [SerializeField] private float upgradeAccuracy;
    public int price;

    [SerializeField] private float upgradeAttackCooldown;

    [SerializeField] private int upgradePrice;

    [SerializeField] private int upgradeMax = 3;

    private int upgradeCounter = 0;

    [SerializeField] private float projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed; } }

    // Start is called before the first frame update
    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void Upgrade(int path) {
      if (upgradeCounter < upgradeMax && PlayerStats.Fish >= upgradePrice) {
        if (path == 0) {
            damage += upgradeDamage;
            PlayerStats.Fish -= upgradePrice;
        } else if (path == 1) {
            attackCooldown -= upgradeAttackCooldown;
            PlayerStats.Fish -= upgradePrice;
        } else {
            if (canSeeCamo != true) {
                canSeeCamo = true;
                PlayerStats.Fish -= upgradePrice;            
            }
        }
        upgradeCounter += 1;          
      }
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }

        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                Shoot();
                canAttack = false;
            }
        }
    }

    private void Shoot()
    {
        // This is the current implementation of accuracy but we could change projectile class later
        float randomAccuracy = (float) rand.NextDouble();
        if (randomAccuracy > accuracy) {
          return;
        }

        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        projectile.transform.position = transform.position;

        projectile.Initialize(this, damage);
    }

    public void Select()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (!monster.getCamo() || (canSeeCamo)) 
            {
                monsters.Enqueue(monster);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            target = null;
        }
    }

    public class Stats {
        public int Damage { get; set; }
        public float AttackCooldown { get; set; }
        public bool CanSeeCamo { get; set; }
        public Stats(int dmg, float ac, bool csc) {
            Damage = dmg;
            AttackCooldown = ac;
            CanSeeCamo = csc;
        }
    }

    public Stats GetStats() {
        return new Stats(damage, attackCooldown, canSeeCamo);
    }
}
