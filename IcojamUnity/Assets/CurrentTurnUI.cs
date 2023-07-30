using UnityEngine;
using UnityEngine.UI;

public class CurrentTurnUI : MonoBehaviour
{
	public GameObject ArrowPreparing;
	public GameObject ArrowAttack;
	public GameObject ArrowEnemyAttack;
	public GameObject ArrowEnemyPrepare;
	public Image ImagePreparing;
	public Image ImageAttack;
	public Image ImageEnemyAttack;
	public Image ImageEnemyPrepare;

	public Color hiddenColor = new Color(0x6d, 0x6d, 0x6d);

	public TMPro.TMP_Text EnemyAttackText;

	public static CurrentTurnUI Instance;

	private void Start()
	{
		Instance = this;
		HideAll();
		ShowPreparing();
	}

	private void Update()
	{
		if (EnemyAI.Instance.AttackingCell != null) {
			EnemyAttackText.fontStyle = TMPro.FontStyles.Bold;
		}
		else
			EnemyAttackText.fontStyle = TMPro.FontStyles.Strikethrough;
	}

	void HideAll()
	{
		ArrowPreparing.SetActive(false);
		ImagePreparing.color = hiddenColor;
		ArrowAttack.SetActive(false);
		ImageAttack.color = hiddenColor;
		ArrowEnemyAttack.SetActive(false);
		ImageEnemyAttack.color = hiddenColor;
		ArrowEnemyPrepare.SetActive(false);
		ImageEnemyPrepare.color = hiddenColor;
	}

	public void ShowPreparing()
	{
		HideAll();
		ArrowPreparing.SetActive(true);
		ImagePreparing.color = Color.white;
	}

	public void ShowAttack()
	{
		HideAll();
		ArrowAttack.SetActive(true);
		ImageAttack.color = Color.white;
	}

	public void ShowEnemyAttack()
	{
		HideAll();
		ArrowEnemyAttack.SetActive(true);
		ImageEnemyAttack.color = Color.white;
	}

	public void ShowEnemyPrepare()
	{
		HideAll();
		ArrowEnemyPrepare.SetActive(true);
		ImageEnemyPrepare.color = Color.white;
	}
}
