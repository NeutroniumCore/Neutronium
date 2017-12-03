namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    public class CfxContextMenu 
    {
        public static bool IsEdition(ContextMenuId contextMenuId) 
        {
            var intValue = (int)contextMenuId;
            return IsEdition(intValue);
        }

        public static bool IsEdition(int intValue) 
        {
            return (intValue >= (int)ContextMenuId.MENU_ID_UNDO) && (intValue <= (int)ContextMenuId.MENU_ID_SELECT_ALL);
        }

        public static bool IsUserDefined(int intValue)
        {
            return (intValue >= (int)ContextMenuId.MENU_ID_USER_FIRST) && (intValue <= (int)ContextMenuId.MENU_ID_USER_LAST);
        }
    }
}
