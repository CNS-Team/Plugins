namespace StatusTxtMgr.Utils.Attrs;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class ImplementsAttribute : Attribute
{
    public Type[] ImplementsTypes;

    public ImplementsAttribute(params Type[] implementsTypes)
    {
        this.ImplementsTypes = implementsTypes;
    }
}