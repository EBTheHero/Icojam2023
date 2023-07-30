using UnityEngine;

public class CurrentTurnUI : MonoBehaviour
{
	public GameObject ArrowPreparing;
	public GameObject ArrowAttack;
	public GameObject ArrowEnemyAttack;
	public GameObject ArrowEnemyPrepare;

	public TMPro.TMP_Text EnemyAttackText;

	public static CurrentTurnUI Instance;

	private void Start()
	{
		Instance = this;
		HideAll();
	}

	private void Update()
	{
		if (EnemyAI.Instance.AttackingCell != null)
			EnemyAttackText.fontStyle = TMPro.FontStyles.Normal;
		else
			EnemyAttackText.fontStyle = TMPro.FontStyles.Strikethrough;
	}

	void HideAll()
	{
		ArrowPreparing.SetActive(false);
		ArrowAttack.SetActive(false);
		ArrowEnemyAttack.SetActive(false);
		ArrowEnemyPrepare.SetActive(false);
	}

	public void ShowPreparing()
	{
		HideAll();
		ArrowPreparing.SetActive(true);
	}

	public void ShowAttack()
	{
		HideAll();
		ArrowAttack.SetActive(true);
	}

	public void ShowEnemyAttack()
	{
		HideAll();
		ArrowEnemyAttack.SetActive(true);
	}

	public void ShowEnemyPrepare()
	{
		HideAll();
		ArrowEnemyPrepare.SetActive(true);
	}
}
