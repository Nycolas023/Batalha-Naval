using UnityEngine;
using UnityEngine.EventSystems;

public interface IBoat {
    Vector3Int positonOnGrid { get; set; }
    int xlength { get; set; }
    int zlength { get; set; }
    int xCenter { get; set; }
    int zCenter { get; set; }
    int[,] componetsGrid { get; set; }
    GameObject gameObject { get; }

    void RemoveBoatFromGrid();

    void ShowInvalidPosition();
    void HideInvalidPosition();
}
