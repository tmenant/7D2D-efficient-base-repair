public class BFSUtils
{
    public static readonly Vector3i[] offsets = new Vector3i[]
    {
        new Vector3i(1, 0, 0),
        new Vector3i(-1, 0, 0),
        new Vector3i(0, 1, 0),
        new Vector3i(0, -1, 0),
        new Vector3i(0, 0, 1),
        new Vector3i(0, 0, -1),
        new Vector3i(0, 1, 1),
        new Vector3i(0, -1, 1),
        new Vector3i(0, 1, -1),
        new Vector3i(0, -1, -1),
        new Vector3i(1, 0, 1),
        new Vector3i(-1, 0, 1),
        new Vector3i(1, 0, -1),
        new Vector3i(-1, 0, -1),
        new Vector3i(1, 1, 0),
        new Vector3i(1, -1, 0),
        new Vector3i(-1, 1, 0),
        new Vector3i(-1, -1, 0),
        new Vector3i(1, 1, 1),
        new Vector3i(1, -1, 1),
        new Vector3i(1, 1, -1),
        new Vector3i(1, -1, -1),
        new Vector3i(-1, 1, 1),
        new Vector3i(-1, -1, 1),
        new Vector3i(-1, 1, -1),
        new Vector3i(-1, -1, -1)
    };
}