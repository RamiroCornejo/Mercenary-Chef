using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGenerator;
using DungeonGenerator.Components;
using System.Linq;

public class Dungeon : MonoBehaviour
{
    public NewRoomGenerator newManagerRooms;

    private void Awake()
    {

    }

    private void Start()
    {
        newManagerRooms.Generate(OnEndDungeonGeneration);
    }

    public void OnEndDungeonGeneration(List<NewRoom> rooms)
    {
        //busco el spawn point entre las rooms
        //pongo al player ahi
        //ajusto la camara
        //rooms[0].myRoomtrigger.IsInside(Character.instance.gameObject); //le digo a la primer room que el player está ahi
        RoomTriggerManager.instancia.Initialize(rooms);
        //ajuste de camara instantaneo

        var doors = rooms[0].GetDoors;
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < doors.Length; i++)
        {
            sum = new Vector3(doors[i].transform.position.x + sum.x, doors[i].transform.position.y + sum.y, doors[i].transform.position.z + sum.z);
        }
        var doors_prom = new Vector3(sum.x/doors.Length, sum.y / doors.Length, sum.z / doors.Length);

        Character.instance.gameObject.transform.position = doors_prom;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            newManagerRooms.Generate(OnEndDungeonGeneration);
        }
    }

    void OnFadeBackEnded()
    {
       
        
    }

    void OnFadeGoEnded()
    {
        
    }


    public  void TeleportBug()
    {
        Character.instance.transform.position = new Vector3(newManagerRooms.listrooms[0].transform.position.x, 0, newManagerRooms.listrooms[0].transform.position.z);
        newManagerRooms.listrooms[0].myRoomtrigger.IsInside(Character.instance.gameObject);
        //ajusto la camara
    }

    public void PlayerIsDead() { }
    public void PlayerIsAlive() { }
    public void OnPause() { }

    public void OnPlayerDeath()
    {
        newManagerRooms.listrooms.ForEach(x => x.PlayerIsDeath());
    }
}
