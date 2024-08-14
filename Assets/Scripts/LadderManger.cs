using UnityEngine;
using UnityEngine.Tilemaps;

public class LadderManger : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] MapGenerator mapGenerator;

    private void Start()
    {
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ��ü�� �÷��̾����� Ȯ���մϴ�
        if (collision.gameObject.CompareTag("Player"))
        {
            // �� ����� �޼��� ȣ��
            mapGenerator.RegenerateMap();
        }
    }
}

