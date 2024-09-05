using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; // ������ �������� �ּ� ����
    [SerializeField] float maximumDivideRate; // ������ �������� �ִ� ����
    [SerializeField] private GameObject line; // lineRenderer�� ����ؼ� ������ �������� �ð������� �����ֱ� ����
    [SerializeField] private GameObject map; // lineRenderer�� ����ؼ� ù ���� ����� �����ֱ� ����
    [SerializeField] private GameObject roomLine; // lineRenderer�� ����ؼ� ���� ����� �����ֱ� ����
    [SerializeField] private int maximumDepth; // Ʈ���� ����, �������� ���� �� �ڼ��� ������ ��
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tilemap ladderMap;
    [SerializeField] RuleTile ruleTile; // ���� �ٴ��� �ϳ��� �� Ÿ�Ϸ� ����
    [SerializeField] Tile outTile; // �� �ܺ��� Ÿ��
    [SerializeField] Tile ladderTile; // ���� �� �߾ӿ� ��ġ�� ��ٸ� Ÿ��
    [SerializeField] GameObject player; // �÷��̾� ������
    [SerializeField] private GameObject[] enemyPrefabs; // �� ������ �迭
    [SerializeField] private int minEnemiesPerRoom = 1; // �� ���� �ּ� �� ����
    [SerializeField] private int maxEnemiesPerRoom = 3; // �� ���� �ִ� �� ����
    [SerializeField] private GameObject[] bossPrefabs; // ���� ������ �迭
    [SerializeField] public int stage; // ��������

    private Node startRoom; // ���� ���� ������ ����
    private Node bossRoom; // ���� ���� ������ ����
    private List<Node> leafNodes; // ���� ��带 ������ ����Ʈ

    void Start()
    {
        leafNodes = new List<Node>();
        FillBackground(); // �� �ε� �� ���δ� �ٱ�Ÿ�Ϸ� ����
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        // FillWall() ���� - RuleTile�� ���� �ٴ��� ó��

        // ���� ��� ���� �� ����
        SetStartAndBossRoom();
        stage = 0;
    }

    void Divide(Node tree, int n)
    {
        if (n == maximumDepth)
        {
            leafNodes.Add(tree); // ���� ��带 ����Ʈ�� �߰�
            return; // ���� ���ϴ� ���̿� �����ϸ� �� ������ �ʴ´�.
        }

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));

        if (tree.nodeRect.width >= tree.nodeRect.height)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
        }

        tree.leftNode.parNode = tree; // �ڽ� ������ �θ� ��带 ������ �� ���� ����
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); // ����, ������ �ڽ� ���鵵 �����ش�.
        Divide(tree.rightNode, n + 1);
    }

    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) // �ش� ��尡 ���� ����� ���� ����� �ش�
        {
            rect = tree.nodeRect;
            int padding = 2; // �� ������ �ּ� ����

            int width = Random.Range(rect.width / 2, rect.width - 1 - padding * 2);
            int height = Random.Range(rect.height / 2, rect.height - 1 - padding * 2);

            int x = rect.x + Random.Range(1 + padding, rect.width - width - padding);
            int y = rect.y + Random.Range(1 + padding, rect.height - height - padding);

            rect = new RectInt(x, y, width, height);
            tree.roomRect = rect; // ���� Rect ������ ����
            FillRoom(rect);

            // �� ����
            if (tree != leafNodes[0] && tree != leafNodes[leafNodes.Count - 1])
            {
                SpawnEnemies(rect);
            }else if (tree != leafNodes[0])
            {
                SpawnBoss(rect);
            } 
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }

    private void GenerateLoad(Node tree, int n)
    {
        if (n == maximumDepth) // ���� ����� ���� �ڽ��� ����.
            return;

        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        // X ������ 3ĭ ������ ���� ����
        for (int offset = -1; offset <= 1; offset++) // �߾��� �������� ���� 1ĭ, ������ 1ĭ �߰�
        {
            for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2 + offset, 0), ruleTile);
            }
        }

        // Y ������ 3ĭ ������ ���� ����
        for (int offset = -1; offset <= 1; offset++) // �߾��� �������� ���� 1ĭ, �Ʒ��� 1ĭ �߰�
        {
            for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
            {
                tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2 + offset, j - mapSize.y / 2, 0), ruleTile);
            }
        }

        GenerateLoad(tree.leftNode, n + 1); // �ڽ� ���鵵 Ž��
        GenerateLoad(tree.rightNode, n + 1);
    }


    void FillBackground() // ����� ä��� �Լ�, �� �ε�� ���� ���� ���ش�.
    {
        for (int i = -10; i < mapSize.x + 10; i++) // �ٱ� Ÿ���� �� �����ڸ��� ���� ������� �ʰ�
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }

    private void FillRoom(RectInt rect)
    { // room�� rect ������ �޾Ƽ� Ÿ���� set ���ִ� �Լ�
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), ruleTile);
            }
        }
    }

    private void SetStartAndBossRoom()
    {
        if (leafNodes.Count > 0)
        {
            startRoom = leafNodes[0];
            bossRoom = leafNodes[leafNodes.Count - 1];

            // ���� ��� ���� ���� �ٸ� Ÿ�Ϸ� ǥ��
            FillSpecialRoom(startRoom.roomRect, TileType.StartRoom);
            FillSpecialRoom(bossRoom.roomRect, TileType.BossRoom);

            // ���� ���� �߾ӿ� �÷��̾ �̵�
            Vector2Int playerPosition = startRoom.center;
            player.transform.position = new Vector3(playerPosition.x - mapSize.x / 2, playerPosition.y - mapSize.y / 2, 0);
        }
    }

    public void InstallLadder()
    {
        // ���� ���� �߾ӿ� ��ٸ� Ÿ�� ��ġ
        Vector2Int bossRoomCenter = bossRoom.center;
        ladderMap.SetTile(new Vector3Int(bossRoomCenter.x - mapSize.x / 2, bossRoomCenter.y - mapSize.y / 2, 0), ladderTile);
    }

    private void FillSpecialRoom(RectInt rect, TileType tileType)
    {
        // Ư�� ���� �����ϱ� ���� �ٸ� ���� �ʿ��ϴٸ� ruleTile�� ������ ��Ģ�� ���
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), ruleTile);
            }
        }
    }

    private enum TileType
    {
        StartRoom,
        BossRoom
    }

    public void RegenerateMap()
    {
        // ���� �� �����
        tileMap.ClearAllTiles();
        ladderMap.ClearAllTiles();
        leafNodes.Clear();

        // �� �� ����
        FillBackground();
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        // FillWall() ���� - RuleTile�� ���� �ٴ��� ó��

        // ���� ��� ���� �� ����
        SetStartAndBossRoom();
    }

    private void SpawnEnemies(RectInt roomRect)
    {
        int numberOfEnemies = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // �� �������� �������� ����
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // ���� �߾ӿ� ���� ����
            Vector2Int spawnPosition = new Vector2Int(
                Random.Range(roomRect.x + 1, roomRect.x + roomRect.width - 1),
                Random.Range(roomRect.y + 1, roomRect.y + roomRect.height - 1)
            );

            // ���� �����ϰ� ��ġ�� ����
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0), Quaternion.identity);

            // ���� ���� �߾ӿ� ��ġ��Ű�ų� ���� ������ ��ġ�� ��ġ
            enemy.transform.position = new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0);
        }
    }

    private void SpawnBoss(RectInt roomRect)
    {
        GameObject bossPrefab = bossPrefabs[0];
        Vector2Int spawnPosition = new Vector2Int(
            Random.Range(roomRect.x + 1, roomRect.x + roomRect.width - 1),
            Random.Range(roomRect.y + 1, roomRect.y + roomRect.height - 1)
        );
        GameObject boss = Instantiate(bossPrefab, new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0), Quaternion.identity);
        boss.transform.position = new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0);
    }
}

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; // �и��� ������ rect ����
    public RectInt roomRect; // �и��� ���� �� ���� rect ����
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}
