using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Fruit_curdata
{
    public Fruit_data data; 
    public Fruit_curdata(Fruit_data op)
    {
        data = op;
    }
}
public class Controller : MonoBehaviour
{
    [Header("ˮ����������")]
    private Fruit_mode fruit_mode;
    private Dictionary<Fruittype, Fruit_data> dic_fruit = new Dictionary<Fruittype, Fruit_data>();
    private Image img_next; 
    [Header("��Ϸ����")]
    private List<Fruit_controller> list_cur = new List<Fruit_controller>();
    public float launchForce = 10f;        // ��������
    public float gravityStrength = 2f;     // ����ǿ��
    public float range_dis;
    private Vector2 launchDirection;private bool ariming;
    public Fruit_data GetFruitData(Fruittype type)
    {
        if (dic_fruit.ContainsKey(type))
            return dic_fruit[type];
        return default;
    }
    public bool Can_next(Fruittype type)
    {
        int op = (int)type; op++;
        if (op > dic_fruit.Count)
        {
            return false;
        }
        return true;
    }
    public Fruit_data Get_nexttype(Fruittype op)
    {
        int num = (int)op;
        if (num < dic_fruit.Count)
        {
            Fruittype type = (Fruittype)(num + 1);
            return GetFruitData(type);
        }
        return GetFruitData(op);
    }
    private GameObject watermelonPrefab;
    private GameObject gam_temp;
    private Vector2 startTouchPos;
    private Vector2 currentTouchPos;
    private bool isAiming = false;private Fruit_data cur_fruit;private Fruit_data next_fruit;
    private float gam_radios;
    [HideInInspector] public  float radios;
    [Header("���Ʋ���")]
    private GameObject gam_whitelin;
    private void Start()
    {
        gam_whitelin = GameObject.Find("whiteline");ariming = false;
        range_dis = transform.Find("range").GetComponent<SpriteRenderer>().bounds.extents.x;
        gam_whitelin.SetActive(false);
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        radios = Mathf.Min(bounds.extents.x, bounds.extents.y);
        img_next = GameObject.Find("Canvas/Top/Next/img").GetComponent<Image>();
        fruit_mode = Resources.Load<Fruit_mode>("data/Fruit_mode");
        foreach (Fruit_data temp in fruit_mode.list_fruit)
        {
            dic_fruit[temp.type] = temp;
        }
        list_cur = new List<Fruit_controller>();
        watermelonPrefab = Resources.Load<GameObject>("prefab/gameobject/Circle");
        gam_temp = Resources.Load<GameObject>("prefab/gameobject/temp");
        //trajectoryLine = GetComponent<LineRenderer>();
        gam_temp= Instantiate(gam_temp,transform .position ,Quaternion.identity);
        gam_temp.SetActive(false);
        gam_radios = gam_temp.GetComponent<SpriteRenderer>().bounds.extents.x;
        next_fruit = GetFruitData((Fruittype)Random.Range(1, 3));
        StartCoroutine(Create_fruit());
    }

    void Update()
    {
        if (Gravit.Instance.game_end) { Debug.Log("��Ϸ����"); return; };
        if(gam_temp.activeSelf)
        HandleTouchInput();
        else
        {
            Debug.Log("gamʧ��");
        }
        DrawTrajectory();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartAiming(touch.position);
                    break;

                case TouchPhase.Moved:
                    UpdateAiming(touch.position);
                    break;

                case TouchPhase.Ended:
                    LaunchWatermelon();
                    break;
            }
        }

        // ������루�����ã�
        //if (Input.GetMouseButtonDown(0))
        //{
        //    StartAiming(Input.mousePosition);
        //}
        //else if (Input.GetMouseButton(0))
        //{
        //    UpdateAiming(Input.mousePosition);
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    LaunchWatermelon();
        //}
    }
    private void OnMouseDown()
    {
        StartAiming(Input.mousePosition);
    }
    private void OnMouseDrag()
    {
        UpdateAiming(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        LaunchWatermelon();
    }
    void StartAiming(Vector2 screenPos)
    {
        isAiming = true;ariming = false;ariming = false;
        startTouchPos=currentTouchPos = Camera.main.ScreenToWorldPoint(screenPos);
        gam_whitelin.SetActive(true);
        Fruit_data op = GetFruitData((Fruittype)(Random.Range(0, dic_fruit.Count)));
    }

    void UpdateAiming(Vector2 screenPos)
    {
        if (!isAiming) return;
        ariming = true;Debug.Log("��ק��");
        currentTouchPos = Camera.main.ScreenToWorldPoint(screenPos);
        if (Vector2.Distance(startTouchPos, currentTouchPos) < 0.1f)
        {
            startTouchPos = transform.position;
        }
        launchDirection = (currentTouchPos - startTouchPos).normalized;
    }

    void LaunchWatermelon()
    {
        if (!isAiming) return;
        if(ariming==false)
        {
            startTouchPos = transform.position;
            currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            launchDirection = (currentTouchPos - startTouchPos).normalized;
            ariming = true;
        }
        // ���㷢�����ȣ����ڻ������룩
        float swipeDistance = Vector2.Distance(startTouchPos, currentTouchPos);
        float force = Mathf.Clamp(swipeDistance * 0.01f, 1f, 10f);
        gam_temp.SetActive(false);
        GameObject newMelon = Instantiate(watermelonPrefab, transform .position, Quaternion.identity);
        Rigidbody2D rb = newMelon.GetComponent<Rigidbody2D>();
        newMelon.GetComponent<Fruit_controller>().Initgam(new Fruit_curdata( cur_fruit));
        // ���ⷢ��
        rb.AddForce(launchDirection * force * launchForce, ForceMode2D.Impulse);
        Debug.Log((launchDirection).magnitude);
        Gravit.Instance.list_temp.Add(newMelon.GetComponent<Fruit_controller>());
        isAiming = false;
        gam_whitelin.SetActive(false);
        StartCoroutine(Create_fruit());
    }
    private IEnumerator Create_fruit()
    {
        yield return new WaitForSeconds(0.1f);
        cur_fruit = next_fruit;
        next_fruit = GetFruitData((Fruittype)Random.Range(1, 3));
        img_next.sprite = next_fruit.sprite;
        gam_temp.SetActive(true);
        gam_temp.transform.localScale = (1 + cur_fruit.add_size*0.3f)*new Vector3(0.05f,0.05f,0.05f);
        gam_temp.GetComponent<SpriteRenderer>().sprite = cur_fruit.sprite;
        gam_radios = gam_temp.GetComponent<SpriteRenderer>().bounds.extents.x;
    }
    void DrawTrajectory()
    {
        if (!isAiming) return;
        Vector2 dir = currentTouchPos - startTouchPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gam_whitelin.transform.eulerAngles=new Vector3(0, 0, angle);
    }
}
