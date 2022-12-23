namespace ThrowExceptions;
using ColossalFramework;
using KianCommons;
using KianCommons.IImplict;

internal class GO :
    Singleton<GO>,
    IAwakingObject, IStartingObject, IDestroyableObject,
    IEnablablingObject, IDisablablingObject,
    IGUIObject, IUpdatableObject, ILateUpdatableObject {
    public void Awake() {
        //throw new System.NotImplementedException();
    }

    public void Start() {
        //throw new System.NotImplementedException();
    }

    public void OnDestroy() {
        //throw new System.NotImplementedException();
    }
    public void OnEnable() {
        //throw new System.NotImplementedException();
    }

    public void OnDisable() {
        //throw new System.NotImplementedException();
    }

    public void OnGUI() {
        //throw new System.NotImplementedException();
    }

    public void Update() {
        Log.Called();
        throw new System.NotImplementedException();
    }

    public void LateUpdate() {
        //throw new System.NotImplementedException();
    }
}
