using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using System.Collections;
public class Fruit_controller : MonoBehaviour
{
    [Header("西瓜属性")]                
    public float mergeRadius = 0.5f;
    private GameObject gam;
    private Rigidbody2D rb;
    private bool isMerging = false;public  bool shake;
    public  Fruit_curdata data;public Controller controller;
    private float radios;//半径
    [HideInInspector] public float timer;
    [HideInInspector] public bool can_test=false;
    public float Get_radios { get { return radios; } }
    [HideInInspector] public Vector2 dir;
    public  void Initgam(Fruit_curdata op,Fruit_curdata next=null)
    {
        gam = Resources.Load<GameObject>("prefab/gameobject/Circle");
        controller = FindAnyObjectByType<Controller>();
        rb = GetComponent<Rigidbody2D>(); data = op; float scale;
        if (next != null)
        {
            scale= 1 + next.data.add_size * 0.3f;
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * scale;
        }
        else
        {
            scale = 1 + op.data.add_size * 0.3f;
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * scale;
            GetComponent<SpriteRenderer>().sprite = op.data.sprite;
            Bounds bounds = GetComponent<SpriteRenderer>().bounds;
            radios = Mathf.Min(bounds.extents.x, bounds.extents.y);
        }
    }
    public void Do_scale(Fruit_curdata op,Fruit_curdata next)
    {
        Initgam(op, next);
        float scale = 1 + op.data.add_size * 0.3f;Vector3 temp= new Vector3(0.05f, 0.05f, 0.05f)*scale;
        transform.DOScale(temp, 0.05f);
        Vector2 dir = (transform.position - controller.transform.position).normalized;
        rb.AddForce(dir * 5); GetComponent<SpriteRenderer>().sprite = op.data.sprite;
        StartCoroutine(Wait_radius());
    }
    private IEnumerator Wait_radius()
    {
        yield return new WaitForSeconds(0.06f);
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        radios = Mathf.Min(bounds.extents.x, bounds.extents.y);

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;can_test = true;Debug.Log("开始碰撞");shake = true;
        rb.velocity = Vector2.zero;dir = (transform.position - controller.transform .position).normalized;
        Fruit_controller otherMelon = collision.gameObject.GetComponent<Fruit_controller>();
        Fruit_controller thisMelon = GetComponent<Fruit_controller>();
        if(thisMelon.transform .position .y>=otherMelon.transform .position.y)
        {
            return;
        }
        if (otherMelon != null && thisMelon != null)
        {
            TryMergeWatermelons(thisMelon, otherMelon);
        }
    }

    void TryMergeWatermelons(Fruit_controller melon1, Fruit_controller melon2)
    {
        if (melon1.data.data.type == melon2.data.data.type)
        {
            // 防止重复合并
            melon1.isMerging = true;melon1.GetComponent<Collider2D>().enabled = false;
            melon2.isMerging = true;melon2.GetComponent<Collider2D>().enabled = false;
            // 计算中间位置
            float dis1 = Vector3.Distance(melon1.transform.position, controller.gameObject.transform.position);
            float dis2 = Vector3.Distance(melon2.transform.position, controller.gameObject.transform.position);
            Vector3 mergePosition = dis1 > dis2 ? melon2.transform.position : melon1.transform.position;
            if (controller.Can_next(data.data.type))
            {
                Debug.Log("开始融合");
                Fruit_data temp_data = controller.Get_nexttype(data.data.type);
                Fruit_curdata temp = new Fruit_curdata(temp_data);
                SpawnMergedWatermelon(mergePosition, temp);
                Uimanager.Instance.Add_scores(data.data.score);
            }
            else
            {
                Uimanager.Instance.Add_scores(10);
            }
            // 销毁两个西瓜
            Destroy(melon1.gameObject);
            Destroy(melon2.gameObject);
        }
    }

    public void SpawnMergedWatermelon(Vector3 position, Fruit_curdata data)
    {
        Debug.Log("融合");
        GameObject newMelon = Instantiate(gam, position, Quaternion.identity);
        Fruit_controller watermelon = newMelon.GetComponent<Fruit_controller>();
        newMelon.GetComponent<Fruit_controller>().Do_scale(data,this.data);
        Gravit.Instance.list_fruit.Add(newMelon.GetComponent<Fruit_controller>());
        Vector2 dir = (transform.position - controller.transform.position).normalized;
        newMelon.GetComponent<Rigidbody2D>().AddForce(dir * 10, ForceMode2D.Impulse);
    }
}