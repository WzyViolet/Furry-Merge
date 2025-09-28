using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Uimanager : MonoBehaviour
{
    public static Uimanager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private Text cur_txt;
    private TextMeshProUGUI total_txt;
    private Image img_score;
    private int score;
    private List<string> list_str;
    // Start is called before the first frame update
    void Start()
    {
        cur_txt = transform.Find("Top/Score/txt").GetComponent<Text>();
        total_txt = transform.Find("Top/HighScore/txt").GetComponent<TextMeshProUGUI>();
        img_score = transform.Find("Top/Score/img").GetComponent<Image>();
        score = 0;Add_score(0);
        list_str = new List<string>()
        {
            "�ٽ�����!!!","����!!!"
        };
    }
    public void Add_score(int op)
    {
        score += op;
        if (score < 100)
        {
            img_score.color = Color.green;
        }
        else if(score<200)
        {
            img_score.color = Color.gray;
        }
        else
        {
            img_score.color = Color.red;
        }
        cur_txt.text = $"���ε÷�{op}";
        total_txt.text = score.ToString();
        StartCoroutine(Wait_score());
    }
    private IEnumerator Wait_score()
    {
        yield return new WaitForSeconds(2);
        string op = list_str[Random.Range(0, list_str.Count)];
        cur_txt.text = op;
        cur_txt.color = Random.Range(0, 10) > 5 ? Color.green : Color.red;
    }
}
