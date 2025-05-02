using UnityEngine;
using UnityEngine.EventSystems;

public interface IBoat {
    Vector3Int positonOnGrid { get; set; }
    int xCenter { get; set; }
    int zCenter { get; set; }
    int[,] componetsGrid { get; set; }
    GameObject gameObject { get; }
    float rotation { get; set; }
    int points { get; set; }

    void RemoveBoatFromGrid();

    void ShowInvalidPosition();
    void HideInvalidPosition();

    void DestroyBoat();
}
