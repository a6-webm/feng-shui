using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        DirectoryInfo dir = new DirectoryInfo("Assets/Scenes");
        foreach (DirectoryInfo d in dir.GetDirectories()) {
            string scenePath = "";
            foreach (FileInfo f in d.GetFiles()) {
                if (f.Extension == ".unity") {
                    scenePath = "Assets/Scenes/" + d.Name + "/" + f.Name;
                }
            }
            if (scenePath == "") { continue; }
            ScrollView sv = root.Q<ScrollView>("LevelScroll");
            Button button = new Button();
            button.text = d.Name;
            button.clicked += () => { SceneManager.LoadScene(scenePath); };
            sv.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
