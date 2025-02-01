using UnityEngine;

//Toggles through Enemys in Help menu
public class EnemyToggleScript : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] int index;

    private void Start()
    {
        UpdateCard();
    }

    public void NextItem()
    {
        if (index < items.Length - 1)
            index++;
        else index = 0;
        UpdateCard();
    }

    public void PrevItem()
    {
        if (index > 0)
            index--;
        else index = items.Length - 1;
        UpdateCard();
    }

    void UpdateCard()
    {
        foreach (var item in items)
            item.SetActive(false);

        items[index].SetActive(true);
        SoundManager.FindFirstObjectByType<SoundManager>().PlaySound("Select");
    }
}