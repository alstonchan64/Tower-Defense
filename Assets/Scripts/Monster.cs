using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    void restoreHealth()
    {
        health = maxHealth;
    }

    [SerializeField] private int armor;
    [SerializeField] private int dodge;
    [SerializeField] private bool isCamo;
    [SerializeField] private float speed;
    [SerializeField] private int bounty;
    [SerializeField] private int lives_damage;

    [SerializeField] private int id;

    public int getID()
    {
        return id;
    }

    public bool getCamo()
    {
        return isCamo;
    }

    // private GameObject[] path;

    private IEnumerator<TileScript> path;
    public Point GridPosition { get; set; }

    [SerializeField] private Vector3 destination;

    public bool IsActive { get; set; }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (IsActive)
        {
    /**             
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (transform.position == destination)
            {
                Release();
            }
    */      
            if (transform.position == LevelManager.Instance.iglooPortal.transform.position)
            { 
                Release();
                PlayerStats.Fish -= lives_damage;  // decrement Fish count when polar bear makes it to the end depending on what type of bear got through
                if (PlayerStats.Fish <= 0)
                {
                    SceneManager.LoadScene(0);
                }
            }

            else if (transform.position == destination) 
            {
                if (path.MoveNext()) 
                {
                    destination = path.Current.WorldPosition;
                }
                else 
                {
                    destination = LevelManager.Instance.iglooPortal.transform.position;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        }
    }

    public void Spawn()
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1)));

        // destination = LevelManager.Instance.RedPortal.transform.position;

        path = LevelManager.Instance.Path();
        
        if (path.MoveNext())
        {
            destination = path.Current.WorldPosition;
        }

        restoreHealth();
        System.Random ran = new System.Random();
        id = ran.Next();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            // for now: projectile where disappear after collision
            GameManager.Instance.Pool.ReleaseObject(other.gameObject);
            health -= Math.Max(1, other.GetComponent<Projectile>().getDamage() - armor);

            // is enemy dead?
            if (health <= 0)
            {
                Release();
                WaveSpawner.Instance.killsNeeded --;
                PlayerStats.Fish += bounty;  // reward player for killing polar bear
            }
        }
    }

    public IEnumerator Scale(Vector3 from, Vector3 to)
    {
        IsActive = false;

        float progress = 0;

        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);

            progress += Time.deltaTime;

            yield return null;
        }

        transform.localScale = to;

        IsActive = true; 
    }

    private void Release()
    {
        IsActive = false;
        GridPosition = LevelManager.Instance.BlueSpawn;

        GameManager.Instance.Pool.ReleaseObject(gameObject);
    }
}
