using System.Collections.Generic;
using UnityEngine;

public class GlobalAssets : GlobalAsset<GlobalAssets>
{
    public List<GlobalAssetGeneric> assets;
    [Gaskellgames.AssetsOnly] public List<GameObject> prefabs;

    public override void OnEnable()
    {
        base.OnEnable();
        for (int i = 0; i < assets.Count && assets[i] != null; i++) assets[i].OnEnable();
        for (int i = 0; i < prefabs.Count && prefabs[i] != null; i++) IGlobalPrefab.RegisterPrefab(prefabs[i]);
    }
}
