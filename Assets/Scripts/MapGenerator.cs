using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private GameObject roomLine; //lineRenderer를 사용해서 방의 사이즈를 보여주기 위함
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tilemap ladderMap;
    [SerializeField] Tile roomTile; //방을 구성하는 타일
    [SerializeField] Tile wallTile; //방과 외부를 구분지어줄 벽 타일
    [SerializeField] Tile outTile; //방 외부의 타일
    [SerializeField] Tile startRoomTile; // 시작 방을 나타낼 타일
    [SerializeField] Tile bossRoomTile; // 보스 방을 나타낼 타일
    [SerializeField] Tile ladderTile; // 보스 방 중앙에 설치할 사다리 타일
    [SerializeField] GameObject player; // 플레이어 프리팹
    [SerializeField] private GameObject[] enemyPrefabs; // 적 프리팹 배열
    [SerializeField] private int minEnemiesPerRoom = 1; // 각 방의 최소 적 개수
    [SerializeField] private int maxEnemiesPerRoom = 3; // 각 방의 최대 적 개수
    [SerializeField] private GameObject[] bossPrefabs; // 보스 프리팹 배열


    private Node startRoom; // 시작 방을 저장할 변수
    private Node bossRoom; // 보스 방을 저장할 변수
    private List<Node> leafNodes; // 리프 노드를 저장할 리스트

    void Start()
    {
        leafNodes = new List<Node>();
        FillBackground();//신 로드 시 전부다 바깥타일로 덮음
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall(); //바깥과 방이 만나는 지점을 벽으로 칠해주는 함수

        // 시작 방과 보스 방 설정
        SetStartAndBossRoom();
    }

    void Divide(Node tree, int n)
    {
        if (n == maximumDepth)
        {
            leafNodes.Add(tree); // 리프 노드를 리스트에 추가
            return; //내가 원하는 높이에 도달하면 더 나눠주지 않는다.
        }

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //가로와 세로중 더 긴것을 구한후, 가로가 길다면 위 좌, 우로 세로가 더 길다면 위, 아래로 나눠주게 될 것이다.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        if (tree.nodeRect.width >= tree.nodeRect.height) //가로가 더 길었던 경우에는 좌 우로 나누게 될 것이며, 이 경우에는 세로 길이는 변하지 않는다.
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //왼쪽 노드에 대한 정보다 
            //위치는 좌측 하단 기준이므로 변하지 않으며, 가로 길이는 위에서 구한 랜덤값을 넣어준다.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //우측 노드에 대한 정보다 
            //위치는 좌측 하단에서 오른쪽으로 가로 길이만큼 이동한 위치이며, 가로 길이는 기존 가로길이에서 새로 구한 가로값을 뺀 나머지 부분이 된다. 
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
        }
        tree.leftNode.parNode = tree; //자식노드들의 부모노드를 나누기전 노드로 설정
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //왼쪽, 오른쪽 자식 노드들도 나눠준다.
        Divide(tree.rightNode, n + 1);//왼쪽, 오른쪽 자식 노드들도 나눠준다.
    }

    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) // 해당 노드가 리프 노드라면 방을 만들어 준다
        {
            rect = tree.nodeRect;
            int padding = 2; // 방 사이의 최소 간격

            int width = Random.Range(rect.width / 2, rect.width - 1 - padding * 2);
            int height = Random.Range(rect.height / 2, rect.height - 1 - padding * 2);

            int x = rect.x + Random.Range(1 + padding, rect.width - width - padding);
            int y = rect.y + Random.Range(1 + padding, rect.height - height - padding);

            rect = new RectInt(x, y, width, height);
            tree.roomRect = rect; // 방의 Rect 정보를 저장
            FillRoom(rect);

            // 적 생성
            if (tree != startRoom && tree != bossRoom)
            {
                SpawnEnemies(rect);
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
        if (n == maximumDepth) //리프 노드라면 이을 자식이 없다.
            return;
        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
        {
            tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0), roomTile);
        }

        for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
        {
            tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
        }
        //이전 포스팅에서 선으로 만들었던 부분을 room tile로 채우는 과정
        GenerateLoad(tree.leftNode, n + 1); //자식 노드들도 탐색
        GenerateLoad(tree.rightNode, n + 1);
    }

    void FillBackground() //배경을 채우는 함수, 씬 load시 가장 먼저 해준다.
    {
        for (int i = -10; i < mapSize.x + 10; i++) //바깥타일은 맵 가장자리에 가도 어색하지 않게
        //맵 크기보다 넓게 채워준다.
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }

    void FillWall() //룸 타일과 바깥 타일이 만나는 부분
    {
        for (int i = 0; i < mapSize.x; i++) //타일 전체를 순회
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == outTile)
                {
                    //바깥타일 일 경우
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;//바깥 타일 기준 8방향을 탐색해서 room tile이 있다면 wall tile로 바꿔준다.
                            if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0)) == roomTile)
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FillRoom(RectInt rect)
    { //room의 rect정보를 받아서 tile을 set해주는 함수
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
            }
        }
    }

    private void SetStartAndBossRoom()
    {
        if (leafNodes.Count > 0)
        {
            startRoom = leafNodes[0];
            bossRoom = leafNodes[leafNodes.Count - 1];

            // 시작 방과 보스 방을 다른 타일로 표시
            FillSpecialRoom(startRoom.roomRect, TileType.StartRoom);
            FillSpecialRoom(bossRoom.roomRect, TileType.BossRoom);

            // 시작 방의 중앙에 플레이어를 이동
            Vector2Int playerPosition = startRoom.center;
            player.transform.position = new Vector3(playerPosition.x - mapSize.x / 2, playerPosition.y - mapSize.y / 2, 0);

            // 보스 방의 중앙에 사다리 타일 배치
            Vector2Int bossRoomCenter = bossRoom.center;
            ladderMap.SetTile(new Vector3Int(bossRoomCenter.x - mapSize.x / 2, bossRoomCenter.y - mapSize.y / 2, 0), ladderTile);
        }
    }



    private void FillSpecialRoom(RectInt rect, TileType tileType)
    {
        Tile specialTile = tileType == TileType.StartRoom ? startRoomTile : bossRoomTile; // 각각의 타일 타입에 맞는 타일을 설정
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), specialTile);
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
        // 현재 맵 지우기
        tileMap.ClearAllTiles();
        ladderMap.ClearAllTiles();
        leafNodes.Clear();

        // 새 맵 생성
        FillBackground();
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall(); // 벽 업데이트

        // 시작 방과 보스 방 설정
        SetStartAndBossRoom();
    }
    private void SpawnEnemies(RectInt roomRect)
    {
        int numberOfEnemies = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 적 프리팹을 랜덤으로 선택
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // 방의 중앙에 적을 생성
            Vector2Int spawnPosition = new Vector2Int(
                Random.Range(roomRect.x + 1, roomRect.x + roomRect.width - 1),
                Random.Range(roomRect.y + 1, roomRect.y + roomRect.height - 1)
            );

            // 적을 생성하고 위치를 설정
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0), Quaternion.identity);

            // 적을 방의 중앙에 위치시키거나 방의 지정된 위치로 배치
            enemy.transform.position = new Vector3(spawnPosition.x - mapSize.x / 2, spawnPosition.y - mapSize.y / 2, 0);
        }
    }


}

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; //분리된 공간의 rect정보
    public RectInt roomRect; //분리된 공간 속 방의 rect정보
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
        //방의 가운데 점. 방과 방을 이을 때 사용
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}