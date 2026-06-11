using System.Collections.Generic;
using SLS.ListUtilities;
using SLS.Singletons;

[UnityEngine.DefaultExecutionOrder(-160)]
public class GlobalSceneRegistry : GlobalAsset<GlobalSceneRegistry>
{
    public DictionaryS<string, SceneReference> NamedScenes = new();

    public override void OnInit()
    {
        for (int i = 0; i < NamedScenes.Count; i++)
        {
            IGlobalScene.RegisterScene(NamedScenes.ValueFromIndex(i), NamedScenes.KeyFromIndex(i));
        }
    }
}
