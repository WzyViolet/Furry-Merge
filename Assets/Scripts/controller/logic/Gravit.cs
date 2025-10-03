using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gravit : MonoBehaviour
{
    public static Gravit Instance;
    private void Awake()
    {
        Instance = this;
        list_fruit = new List<Fruit_controller>();
    }
    private GameObject gam_circle;private float dis_circle, dis_lose;
    public List<Fruit_controller> list_fruit;bool flash = false;
    public float outerBoundaryRadius;private TextMeshProUGUI lose;
    [HideInInspector]public bool game_end;
    private void Start()
    {
        lose = GameObject.Find("Canvas_/lose").GetComponent<TextMeshProUGUI>();
        lose.gameObject.SetActive(false);
        gam_circle = GameObject.Find("redline");
        gam_circle.SetActive(false);
        dis_lose = gam_circle.GetComponent<SpriteRenderer>().bounds.extents.x;
        dis_circle =dis_lose+ 0.5f;
        gam_circle.SetActive(false);
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        outerBoundaryRadius = Mathf.Min(bounds.extents.x, bounds.extents.y);
    }
    void Update()
    {
        if (list_fruit.Count > 0)
        {
            flash = false;
            foreach (Fruit_controller melon in list_fruit)
            {
                if (melon != null)
                {
                    if (melon.shake)
                    {
                        ApplyRadialGravity(melon);
                    }
                    CheckOuterBoundary(melon);
                }
            }
            if (flash)
            {
                gam_circle.SetActive(true);
            }
            else
            {
                gam_circle.SetActive(false);
            }
        }
    }

    void ApplyRadialGravity(Fruit_controller melon)
    {
        melon.timer = 0; melon.shake = false;
        Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();

        // 向外施加重力（离心力）
        rb.AddForce(melon.dir * 1000, ForceMode2D.Impulse);
    }

    void CheckOuterBoundary(Fruit_controller melon)
    {
        float distanceFromCenter = Vector2.Distance((Vector2)transform.position, melon.transform.position);
        if (melon.can_test&&( distanceFromCenter -melon.Get_radios < dis_circle))
        {
            if(melon.can_test)
            flash = true;
            if (distanceFromCenter - melon.Get_radios < dis_lose)
            {
                lose.gameObject.SetActive(true);
                Time.timeScale = 0; game_end = true;
            }
        }
        if (distanceFromCenter + melon.Get_radios > outerBoundaryRadius)
        {
            // 碰到外层圆，停止运动
            Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            Vector2 dir = (melon.transform.position - transform.position).normalized;
            // 确保西瓜不会超出边界
            Vector2 clampedPosition = dir * (outerBoundaryRadius - 0.001f) - dir * melon.Get_radios;
            melon.transform.position = clampedPosition;
        }
    }
}
