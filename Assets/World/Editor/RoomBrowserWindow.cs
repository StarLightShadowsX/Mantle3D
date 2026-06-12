using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLS.EditorUtilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace World.Editor
{
    public class RoomBrowserWindow : EditorWindow
    {
        [MenuItem("File/Room Browser", priority = 0)]
        private static void ShowWindow()
        {
            var wnd = GetWindow<RoomBrowserWindow>();
            wnd.name = "Room Browser";
            wnd.titleContent = new GUIContent(wnd.name);
        }

        VisualElement CreateRow;
        TextField NewRoomNameInput;
        Button NewRoomFinal;
        VisualElement ListElement;

        private void CreateGUI()
        {
            CreateRow = new();
            CreateRow.style.flexDirection = FlexDirection.Row;
            NewRoomNameInput = new();
            NewRoomNameInput.style.flexGrow = 1f;
            NewRoomFinal = new(ClickAddButton);
            NewRoomFinal.style.width = 50;
            NewRoomFinal.text = "Create";
            ListElement = new();

            rootVisualElement.Add(CreateRow);
            CreateRow.Add(NewRoomNameInput);
            CreateRow.Add(NewRoomFinal);
            rootVisualElement.Add(ListElement);

            IReadOnlyList<RoomAsset> rooms = RoomRegistry.AllRooms;
            foreach (var item in rooms)
            {
                Label roomLabel = new();
                roomLabel.text = item.displayName;
                roomLabel.style.color = Color.steelBlue;
                new ElementHighlight(roomLabel, .2f).Hover();
                new ElementHighlight(roomLabel, Color.white).Click();
                roomLabel.RegisterCallback<MouseDownEvent>(e => item.OpenScene());
                ListElement.Add(roomLabel);
            }

        }

        public void ClickAddButton()
        {
            if (string.IsNullOrEmpty(NewRoomNameInput.value)) return;
            var newRoom = RoomAsset.Editor.CREATE(NewRoomNameInput.value);
            newRoom.OpenScene();
        }
    }
}
