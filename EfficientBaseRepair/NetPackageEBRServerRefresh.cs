public class NetPackageEBRServerRefresh : NetPackage
{
    private Vector3i worldPos = default;

    public NetPackageEBRServerRefresh() { }

    public NetPackageEBRServerRefresh Setup(Vector3i worldPos)
    {
        this.worldPos = worldPos;
        return this;
    }

    public override void write(PooledBinaryWriter _bw)
    {
        base.write(_bw);

        _bw.Write(this.worldPos.x);
        _bw.Write(this.worldPos.y);
        _bw.Write(this.worldPos.z);
    }

    public override void read(PooledBinaryReader _br)
    {
        this.worldPos = new Vector3i(
            _br.ReadInt32(),
            _br.ReadInt32(),
            _br.ReadInt32()
        );
    }

    public override void ProcessPackage(World world, GameManager gameManager)
    {
        if (world == null) return;

        if (ConnectionManager.Instance.IsServer)
        {
            var tileEntity = world.GetTileEntity(worldPos) as TileEntityComposite;

            if (tileEntity.GetFeature<TEFeatureEBR>() is TEFeatureEBR ebrFeature)
            {
                ebrFeature.RefreshStats();
            }
        }
    }
}