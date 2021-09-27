using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeeAnonAddy
{
    internal class CreateAddressMenuItem : IMenuItem
    {
        private readonly ToolStripMenuItem menuItem;

        public CreateAddressMenuItem()
        {
            this.menuItem = new ToolStripMenuItem
            {
                Name = "m_ctxToolsAnonAddyAddress",
                Text = "User Name Field: AnonAddy UUID Address"
            };
        }

        internal void OnClick(Func<Task> onClick)
        {
            this.menuItem.Click += async (sender, args) =>
            {
                await onClick.Invoke();
            };
        }

        public ToolStripItem ToToolStripItem()
        {
            return this.menuItem;
        }
    }
}