using UnityEngine;

public static class Layers
{
    public static readonly int Ground = LayerMask.NameToLayer("Ground");

    public static int Mask(params int[] layers)
    {
        int result = 0;

        foreach(var layer in layers)
        {
            result |= 1 << layer;
        }

        return result;
    }
}
