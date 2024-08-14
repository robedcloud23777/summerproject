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
        // 충돌한 객체가 플레이어인지 확인합니다
        if (collision.gameObject.CompareTag("Player"))
        {
            // 맵 재생성 메서드 호출
            mapGenerator.RegenerateMap();
        }
    }
}

