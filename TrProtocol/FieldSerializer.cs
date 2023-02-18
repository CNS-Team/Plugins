using System.Reflection;

namespace TrProtocol;

public abstract class NumericFieldSerializer<T> : FieldSerializer<T>, IConfigurable
{
    private int upper, lower;
    private T zero;
    private bool interrupt, enabled;
    public override void Write(BinaryWriter bw, object o)
    {
        if (this.enabled)
        {
            var o2 = Convert.ToInt32(o);
            if (o2 > this.upper || o2 < this.lower)
            {
                if (this.interrupt)
                {
                    throw new OutOfBoundsException(
                        $"Packet ignored due to field {typeof(T)} = {o2} out of bounds ({this.lower}, {this.upper})");
                }

                o = this.zero;
            }
        }
        this.WriteOverride(bw, (T) o);
    }

    public IConfigurable Configure(PropertyInfo prop, string version)
    {
        foreach (var bounds in prop.GetCustomAttributes<BoundsAttribute>())
        {
            if (bounds.Version != version)
            {
                continue;
            }

            this.zero = (T) Convert.ChangeType(0, prop.PropertyType);
            this.upper = bounds.UpperBound;
            this.lower = bounds.LowerBound;
            this.interrupt = bounds.Interrupt;
            this.enabled = true;
        }
        return this;
    }
}
public abstract class FieldSerializer<T> : IFieldSerializer
{
    protected abstract T ReadOverride(BinaryReader br);

    protected abstract void WriteOverride(BinaryWriter bw, T t);

    public virtual object Read(BinaryReader br)
    {
        return this.ReadOverride(br);
    }

    public virtual void Write(BinaryWriter bw, object o)
    {
        this.WriteOverride(bw, (T) o);
    }
}