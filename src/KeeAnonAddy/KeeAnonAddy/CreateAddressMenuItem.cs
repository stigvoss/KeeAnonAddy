using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeeAnonAddy
{
    internal class CreateAddressMenuItem
    {
        private readonly ToolStripMenuItem menuItem;

        public CreateAddressMenuItem()
        {
            menuItem = new ToolStripMenuItem
            {
                Name = "m_ctxToolsAnonAddyAddress",
                Text = "User Name Field: AnonAddy UUID Address"
            };
        }

        internal void OnClick(Func<Task> onClick)
        {
            menuItem.Click += async (sender, args) =>
            {
                await onClick?.Invoke();
            };
        }

        public ToolStripItem ToToolStripItem()
        {
            return menuItem;
        }
    }
}