﻿namespace Content.Client.Simulation;

public enum MovementType : byte
{
    Block = 0,
    Swap = 1,
    Pass = 2,
    PassUnder = 3,
    Custom = 4,
}