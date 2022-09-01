using System.Collections;
using TMPro;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public Stat stat;
    protected PlayerStat playerStat;
    protected Animator animator;

    public bool isdamage;
    public bool isDeath = false;

    protected Rigidbody2D rigid;
    [SerializeField]
    protected GameObject damageCanvas;
    protected TextMeshProUGUI damageText;
    Coroutine coDamage;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        stat = GetComponent<Stat>();
        if(this.gameObject.name.Contains("Player"))
            playerStat = GetComponent<PlayerStat>();
        damageCanvas = Resources.Load<GameObject>("Prefabs/UI/DamageCanvas").gameObject;
    }

    public virtual void TakeDamage(GameObject obj, float damage, bool isCritical = false, bool isPersent = false, float trueDamage = 0, float counter = 0, float dash = 0)
    {
        if (isDeath == true)
            return;

        GameObject go = null;

        if (UIManager.Instance.damageTextUI.transform.childCount == 0)
            go = Instantiate(damageCanvas, transform.position, Quaternion.identity);
        else
        {
            go = UIManager.Instance.damageTextUI.transform.GetChild(0).gameObject;
            go.transform.SetParent(obj.transform.parent);
            go.transform.position = transform.position;
            go.SetActive(true);
        }

        go.GetComponent<Canvas>().sortingLayerName = "Tile";

        damageText = go.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damage.ToString();
        damageText.fontSize = 45;

        if (isCritical == true && trueDamage == 0 && counter == 0 && dash == 0)
        {
            damageText.colorGradient = new VertexGradient(new Color(1, 0.95f, 0), new Color(1, 0.95f, 0),
                new Color(1, 0.6f, 0), new Color(1, 0.6f, 0));
            go.GetComponent<DamageCanvasUp>().isCritical = true;
        }
        else if(isCritical == false && trueDamage != 0 && counter == 0 && dash == 0)
        {
            damageText.text = trueDamage.ToString();
            damageText.fontSize = 22;
            damageText.colorGradient = new VertexGradient(new Color(0, 0.38f, 0.03f), new Color(0, 0.38f, 0.03f),
                new Color(0, 0.38f, 0.03f), new Color(0, 0, 0));
            go.GetComponent<DamageCanvasUp>().isCritical = false;
        }
        else if(isCritical == false && trueDamage == 0 && counter != 0 && dash == 0)
        {
            damageText.text = counter.ToString();
            damageText.colorGradient = new VertexGradient(new Color(1, 0, 0), new Color(1, 1, 0),
                new Color(1, 1, 0), new Color(1, 0, 0));
            go.GetComponent<DamageCanvasUp>().isCritical = false;
        }
        else if(isCritical == false && trueDamage == 0 && counter == 0 && dash != 0)
        {
            damageText.text = dash.ToString();
            damageText.colorGradient = new VertexGradient(new Color(1, 1, 0), new Color(1, 1, 0),
                new Color(1, 1, 0), new Color(1, 1, 1));
            go.GetComponent<DamageCanvasUp>().isCritical = false;
        }
        else
        {
            damageText.colorGradient = new VertexGradient(new Color(0.99f, 0.61f, 0.61f), new Color(0.99f, 0.61f, 0.61f),
                new Color(1, 0, 0), new Color(1, 0, 0));
            go.GetComponent<DamageCanvasUp>().isCritical = false;
        }
        if (coDamage == null)
            coDamage = StartCoroutine(DamageText());
    }

    public IEnumerator DamageText()
    {
        yield return new WaitForSeconds(0.5f);
        coDamage = null;
    }

    protected virtual void Die(GameObject obj)
    {

    }
}
