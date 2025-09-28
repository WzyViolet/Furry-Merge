using System.Collections.Generic;
using UnityEngine;

public class RadialGravity : MonoBehaviour
{
    public static RadialGravity Instance;
    private void Awake()
    {
        Instance = this;
        center = transform.Find("center").transform;
        list_fruit = new List<Fruit_controller>();
    }
    public List<Fruit_controller> list_fruit;
    public float gravity = 2f;
    public float outerBoundaryRadius = 2f;
    private Transform center;

    void FixedUpdate()
    {
        if (list_fruit.Count > 0)
        {
            foreach (Fruit_controller melon in list_fruit)
            {
                if(melon!=null)
                //ApplyRadialGravity(melon);
                CheckOuterBoundary(melon);
            }
        }
    }

    void ApplyRadialGravity(Fruit_controller melon)
    {
        Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();
        Vector2 directionToCenter = ((Vector2)center.position  - (Vector2)melon.transform.position).normalized;

        // ����ʩ����������������
        rb.AddForce(directionToCenter * -gravity * rb.mass);
    }

    void CheckOuterBoundary(Fruit_controller melon)
    {
        float distanceFromCenter = Vector2.Distance((Vector2)center.position , melon.transform.position);

        if (distanceFromCenter >= outerBoundaryRadius)
        {
            // �������Բ��ֹͣ�˶�
            Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            Vector2 dir =melon.transform.position-center.position ;
            // ȷ�����ϲ��ᳬ���߽�
            Vector2 clampedPosition = (dir).normalized * (outerBoundaryRadius - 0.01f);
            melon.transform.position = clampedPosition;
        }
    }
}