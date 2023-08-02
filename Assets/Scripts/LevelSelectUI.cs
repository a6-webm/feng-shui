using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        DirectoryInfo dir = new DirectoryInfo("Assets/Scenes");
        foreach (DirectoryInfo d in dir.GetDirectories()) {
            ScrollView sv = root.Q<ScrollView>("LevelScroll");
            Button button = new Button();
            button.text = d.Name;
            sv.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
