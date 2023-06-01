namespace Scenes.InGame.Items
{
    public class ItemKey : ItemStack
    {
        public ItemKey()
        {
            ItemName = ItemKind.Key;
        }

        public override void OnConsume()
        {
            // Do Nothing
        }

        public override void OnHold()
        {
            // Do Nothing
        }
    }
}
