using UnityEngine;
using UnityEngine.Tilemaps;

public class LadderManger : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    private void Start()
    {
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            mapGenerator.stage++;
            mapGenerator.RegenerateMap();
        }
    }
}

