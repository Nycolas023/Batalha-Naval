using UnityEngine;

[CreateAssetMenu]
public class PlayerModelSO : ScriptableObject {
    [SerializeField] private PlayerModel _value;
    
    public PlayerModel Value {
        get { return _value; }
        set { _value = value; }
    }
}
