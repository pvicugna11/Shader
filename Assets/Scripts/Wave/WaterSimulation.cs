using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulation : MonoBehaviour
{
    [SerializeField] CustomRenderTexture heightMap; // 波動シュミレーションの出力ハイトマップ
    [SerializeField] int count = 5;                 // 1フレームにつきcount回更新

    private void Start()
    {
        heightMap.Initialize(); // ハイトマップの初期化
    }

    private void Update()
    {
        heightMap.Update(count); // ハイトマップの更新
    }
}
