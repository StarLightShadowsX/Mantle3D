using System.Collections.Generic;
using UnityEngine;

public class GlobalAssets : GlobalAsset<GlobalAssets>
{
    public List<GlobalAssetGeneric> assets;
    [Gaskellgames.AssetsOnly] public List<GameObject> typedPrefabs;
    public List<(string name, GameObject obj)> namedPrefabs;

    public override void OnEnable()
    {
        base.OnEnable();
        for (int i = 0; i < assets.Count && assets[i] != null; i++) assets[i].OnEnable();
        for (int i = 0; i < typedPrefabs.Count && typedPrefabs[i] != null; i++) IGlobalPrefab.RegisterPrefab(typedPrefabs[i]);
        for (int i = 0; i < namedPrefabs.Count; i++) IGlobalPrefab.RegisterPrefab(namedPrefabs[i].obj, namedPrefabs[i].name);
    }
}
