using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace World.Editor
{
    public class RoomBrowserWindow : EditorWindow
    {
        [MenuItem("File/Room Browser")]
        private static void ShowWindow()
        {
            var wnd = GetWindow<RoomBrowserWindow>();
            wnd.titleContent = new GUIContent("MyEditorWindow");
        }

        private void CreateGUI()
        {


            List<RoomAsset> roomAssets = new();


        }
    }
}
