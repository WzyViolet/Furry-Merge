using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System. Serializable]
public class Fruit_curdata
{
    public Fruit_data data; public int cur_lv;
    public Fruit_curdata(Fruit_data op, int lv)
    {
        data = op; cur_lv = lv;
    }
}
public class Controller : MonoBehaviour
{
    [Header("水果数据配置")]
    public Fruit_mode fruit_mode;
    private Dictionary<Fruittype, Fruit_data> dic_fruit = new Dictionary<Fruittype, Fruit_data>();
    private Image img_next;private Fruit_curdata next_fruit;
    [Header("游戏参数")]
    private List<Fruit_controller> list_cur = new List<Fruit_controller>();
    public float launchForce = 10f;        // 发射力度
    public float outerCircleRadius = 5f;   // 外层圆半径
    public float gravityStrength = 2f;     // 重力强度

    private Vector2 launchDirection;
    public Fruit_data GetFruitData(Fruittype type)
    {
        if (dic_fruit.ContainsKey(type))
            return dic_fruit[type];
        return default;
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
    [SerializeField] private GameObject watermelonPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private LineRenderer trajectoryLine;

    private Vector2 startTouchPos;
    private Vector2 currentTouchPos;
    private bool isAiming = false;
    private void Start()
    {
        img_next = GameObject.Find("Canvas/Top/Next/img").GetComponent<Image>();
        fruit_mode = Resources.Load<Fruit_mode>("data/Fruit_mode");
        launchPoint = transform.Find("center").transform;
        foreach (Fruit_data temp in fruit_mode.list_fruit)
        {
            dic_fruit[temp.type] = temp;
        }
        list_cur = new List<Fruit_controller>();
        watermelonPrefab = Resources.Load<GameObject>("Prefab/Circle");
        trajectoryLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        HandleTouchInput();
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

        // 鼠标输入（测试用）
        if (Input.GetMouseButtonDown(0))
        {
            StartAiming(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateAiming(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            LaunchWatermelon();
        }
    }

    void StartAiming(Vector2 screenPos)
    {
        isAiming = true;
        startTouchPos = screenPos;
        trajectoryLine.enabled = true;
        Fruit_data op = GetFruitData((Fruittype)(Random.Range(0, dic_fruit.Count)));
        next_fruit = new Fruit_curdata(op, Random.Range(0, 3));
        Debug.Log("等级为" + next_fruit.cur_lv);
        img_next.sprite = next_fruit.data.sprite;
    }

    void UpdateAiming(Vector2 screenPos)
    {
        if (!isAiming) return;

        currentTouchPos = screenPos;
        launchDirection = (startTouchPos - currentTouchPos).normalized;
    }

    void LaunchWatermelon()
    {
        if (!isAiming) return;

        // 计算发射力度（基于滑动距离）
        float swipeDistance = Vector2.Distance(startTouchPos, currentTouchPos);
        float force = Mathf.Clamp(swipeDistance * 0.01f, 1f, 10f);

        GameObject newMelon = Instantiate(watermelonPrefab, launchPoint.position, Quaternion.identity);
        Rigidbody2D rb = newMelon.GetComponent<Rigidbody2D>();
        newMelon.GetComponent<Fruit_controller>().Initgam(next_fruit);
        // 向外发射
        rb.AddForce(launchDirection * force * launchForce, ForceMode2D.Impulse);
        RadialGravity.Instance.list_fruit.Add(newMelon.GetComponent<Fruit_controller>());
        isAiming = false;
        trajectoryLine.enabled = false;
    }

    void DrawTrajectory()
    {
        if (!isAiming) return;

        Vector3[] points = new Vector3[10];
        Vector2 startPos = launchPoint.position;
        Vector2 velocity = launchDirection * launchForce;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            points[i] = startPos + velocity * time + 0.5f * Physics2D.gravity * time * time;
        }

        trajectoryLine.positionCount = points.Length;
        trajectoryLine.SetPositions(points);
    }
}
