using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Controllers;
using Components;
using System.Threading.Tasks;
using UnityEditor;

public class InputControllerTest
{
    [Test]
    public void PathfindingSimplePasses()
    {
        // Use the Assert class to test conditions.
        Assert.IsTrue(true);
    }

    // [UnityTest]
    // public IEnumerator Set_Origin_And_Destination_Test()
    // {
    //     // Crea dos GameObjects para el origen y el destino
    //     GameObject origin = Object.Instantiate(Resources.Load<GameObject>("TileForLists"));
    //     GameObject destination = Object.Instantiate(Resources.Load<GameObject>("TileForLists"));

    //     GameObject tilesContainer = new("TilesForLists");

    //     origin.transform.parent = tilesContainer.transform;
    //     destination.transform.parent = tilesContainer.transform;

    //     // Evita que el destino tenga la misma posición que el origen
    //     destination.transform.position = new Vector3(1, 0, 0);

    //     // Asigna los GameObjects a los campos de origen y destino del InputController
    //     yield return Controllers.InputController.SetInput(origin);
    //     yield return Controllers.InputController.SetInput(destination);
        
    //     // Verifica que los GameObjects se hayan asignado correctamente
    //     Assert.IsTrue(Controllers.InputController.origin == origin);
    //     Assert.IsTrue(Controllers.InputController.destination == destination);
        
    //     yield return null;
    // }

    // [UnityTest]
    // public IEnumerator Set_Destination_When_Mouse_Enter_Test()
    // {
    //     // Crea dos GameObjects para el origen y el destino
    //     GameObject origin = new("Origin");
    //     GameObject destination = new("Destination");

    //     // Añadir el componente Tile a los GameObjects
    //     origin.AddComponent<Components.Tile>();
    //     destination.AddComponent<Components.Tile>();

    //     // Asigna el tag "Tile" a los GameObjects
    //     destination.transform.tag = "Tile";

    //     // Evita que el destino tenga la misma posición que el origen
    //     destination.transform.position = new Vector3(1, 0, 0);

    //     // Asigna los GameObjects a los campos de origen y destino del InputController
    //     yield return Controllers.InputController.SetInput(origin);
    //     Controllers.InputController.SetInputWhenMouseEnter(destination);
        
    //     // Verifica que los GameObjects se hayan asignado correctamente
    //     Assert.IsTrue(Controllers.InputController.origin == origin);
    //     Assert.IsTrue(Controllers.InputController.destination == destination);
        
    //     yield return null;
    // }
}
