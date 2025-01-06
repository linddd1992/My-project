using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private AudioSource finishSound;
    private SpriteRenderer spriteRenderer;
    public Sprite normalSprite;  // 正常状态贴图
    public Sprite noneSprite;    // 碰撞nonemap时的贴图

    private bool levelCompleted = false;

    private void Start()
    {
        // finishSound = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        MapManager.Instance.OnStateChanged += HandleStateChange;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100;  // 设置较高的渲染层级
            if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            // finishSound.Play();
            levelCompleted = true;
            Invoke("CompleteLevel", 0.1f);
            GameManager.Instance.LevelComplete();
        }
    }

    private void HandleStateChange(MapState newState) {
        if (newState == MapState.Night) {
            // Night state - check nonemap
            if (IsInNonemap() && spriteRenderer != null && noneSprite != null) {
                spriteRenderer.sprite = noneSprite;
            }
            else if (spriteRenderer != null && normalSprite != null) {
                spriteRenderer.sprite = normalSprite;
            }
        }
        else if (newState == MapState.Normal) {
            // Normal state - check tilemap
            if (IsInTilemap() && spriteRenderer != null && noneSprite != null) {
                spriteRenderer.sprite = noneSprite;
            }
            else if (spriteRenderer != null && normalSprite != null) {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }

    private bool IsInNonemap() {
        if (MapManager.Instance == null || MapManager.Instance.nonemap == null) {
            return false;
        }
        Vector3Int cellPos = MapManager.Instance.nonemap.WorldToCell(transform.position);
        return MapManager.Instance.nonemap.HasTile(cellPos);
    }

    private bool IsInTilemap() {
        if (MapManager.Instance == null || MapManager.Instance.tilemap == null) {
            return false;
        }
        Vector3Int cellPos = MapManager.Instance.tilemap.WorldToCell(transform.position);
        return MapManager.Instance.tilemap.HasTile(cellPos);
    }

    private void CompleteLevel()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

}
