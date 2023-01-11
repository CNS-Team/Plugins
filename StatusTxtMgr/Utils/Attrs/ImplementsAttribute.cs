namespace StatusTxtMgr.Utils.Attrs
{
    public class ImplementsAttribute : Attribute
    {
        public Type[] ImplementsTypes;

        public ImplementsAttribute(params Type[] implementsTypes)
        {
            ImplementsTypes = implementsTypes;
        }
    }
}
