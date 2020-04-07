using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Teleport : MonoBehaviour
{
    private GameObject UIRootObject;

    public int scene;
    public int currentScene;
    private bool changingScenes;
    private bool loaded;

    private void Start()
    {
        loaded = false;
        changingScenes = false;
        //StartCoroutine(loadScene(scene));
    }

    private void OnTriggerEnter(Collider other)
    {
        basicLoad(scene, other);

        /*if (!changingScenes && loaded)
        {
            changingScenes = true;
            changeScene(scene, other.transform.parent.gameObject);
        }*/
    }

    void basicLoad(int index, Collider collider)
    {
        if (!changingScenes)
        {
            changingScenes = true;
            UIRootObject = collider.transform.parent.gameObject;

            Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(index);
            SceneManager.LoadScene(index);
            //SceneManager.MoveGameObjectToScene(UIRootObject, sceneToLoad);

            //GameObject.Destroy(GameObject.Find(sceneToLoad.name));
            //collider.gameObject.GetComponent<Player>().setRespawn(GameObject.Find("Map").transform.Find("Start").transform.Find("Respawn Point").transform.position);
        }
    }

    /*
    //scene changer stuff
    
    private AsyncOperation sceneAsync;

    public void changeScene(int index, GameObject obj)
    {
        UIRootObject = obj;
        OnFinishedLoadingAllScene(index);
    }

    
    IEnumerator loadScene(int index)
    {

        AsyncOperation scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        scene.allowSceneActivation = false;
        sceneAsync = scene;

        //Wait until we are done loading the scene
        while (!scene.isDone)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + scene.progress);
            yield return null;
        }
    }
    
    void enableScene(int index)
    {
        //Activate the Scene
        sceneAsync.allowSceneActivation = true;


        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(index);
        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is Valid");
            SceneManager.MoveGameObjectToScene(UIRootObject, sceneToLoad);
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }

    void OnFinishedLoadingAllScene(int index)
    {
        Debug.Log("Done Loading Scene");
        enableScene(index);
        //GameObject.Destroy(GameObject.Find(SceneManager.GetSceneByBuildIndex(index).name));
        UIRootObject.transform.position = Vector3.zero;
        //UIRootObject.transform.rotation = Quaternion.zero;
        Debug.Log("Scene Activated!");
    }
    */
}
