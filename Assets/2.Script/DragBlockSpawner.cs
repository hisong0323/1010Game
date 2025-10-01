using System.Collections;
using UnityEngine;

public class DragBlockSpawner : MonoBehaviour
{
    [SerializeField] private BlockArrangeSystem blockArrangeSystem; // DragBlock 클래스에서 사용
    [SerializeField] private Transform[] blockSpawnPoints; // 드래그 가능한 블록에 배치되는 위치
    [SerializeField] private GameObject[] blockPrefabs; // 생성 가능한 모든 블록 프리팹

    // 외부에서 드래그 블록의 부모 Transform[] 배열 정보 열람
    public Transform[] BlockSpawPoints => blockSpawnPoints;

    private void Start()
    {
        StageController.Instance.Revive += OnRevive;

    }
    public void SpawnBlocks()
    {
        for (int i = 0; i < blockSpawnPoints.Length; ++i)
        {
            int index = Random.Range(0, blockPrefabs.Length);

            // 드래그 블록 생성(원본 프리팹, 생성 위치, 초기 회전값, 부모 Transform)
            GameObject clone = Instantiate(blockPrefabs[index], blockSpawnPoints[i].position, Quaternion.identity, blockSpawnPoints[i]);
            clone.GetComponent<DragBlock>().Setup(blockArrangeSystem);
        }
    }

    private void OnRevive()
    {
        for (int i = 0; i < blockSpawnPoints.Length; i++)
        {
            if (blockSpawnPoints[i].childCount != 0)
            {
                Destroy(blockSpawnPoints[i].GetChild(0).gameObject);
            }
        }
        for (int i = 0; i < blockSpawnPoints.Length; ++i)
        {
            GameObject clone = Instantiate(blockPrefabs[0], blockSpawnPoints[i].position, Quaternion.identity, blockSpawnPoints[i]);
            clone.GetComponent<DragBlock>().Setup(blockArrangeSystem);
        }
    }
}
