using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public enum InputMap
{
    PLAYER_MOVE_FORWARD = 0,
    PLAYER_MOVE_BACK = 1,
    PLAYER_MOVE_LEFT = 2,
    PLAYER_MOVE_RIGHT = 3,
    PLAYER_JUMP = 4,
    PLAYER_SPRINT = 5
}

public class InputMapCodes
{
    public static string[] INPUT_CODES {get; private set;} = { 
        "player_move_forward",  //0
        "player_move_back",     //1
        "player_move_left",     //2
        "player_move_right",    //3
        "player_jump",          //4
        "player_sprint"         //5
    };

    public static string GetCode(InputMap code)
    {
        return INPUT_CODES[(int)code];
    }
}