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
        heightMap.ClearUpdateZones(); // "Update"Passに設定
        UpdateZones();                // Passの更新
        heightMap.Update(count);      // ハイトマップの更新
    }

    private void UpdateZones()
    {
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);

        if (!leftClick && !rightClick)
        {
            return;
        }

        RaycastHit hitInfo;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            // "Update"Passのテクスチャ設定
            var defaultZone = new CustomRenderTextureUpdateZone();
            defaultZone.needSwap = true;                          // ダブルバッファの更新を要求
            defaultZone.passIndex = 0;                            // "Update"Passのインデックス
            defaultZone.rotation = 0;                             // テクスチャの回転
            defaultZone.updateZoneCenter = new Vector2(.5f, .5f); // テクスチャの中心
            defaultZone.updateZoneSize = new Vector2(1, 1);       // テクスチャのサイズ

            // "Click"Passのテクスチャ設定
            var clickZone = new CustomRenderTextureUpdateZone();
            clickZone.needSwap = true;                                                                    // ダブルバッファの更新を要求
            clickZone.passIndex = leftClick ? 1 : 2;                                                      // "Click"Passのインデックス
            clickZone.rotation = 0;                                                                       // テクスチャの回転
            clickZone.updateZoneCenter = new Vector2(hitInfo.textureCoord.x, 1 - hitInfo.textureCoord.y); // テクスチャの中心
            clickZone.updateZoneSize = new Vector2(.001f, .001f);                                         // テクスチャのサイズ

            // ハイトマップのパス全体の更新
            heightMap.SetUpdateZones(new CustomRenderTextureUpdateZone[] { defaultZone, clickZone });
        }
    }
}
