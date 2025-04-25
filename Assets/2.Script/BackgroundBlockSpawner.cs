using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab; // ������� ��ġ�Ǵ� ������
    [SerializeField] private int orderInLayer; // ��ġ�Ǵ� ��ϵ��� �׸������� ����

    //private void Awake()
    public BackgroundBlock[] SpawnBlocks(Vector2Int blockCount, Vector2 blockHalf)
    {
        BackgroundBlock[] blocks = new BackgroundBlock[blockCount.x * blockCount.y];
        for (int y = 0; y < blockCount.y; ++y)
        {
            for (int x = 0; x < blockCount.x; ++x)
            {
                // ��� ���� �߾��� (0, 0, 0)�� �ǵ��� ��ġ
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;
                Vector3 position = new Vector3(px, py, 0);
                // ��� ���� (���� ������, ��ġ, ȸ��, �θ� Transfrom
                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                // ��� ������ ����� �׷����� ���� ����
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                // ������ ��� ����� ������ ��ȯ�ϱ� ���� block[] �迭�� ����
                blocks[y * blockCount.x + x] = clone.GetComponent<BackgroundBlock>();
            }
        }
        return blocks;
    }
}
