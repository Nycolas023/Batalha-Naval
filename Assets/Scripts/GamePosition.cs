using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePosition : MonoBehaviour {
    public int x { get; private set; }
    public int y { get; private set; }

    private void OnMouseDown() {
        GamaManager.Instance.ChangeColor(this);
    }

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
